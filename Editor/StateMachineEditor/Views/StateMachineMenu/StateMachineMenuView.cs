using System;
using System.Collections.Generic;
using System.Linq;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Data.Repository;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Utilities;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.ViewModels;
using Unity.Properties;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;
using ItemOptionsMenu = Dev.Cortez.StateMachines.Editor.StateMachineEditor.Views.VisualElements.ItemOptionsMenu;
using TypePickerDropdownField =
    Dev.Cortez.StateMachines.Editor.StateMachineEditor.Views.VisualElements.TypePickerDropdownField;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Views.StateMachineMenu
{
    public class StateMachineMenuView : VisualElement
    {
        // Inspector <-> Graph wiring helpers
        private readonly Dictionary<string, Foldout> _foldoutByStateId = new();
        private readonly Dictionary<string, ListView> _transitionsListByStateId = new();

        public event Action<StateMachineDefinitionViewModel> AddNewStateButtonPressed;
        public event Action<StateMachineDefinitionViewModel, StateDefinitionViewModel> EditStateButtonPressed;
        public event Action<StateMachineDefinitionViewModel, int> DeleteStateButtonPressed;
        public event Action<StateMachineDefinitionViewModel, StateDefinitionViewModel> AddTransitionButtonPressed;

        public event
            Action<StateMachineDefinitionViewModel, StateDefinitionViewModel, TransitionRuleDefinitionViewModel>
            RemoveTransitionButtonPressed;

        public event
            Action<StateMachineDefinitionViewModel, StateDefinitionViewModel, TransitionRuleDefinitionViewModel>
            EditTransitionButtonPressed;

        public event Action BackButtonPressed;

        public event Action<StateMachineDefinitionViewModel, StateDefinitionViewModel, StateDefinitionViewModel>
            CreateTransitionBetweenStatesRequested;

        private StateMachineDefinitionViewModel _stateMachineDefinitionViewModel;
        private TypePickerDropdownField _stateMachineTypeDropdown;
        private TypePickerDropdownField _transitionSolverTypeDropdown;
        private PropertyField _payloadPropertyField;
        private Button _backButton;
        private Button _addStateButton;
        private MultiColumnListView _statesListView;
        private VisualElement _stateMachineGraphContainer;
        private StateMachineGraphView.StateMachineGraphView _stateMachineGraphView;
        private ListView _transitionableStatesListView;
        private Foldout _lastHighlightedFoldout;
        private DropdownField _initialStateDropdownField;

        public StateMachineMenuView()
        {
            var visualTreeAsset =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(StateMachineEditorViewRepository.
                    STATE_MACHINE_MENU_VIEW_PATH);
            visualTreeAsset.CloneTree(this);
            Initialize();
            BindStatesList();

            style.flexGrow = 1;
            style.flexShrink = 1;
            style.flexBasis = StyleKeyword.Auto;

            RegisterCallback<DetachFromPanelEvent>(OnDetached);
        }

        public void Bind(StateMachineDefinitionViewModel stateMachineDefinitionViewModel)
        {
            _stateMachineDefinitionViewModel = stateMachineDefinitionViewModel;
            dataSource = stateMachineDefinitionViewModel;
            _transitionSolverTypeDropdown.ChangeType(typeof(ITransitionSolver),
                Type.GetType(stateMachineDefinitionViewModel.TransitionSolverTypeName));
            _stateMachineTypeDropdown.ChangeType(typeof(IStateMachine),
                Type.GetType(stateMachineDefinitionViewModel.TypeName));
            _payloadPropertyField.BindProperty(stateMachineDefinitionViewModel.Payload);
            _stateMachineGraphView.Bind(stateMachineDefinitionViewModel);
        }

        private void OnDetached(DetachFromPanelEvent detachFromPanelEvent)
        {
            if (_backButton != null)
            {
                _backButton.clicked -= OnBackButtonPressed;
            }

            if (_addStateButton != null)
            {
                _addStateButton.clicked -= OnAddStateButtonPressed;
            }

            // Unregister dropdown value change callbacks
            if (_stateMachineTypeDropdown != null)
            {
                _stateMachineTypeDropdown.UnregisterValueChangedCallback(OnChangedStateMachineTypeDropdown);
            }

            if (_transitionSolverTypeDropdown != null)
            {
                _transitionSolverTypeDropdown.UnregisterValueChangedCallback(OnChangedTransitionSolverTypeDropdown);
            }

            // Unwire graph events
            if (_stateMachineGraphView != null)
            {
                _stateMachineGraphView.NodeClicked -= OnGraphNodeClicked;
                _stateMachineGraphView.EdgeClicked -= OnGraphEdgeClicked;
                _stateMachineGraphView.CreateTransitionRequested -= OnGraphCreateTransitionRequested;
            }

            BackButtonPressed = null;
            EditStateButtonPressed = null;
            DeleteStateButtonPressed = null;
            AddNewStateButtonPressed = null;
            AddTransitionButtonPressed = null;
            RemoveTransitionButtonPressed = null;
            EditTransitionButtonPressed = null;
            CreateTransitionBetweenStatesRequested = null;
        }

        private void Initialize()
        {
            _stateMachineTypeDropdown = this.Q<TypePickerDropdownField>("StateMachineTypeDropdown");
            _transitionSolverTypeDropdown = this.Q<TypePickerDropdownField>("TransitionSolverTypeDropdown");
            _payloadPropertyField = this.Q<PropertyField>("PayloadPropertyField");
            _backButton = this.Q<Button>("BackButton");
            _addStateButton = this.Q<Button>("AddStateButton");
            _statesListView = this.Q<MultiColumnListView>("StatesListView");
            _stateMachineGraphContainer = this.Q<VisualElement>("StateMachineGraphContainer");
            _stateMachineGraphView = new StateMachineGraphView.StateMachineGraphView();
            _stateMachineGraphContainer.Add(_stateMachineGraphView);
            _transitionableStatesListView = this.Q<ListView>("TransitionableStatesListView");
            _initialStateDropdownField = this.Q<DropdownField>("InitialStateDropdownField");

            // Wire graph <-> inspector events
            _stateMachineGraphView.NodeClicked += OnGraphNodeClicked;
            _stateMachineGraphView.EdgeClicked += OnGraphEdgeClicked;
            _stateMachineGraphView.CreateTransitionRequested += OnGraphCreateTransitionRequested;
            _backButton.clicked += OnBackButtonPressed;
            _addStateButton.clicked += OnAddStateButtonPressed;

            _stateMachineTypeDropdown.RegisterValueChangedCallback(OnChangedStateMachineTypeDropdown);
            _transitionSolverTypeDropdown.RegisterValueChangedCallback(OnChangedTransitionSolverTypeDropdown);

            Debug.Assert(_stateMachineTypeDropdown != null, "_stateMachineTypeDropdown should not be null");
            Debug.Assert(_transitionSolverTypeDropdown != null, "_transitionSolverTypeDropdown should not be null");
            Debug.Assert(_payloadPropertyField != null, "_payloadPropertyField should not be null");
            Debug.Assert(_backButton != null, "_exitButton should not be null");
            Debug.Assert(_addStateButton != null, "_addStateButton should not be null");
            Debug.Assert(_statesListView != null, "_statesListView should not be null");
            Debug.Assert(_initialStateDropdownField != null, "_initialStateDropdownField should not be null");
        }

        private void BindStatesList()
        {
            _statesListView.columns[0].bindCell = DoBindStateIdCell;
            _statesListView.columns[1].bindCell = DoBindStateNameCell;
            _statesListView.columns[2].bindCell = DoBindStateDescriptionCell;
            _statesListView.columns[3].bindCell = DoBindStateTypeCell;
            _statesListView.columns[4].makeCell = () => new ItemOptionsMenu();
            _statesListView.columns[4].bindCell = DoBindStateMenuCell;
            _statesListView.columns[4].unbindCell = DoUnbindStateMenuCell;
            _statesListView.itemIndexChanged += OnStateItemIndexChanged;

            // For the TransitionableStatesListView we assign both bind and unbind handlers to avoid accumulating subscriptions
            _transitionableStatesListView.bindItem = DoBindTransitionableStateItem;
            _transitionableStatesListView.unbindItem = DoUnbindTransitionableStateItem;
        }

        private void OnStateItemIndexChanged(int sourceIndex, int destinationIndex)
        {
            _stateMachineDefinitionViewModel?.MoveStateAtIndex(sourceIndex, destinationIndex);
        }

        private static void DoBindStateIdCell(VisualElement visualElement, int index)
        {
            BindStateTablePropertyCell(visualElement, nameof(StateDefinitionViewModel.Id), index);
        }

        private void DoBindTransitionableStateItem(VisualElement visualElement, int index)
        {
            var button = visualElement.Q<Button>();
            var listView = visualElement.Q<ListView>();
            var foldout = visualElement.Q<Foldout>("TransitionableStateItemEntry");

            var stateVm = _stateMachineDefinitionViewModel.States[index];

            // Map state id to its UI parts for cross-highlighting
            if (stateVm != null)
            {
                _transitionsListByStateId[stateVm.Id] = listView;
                _foldoutByStateId[stateVm.Id] = foldout;
            }

            // Ensure we don't accumulate multiple handlers on rebinds
            if (button != null)
            {
                if (button.userData is Action prevAddHandler)
                {
                    button.clicked -= prevAddHandler;
                }

                Action addHandler = () => OnAddTransitionButtonPressed(stateVm);
                button.userData = addHandler;
                button.clicked += addHandler;
            }

            // Clicking the foldout highlights and centers the corresponding node
            if (foldout != null)
            {
                if (foldout.userData is EventCallback<PointerUpEvent> prevFoldoutCb)
                {
                    foldout.UnregisterCallback(prevFoldoutCb);
                }

                EventCallback<PointerUpEvent> foldoutCb = evt =>
                {
                    if (evt.button != 0)
                    {
                        return;
                    }

                    _stateMachineGraphView?.HighlightNodeById(stateVm.Id);
                    evt.StopPropagation();
                };
                foldout.userData = foldoutCb;
                foldout.RegisterCallback(foldoutCb);
            }

            if (listView != null)
            {
                // Assign (not add) bind/unbind to avoid duplicate subscriptions per virtualization cycle
                listView.bindItem = (element, itemIndex) => DoBindTransitionItemEntry(element, stateVm, itemIndex);
                listView.unbindItem = (element, itemIndex) => DoUnbindTransitionItemEntry(element);
            }

            // Add validation icon for state in the foldout header
            if (stateVm != null && foldout != null)
            {
                // Get the toggle element within the foldout to insert the icon
                var toggle = foldout.Q<Toggle>();

                if (toggle != null)
                {
                    var stateIcon = ValidationIconHelper.GetOrCreateValidationIcon(toggle);
                    ValidationIconHelper.UpdateValidationIconVisibility(stateIcon, stateVm.IsValid);
                }
            }
        }

        private void DoBindTransitionItemEntry(VisualElement visualElement,
            StateDefinitionViewModel stateDefinitionViewModel, int index)
        {
            var removeItemButton = visualElement.Q<Button>("RemoveButton");
            var editItemButton = visualElement.Q<Button>("EditButton");

            // Ensure handlers do not pile up across rebinds
            if (removeItemButton != null)
            {
                if (removeItemButton.userData is Action prevRemove)
                {
                    removeItemButton.clicked -= prevRemove;
                }

                Action removeHandler = () =>
                {
                    RemoveTransitionButtonPressed?.Invoke(_stateMachineDefinitionViewModel, stateDefinitionViewModel,
                        stateDefinitionViewModel.Transitions[index]);
                };
                removeItemButton.userData = removeHandler;
                removeItemButton.clicked += removeHandler;
            }

            if (editItemButton != null)
            {
                if (editItemButton.userData is Action prevEdit)
                {
                    editItemButton.clicked -= prevEdit;
                }

                Action editHandler = () =>
                {
                    EditTransitionButtonPressed?.Invoke(_stateMachineDefinitionViewModel, stateDefinitionViewModel,
                        stateDefinitionViewModel.Transitions[index]);
                };
                editItemButton.userData = editHandler;
                editItemButton.clicked += editHandler;
            }

            // Row click highlights edge in graph
            if (visualElement != null)
            {
                if (visualElement.userData is EventCallback<PointerUpEvent> prev)
                {
                    visualElement.UnregisterCallback(prev);
                }

                EventCallback<PointerUpEvent> rowClick = evt =>
                {
                    if (evt.button != 0)
                    {
                        return;
                    }

                    var trVm = stateDefinitionViewModel.Transitions[index];
                    _stateMachineGraphView?.HighlightTransitionByPath(trVm.SerializedProperty.propertyPath);
                    // Do not highlight node when selecting a transition to keep selection exclusive to the edge
                    evt.StopPropagation();
                };
                visualElement.userData = rowClick;
                visualElement.RegisterCallback(rowClick);
            }

            // Add validation icon for transition
            var transitionVm = stateDefinitionViewModel.Transitions[index];
            var transitionIcon = ValidationIconHelper.GetOrCreateValidationIcon(visualElement);
            ValidationIconHelper.UpdateValidationIconVisibility(transitionIcon, transitionVm.IsValid);
        }

        private void DoBindStateNameCell(VisualElement visualElement, int index)
        {
            BindStateTablePropertyCell(visualElement, nameof(StateDefinitionViewModel.Name), index);

            // Add validation icon next to name
            var icon = ValidationIconHelper.GetOrCreateValidationIcon(visualElement);
            var stateVm = _stateMachineDefinitionViewModel.States[index];

            if (stateVm != null)
            {
                ValidationIconHelper.UpdateValidationIconVisibility(icon, stateVm.IsValid);
            }
        }

        private static void DoBindStateDescriptionCell(VisualElement visualElement, int index)
        {
            BindStateTablePropertyCell(visualElement, nameof(StateDefinitionViewModel.Description), index);
        }

        private static void DoBindStateTypeCell(VisualElement visualElement, int index)
        {
            BindStateTablePropertyCell(visualElement, nameof(StateDefinitionViewModel.TypeNameShort), index);
        }

        private void OnAddStateButtonPressed()
        {
            AddNewStateButtonPressed?.Invoke(_stateMachineDefinitionViewModel);
        }

        private void OnBackButtonPressed()
        {
            BackButtonPressed?.Invoke();
        }

        private void OnChangedTransitionSolverTypeDropdown(ChangeEvent<string> changeEvent)
        {
            // Use SelectedTypeAssemblyQualifiedName to get the full type name for storage
            _stateMachineDefinitionViewModel.TransitionSolverTypeName =
                _transitionSolverTypeDropdown.SelectedTypeAssemblyQualifiedName;
        }

        private void OnChangedStateMachineTypeDropdown(ChangeEvent<string> changeEvent)
        {
            // Use SelectedTypeAssemblyQualifiedName to get the full type name for storage
            _stateMachineDefinitionViewModel.TypeName = _stateMachineTypeDropdown.SelectedTypeAssemblyQualifiedName;
        }

        private static void BindStateTablePropertyCell(VisualElement visualElement, string propertyName,
            int index)
        {
            var label = visualElement.Q<Label>();
            label.SetBinding("value", new DataBinding
            {
                dataSourcePath =
                    new PropertyPath(
                        $"{nameof(StateMachineDefinitionViewModel.States)}[{index}].{propertyName}"),
                bindingMode = BindingMode.ToTarget
            });
        }

        private static void DoUnbindStateMenuCell(VisualElement visualElement, int index)
        {
            var itemOptionsMenu = visualElement.Q<ItemOptionsMenu>();

            itemOptionsMenu?.UnsubscribeFromEvents();
        }

        private void OnAddTransitionButtonPressed(StateDefinitionViewModel stateDefinitionViewModel)
        {
            AddTransitionButtonPressed?.Invoke(_stateMachineDefinitionViewModel, stateDefinitionViewModel);
        }

        // ---- Graph event handlers ----
        private void OnGraphNodeClicked(string stateId)
        {
            if (string.IsNullOrEmpty(stateId))
            {
                return;
            }

            if (_foldoutByStateId.TryGetValue(stateId, out var foldout) && foldout != null)
            {
                foldout.value = true; // expand
                HighlightFoldout(foldout);
            }
        }

        private void OnGraphEdgeClicked(string sourceStateId, string transitionPropertyPath)
        {
            if (string.IsNullOrEmpty(sourceStateId) || string.IsNullOrEmpty(transitionPropertyPath))
            {
                return;
            }

            // expand correct state foldout
            if (_foldoutByStateId.TryGetValue(sourceStateId, out var foldout) && foldout != null)
            {
                foldout.value = true;
                HighlightFoldout(foldout);
            }

            // select transition row
            if (_transitionsListByStateId.TryGetValue(sourceStateId, out var list) && list != null)
            {
                var stateVm = _stateMachineDefinitionViewModel?.States.FirstOrDefault(s => s.Id == sourceStateId);

                if (stateVm != null)
                {
                    for (var i = 0; i < stateVm.Transitions.Count; i++)
                    {
                        if (stateVm.Transitions[i].SerializedProperty.propertyPath == transitionPropertyPath)
                        {
                            list.selectedIndex = i;

                            break;
                        }
                    }
                }
            }
        }

        private void OnGraphCreateTransitionRequested(string sourceStateId, string targetStateId)
        {
            var source = _stateMachineDefinitionViewModel?.States.FirstOrDefault(s => s.Id == sourceStateId);
            var target = _stateMachineDefinitionViewModel?.States.FirstOrDefault(s => s.Id == targetStateId);

            if (source == null || target == null)
            {
                return;
            }

            CreateTransitionBetweenStatesRequested?.Invoke(_stateMachineDefinitionViewModel, source, target);
        }

        private void HighlightFoldout(Foldout foldout)
        {
            if (_lastHighlightedFoldout != null && _lastHighlightedFoldout != foldout)
            {
                // reset styling
                ResetFoldoutHighlight(_lastHighlightedFoldout);
            }

            _lastHighlightedFoldout = foldout;
            var c = new Color(1f, 0.9f, 0.2f, 1f);
            foldout.style.borderLeftColor = c;
            foldout.style.borderRightColor = c;
            foldout.style.borderTopColor = c;
            foldout.style.borderBottomColor = c;
        }

        private static void ResetFoldoutHighlight(Foldout foldout)
        {
            var c = new Color(0.063f, 0.098f, 0.133f, 1f); // close to panel bg border
            foldout.style.borderLeftColor = c;
            foldout.style.borderRightColor = c;
            foldout.style.borderTopColor = c;
            foldout.style.borderBottomColor = c;
        }

        private void DoBindStateMenuCell(VisualElement visualElement, int index)
        {
            var itemOptionsMenu = visualElement.Q<ItemOptionsMenu>();

            // TODO - Handle unsubscribing
            if (itemOptionsMenu != null)
            {
                itemOptionsMenu.SubscribeToEvents(PressedEditStateButton, PressedDeleteStateButton);
            }
            else
            {
                Debug.LogError("Unable to bind menu item.");
            }

            return;

            void PressedDeleteStateButton()
            {
                DeleteStateButtonPressed?.Invoke(_stateMachineDefinitionViewModel, index);
            }

            void PressedEditStateButton()
            {
                EditStateButtonPressed?.Invoke(_stateMachineDefinitionViewModel,
                    _stateMachineDefinitionViewModel.States[index]);
            }
        }

        private void DoUnbindTransitionableStateItem(VisualElement visualElement, int index)
        {
            // Unregister Add Transition button handler
            var button = visualElement.Q<Button>();

            if (button != null && button.userData is Action addHandler)
            {
                button.clicked -= addHandler;
                button.userData = null;
            }

            var foldout = visualElement.Q<Foldout>("TransitionableStateItemEntry");

            if (foldout != null && foldout.userData is EventCallback<PointerUpEvent> prevFoldoutCb)
            {
                foldout.UnregisterCallback(prevFoldoutCb);
                foldout.userData = null;
            }

            // Clean state lookup maps
            if (_stateMachineDefinitionViewModel != null && index >= 0 &&
                index < _stateMachineDefinitionViewModel.States.Count)
            {
                var stateVm = _stateMachineDefinitionViewModel.States[index];

                if (stateVm != null)
                {
                    _foldoutByStateId.Remove(stateVm.Id);
                    _transitionsListByStateId.Remove(stateVm.Id);
                }
            }

            // Unassign inner ListView bind/unbind and let UXML data binding manage itemsSource
            var listView = visualElement.Q<ListView>();

            if (listView != null)
            {
                listView.unbindItem = null;
                listView.bindItem = null;
            }
        }

        private static void DoUnbindTransitionItemEntry(VisualElement visualElement)
        {
            // TODO - CLEANUP THIS, apply DRY principle

            // Unregister Remove button
            var removeItemButton = visualElement.Q<Button>("RemoveButton");

            if (removeItemButton != null && removeItemButton.userData is Action prevRemove)
            {
                removeItemButton.clicked -= prevRemove;
                removeItemButton.userData = null;
            }

            // Unregister Edit button
            var editItemButton = visualElement.Q<Button>("EditButton");

            if (editItemButton != null && editItemButton.userData is Action prevEdit)
            {
                editItemButton.clicked -= prevEdit;
                editItemButton.userData = null;
            }

            // Unregister row click highlight handler
            if (visualElement != null && visualElement.userData is EventCallback<PointerUpEvent> prevRow)
            {
                visualElement.UnregisterCallback(prevRow);
                visualElement.userData = null;
            }
        }
    }
}