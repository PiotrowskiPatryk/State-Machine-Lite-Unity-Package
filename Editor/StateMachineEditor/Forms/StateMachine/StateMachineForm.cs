using System;
using Dev.Cortez.StateMachines.Core;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Data.Repository;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.ViewModels;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Views.VisualElements;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Forms.StateMachine
{
    public class StateMachineForm : FormBaseWindow<StateMachineDefinitionViewModel>
    {
        private Button _submitButton;
        private Button _cancelButton;
        private PropertyField _propertyField;

        protected override string FORM_PATH => StateMachineEditorViewRepository.STATE_MACHINE_FORM_PATH;

        protected override bool IsInputDataValid()
        {
            return !string.IsNullOrWhiteSpace(Data.Name) && !string.IsNullOrWhiteSpace(Data.TypeName);
        }

        protected override void OnShown()
        {
            _submitButton = RootVisualElement.Q<Button>("SubmitButton");
            _cancelButton = RootVisualElement.Q<Button>("CancelButton");
            _propertyField = RootVisualElement.Q<PropertyField>();
            _propertyField.BindProperty(Data.Payload);

            var typePicker = RootVisualElement.Q<TypePickerDropdownField>("TypePicker");
            typePicker.ChangeType(typeof(IStateMachine), Type.GetType(Data.TypeName));

            var transitionSolverPicker = RootVisualElement.Q<TypePickerDropdownField>("TransitionSolverPicker");
            transitionSolverPicker.ChangeType(typeof(ITransitionSolver), Type.GetType(Data.TransitionSolverTypeName));

            typePicker.RegisterValueChangedCallback(OnTypeValueChanged);
            transitionSolverPicker.RegisterValueChangedCallback(OnTransitionSolverTypeValueChanged);

            _submitButton.clicked += Submit;
            _cancelButton.clicked += Cancel;
        }

        private void OnTransitionSolverTypeValueChanged(ChangeEvent<string> transitionSolverTypeValue)
        {
            Data.TransitionSolverTypeName = transitionSolverTypeValue.newValue;
        }

        private void OnTypeValueChanged(ChangeEvent<string> stateMachineTypeValue)
        {
            Data.TypeName = stateMachineTypeValue.newValue;
            _propertyField.BindProperty(Data.Payload);
        }
    }
}