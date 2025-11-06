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
        public event Action<StateMachineDefinitionViewModel> AddTransitionButtonPressed;

        public event Action ExitButtonPressed;

        private StateMachineDefinitionViewModel _stateMachineDefinitionViewModel;
        private TypePickerDropdownField _stateMachineTypeDropdown;
        private TypePickerDropdownField _transitionSolverTypeDropdown;
        private PropertyField _payloadPropertyField;
        private Button _saveButton;
        private Button _exitButton;
        private Button _addTransitionButton;
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
            _transitionableStatesListView.dataSource = _stateMachineDefinitionViewModel.Transitions;
        }

        private void OnDetached(DetachFromPanelEvent detachFromPanelEvent)
        {
            SaveButtonPressed = null;
            ExitButtonPressed = null;
            EditStateButtonPressed = null;
            DeleteStateButtonPressed = null;
            AddNewStateButtonPressed = null;
            AddTransitionButtonPressed = null;
        }

        private void Initialize()
        {
            _stateMachineTypeDropdown = this.Q<TypePickerDropdownField>("StateMachineTypeDropdown");
            _transitionSolverTypeDropdown = this.Q<TypePickerDropdownField>("TransitionSolverTypeDropdown");
            _payloadPropertyField = this.Q<PropertyField>("PayloadPropertyField");
            _saveButton = this.Q<Button>("SaveButton");
            _exitButton = this.Q<Button>("ExitButton");
            _addTransitionButton = this.Q<Button>("AddTransitionButton");
            _addStateButton = this.Q<Button>("AddStateButton");
            _statesListView = this.Q<MultiColumnListView>("StatesListView");
            _stateMachineGraphContainer = this.Q<VisualElement>("StateMachineGraphContainer");
            _stateMachineGraphView = new StateMachineGraphView.StateMachineGraphView();
            _stateMachineGraphContainer.Add(_stateMachineGraphView);
            _transitionableStatesListView = this.Q<ListView>("TransitionableStatesListView");

            _saveButton.clicked += OnSaveButtonPressed;
            _exitButton.clicked += OnExitButtonPressed;
            _addStateButton.clicked += OnAddStateButtonPressed;
            _addTransitionButton.clicked += OnAddTransitionButtonPressed;

            _stateMachineTypeDropdown.RegisterValueChangedCallback(OnChangedStateMachineTypeDropdown);
            _transitionSolverTypeDropdown.RegisterValueChangedCallback(OnChangedTransitionSolverTypeDropdown);

            Assert.NotNull(_stateMachineTypeDropdown);
            Assert.NotNull(_transitionSolverTypeDropdown);
            Assert.NotNull(_payloadPropertyField);
            Assert.NotNull(_saveButton);
            Assert.NotNull(_exitButton);
            Assert.NotNull(_addTransitionButton);
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
        }

        private void DoBindStateIdCell(VisualElement visualElement, int index)
        {
            BindStateTablePropertyCell(visualElement, nameof(StateDefinitionViewModel.Id), index);
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

        private void OnAddTransitionButtonPressed()
        {
            AddTransitionButtonPressed?.Invoke(_stateMachineDefinitionViewModel);
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
    }
}