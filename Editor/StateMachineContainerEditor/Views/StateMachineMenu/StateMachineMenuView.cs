using System;
using Dev.Cortez.StateMachines.Core;
using Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.ViewModels;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Fields;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.Views.StateMachineMenu
{
    public class StateMachineMenuView : VisualElement
    {
        private const string UxmlPath =
            "Packages/dev.cortez.state-machines/Editor/StateMachineContainerEditor/Views/StateMachineMenu/StateMachineMenu.uxml";

        public event Action<StateMachineDefinitionViewModel> SaveButtonPressed;
        public event Action ExitButtonPressed;

        private StateMachineDefinitionViewModel _stateMachineDefinitionViewModel;

        private TypePickerDropdownField _stateMachineTypeDropdown;
        private TypePickerDropdownField _transitionSolverTypeDropdown;
        private PropertyField _payloadPropertyField;
        private Button _saveButton;
        private Button _exitButton;

        public StateMachineMenuView()
        {
            var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UxmlPath);
            visualTreeAsset.CloneTree(this);
            Initialize();

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
        }

        private void OnDetached(DetachFromPanelEvent detachFromPanelEvent)
        {
            SaveButtonPressed = null;
            ExitButtonPressed = null;
        }

        private void Initialize()
        {
            _stateMachineTypeDropdown = this.Q<TypePickerDropdownField>("StateMachineTypeDropdown");
            _transitionSolverTypeDropdown = this.Q<TypePickerDropdownField>("TransitionSolverTypeDropdown");
            _payloadPropertyField = this.Q<PropertyField>("PayloadPropertyField");
            _saveButton = this.Q<Button>("SaveButton");
            _exitButton = this.Q<Button>("ExitButton");
            _saveButton.clicked += OnSaveButtonPressed;
            _exitButton.clicked += OnExitButtonPressed;

            _stateMachineTypeDropdown.RegisterValueChangedCallback(OnChangedStateMachineTypeDropdown);
            _transitionSolverTypeDropdown.RegisterValueChangedCallback(OnChangedTransitionSolverTypeDropdown);

            Assert.NotNull(_stateMachineTypeDropdown);
            Assert.NotNull(_transitionSolverTypeDropdown);
            Assert.NotNull(_payloadPropertyField);
            Assert.NotNull(_saveButton);
        }

        private void OnSaveButtonPressed()
        {
            SaveButtonPressed?.Invoke(_stateMachineDefinitionViewModel);
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
    }
}