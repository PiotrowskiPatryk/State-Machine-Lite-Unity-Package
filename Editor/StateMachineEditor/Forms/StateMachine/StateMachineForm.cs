using System;
using Dev.Cortez.StateMachines.Core;
using Dev.Cortez.StateMachines.Core.Interfaces;
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
        private TypePickerDropdownField _typePicker;
        private TypePickerDropdownField _transitionSolverPicker;

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

            _typePicker = RootVisualElement.Q<TypePickerDropdownField>("TypePicker");
            _typePicker.ChangeType(typeof(IStateMachine), Type.GetType(Data.TypeName));

            _transitionSolverPicker = RootVisualElement.Q<TypePickerDropdownField>("TransitionSolverPicker");
            _transitionSolverPicker.ChangeType(typeof(ITransitionSolver), Type.GetType(Data.TransitionSolverTypeName));

            _typePicker.RegisterValueChangedCallback(OnTypeValueChanged);
            _transitionSolverPicker.RegisterValueChangedCallback(OnTransitionSolverTypeValueChanged);

            _submitButton.clicked += Submit;
            _cancelButton.clicked += Cancel;
        }

        private void OnTransitionSolverTypeValueChanged(ChangeEvent<string> transitionSolverTypeValue)
        {
            // Use SelectedTypeAssemblyQualifiedName to get the full type name for storage
            Data.TransitionSolverTypeName = _transitionSolverPicker.SelectedTypeAssemblyQualifiedName;
        }

        private void OnTypeValueChanged(ChangeEvent<string> stateMachineTypeValue)
        {
            // Use SelectedTypeAssemblyQualifiedName to get the full type name for storage
            Data.TypeName = _typePicker.SelectedTypeAssemblyQualifiedName;
            _propertyField.BindProperty(Data.Payload);
        }
    }
}
