using System;
using Dev.Cortez.StateMachines.Core;
using Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.ViewModels;
using Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.VisualElements;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Fields;
using NUnit.Framework;
using Unity.Properties;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.Views.StateMachineMenu
{
    public class StateMachineMenuView : VisualElement
    {
        private const string UxmlPath =
            "Packages/dev.cortez.state-machines/Editor/StateMachineContainerEditor/Views/StateMachineMenu/StateMachineMenu.uxml";

        public event Action<StateMachineDefinitionViewModel> SaveButtonPressed;
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

        public event Action ExitButtonPressed;

        private StateMachineDefinitionViewModel _stateMachineDefinitionViewModel;
        private TypePickerDropdownField _stateMachineTypeDropdown;
        private TypePickerDropdownField _transitionSolverTypeDropdown;
        private PropertyField _payloadPropertyField;
        private Button _saveButton;
        private Button _exitButton;
        private Button _addStateButton;
        private MultiColumnListView _statesListView;
        private VisualElement _stateMachineGraphContainer;
        private StateMachineGraphView.StateMachineGraphView _stateMachineGraphView;
        private ListView _transitionableStatesListView;

        public StateMachineMenuView()
        {
            var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UxmlPath);
            visualTreeAsset.CloneTree(this);
            Initialize();
            BindStatesList();

            style.flexGrow = 1;
            style.flexShrink = 0;
            style.flexBasis = 0;

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
            // Unregister top-level button handlers to avoid leaks
            if (_saveButton != null)
            {
                _saveButton.clicked -= OnSaveButtonPressed;
            }

            if (_exitButton != null)
            {
                _exitButton.clicked -= OnExitButtonPressed;
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

            SaveButtonPressed = null;
            ExitButtonPressed = null;
            EditStateButtonPressed = null;
            DeleteStateButtonPressed = null;
            AddNewStateButtonPressed = null;
            AddTransitionButtonPressed = null;
            RemoveTransitionButtonPressed = null;
            EditTransitionButtonPressed = null;
        }

        private void Initialize()
        {
            _stateMachineTypeDropdown = this.Q<TypePickerDropdownField>("StateMachineTypeDropdown");
            _transitionSolverTypeDropdown = this.Q<TypePickerDropdownField>("TransitionSolverTypeDropdown");
            _payloadPropertyField = this.Q<PropertyField>("PayloadPropertyField");
            _saveButton = this.Q<Button>("SaveButton");
            _exitButton = this.Q<Button>("ExitButton");
            _addStateButton = this.Q<Button>("AddStateButton");
            _statesListView = this.Q<MultiColumnListView>("StatesListView");
            _stateMachineGraphContainer = this.Q<VisualElement>("StateMachineGraphContainer");
            _stateMachineGraphView = new StateMachineGraphView.StateMachineGraphView();
            _stateMachineGraphContainer.Add(_stateMachineGraphView);
            _transitionableStatesListView = this.Q<ListView>("TransitionableStatesListView");

            _saveButton.clicked += OnSaveButtonPressed;
            _exitButton.clicked += OnExitButtonPressed;
            _addStateButton.clicked += OnAddStateButtonPressed;

            _stateMachineTypeDropdown.RegisterValueChangedCallback(OnChangedStateMachineTypeDropdown);
            _transitionSolverTypeDropdown.RegisterValueChangedCallback(OnChangedTransitionSolverTypeDropdown);

            Assert.NotNull(_stateMachineTypeDropdown);
            Assert.NotNull(_transitionSolverTypeDropdown);
            Assert.NotNull(_payloadPropertyField);
            Assert.NotNull(_saveButton);
            Assert.NotNull(_exitButton);
            Assert.NotNull(_addStateButton);
            Assert.NotNull(_statesListView);
        }

        private void BindStatesList()
        {
            _statesListView.columns[0].bindCell = DoBindStateIdCell;
            _statesListView.columns[1].bindCell = DoBindStateNameCell;
            _statesListView.columns[2].bindCell = DoBindStateDescriptionCell;
            _statesListView.columns[3].bindCell = DoBindStateTypeCell;
            _statesListView.columns[4].bindCell = DoBindStateMenuCell;
            _statesListView.columns[4].unbindCell = DoUnbindStateMenuCell;

            // For the TransitionableStatesListView we assign both bind and unbind handlers to avoid accumulating subscriptions
            _transitionableStatesListView.bindItem = DoBindTransitionableStateItem;
            _transitionableStatesListView.unbindItem = DoUnbindTransitionableStateItem;
        }

        private void DoBindStateIdCell(VisualElement visualElement, int index)
        {
            BindStateTablePropertyCell(visualElement, nameof(StateDefinitionViewModel.Id), index);
        }

        private void DoBindTransitionableStateItem(VisualElement visualElement, int index)
        {
            var button = visualElement.Q<Button>();
            var listView = visualElement.Q<ListView>();

            // Ensure we don't accumulate multiple handlers on rebinds
            if (button != null)
            {
                if (button.userData is Action prevAddHandler)
                {
                    button.clicked -= prevAddHandler;
                }

                Action addHandler = () => OnAddTransitionButtonPressed(_stateMachineDefinitionViewModel.States[index]);
                button.userData = addHandler;
                button.clicked += addHandler;
            }

            if (listView != null)
            {
                var stateVm = _stateMachineDefinitionViewModel.States[index];

                // Assign (not add) bind/unbind to avoid duplicate subscriptions per virtualization cycle
                listView.bindItem = (element, itemIndex) => DoBindTransitionItemEntry(element, stateVm, itemIndex);
                listView.unbindItem = (element, itemIndex) => DoUnbindTransitionItemEntry(element);
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
        }

        private void DoBindStateNameCell(VisualElement visualElement, int index)
        {
            BindStateTablePropertyCell(visualElement, nameof(StateDefinitionViewModel.Name), index);
        }

        private void DoBindStateDescriptionCell(VisualElement visualElement, int index)
        {
            BindStateTablePropertyCell(visualElement, nameof(StateDefinitionViewModel.Description), index);
        }

        private void DoBindStateTypeCell(VisualElement visualElement, int index)
        {
            BindStateTablePropertyCell(visualElement, nameof(StateDefinitionViewModel.TypeNameShort), index);
        }

        private void OnSaveButtonPressed()
        {
            SaveButtonPressed?.Invoke(_stateMachineDefinitionViewModel);
        }

        private void OnAddStateButtonPressed()
        {
            AddNewStateButtonPressed?.Invoke(_stateMachineDefinitionViewModel);
        }

        private void OnExitButtonPressed()
        {
            ExitButtonPressed?.Invoke();
        }

        private void OnChangedTransitionSolverTypeDropdown(ChangeEvent<string> changeEvent)
        {
            _stateMachineDefinitionViewModel.TransitionSolverTypeName = changeEvent.newValue;
        }

        private void OnChangedStateMachineTypeDropdown(ChangeEvent<string> changeEvent)
        {
            _stateMachineDefinitionViewModel.TypeName = changeEvent.newValue;
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

        private void DoUnbindStateMenuCell(VisualElement visualElement, int index)
        {
            var itemOptionsMenu = visualElement.Q<ItemOptionsMenu>();

            itemOptionsMenu?.UnsubscribeFromEvents();
        }

        private void OnAddTransitionButtonPressed(StateDefinitionViewModel stateDefinitionViewModel)
        {
            AddTransitionButtonPressed?.Invoke(_stateMachineDefinitionViewModel, stateDefinitionViewModel);
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

            // Unassign inner ListView bind/unbind and let UXML data binding manage itemsSource
            var listView = visualElement.Q<ListView>();

            if (listView != null)
            {
                listView.unbindItem = null;
                listView.bindItem = null;
            }
        }

        private void DoUnbindTransitionItemEntry(VisualElement visualElement)
        {
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
        }
    }
}