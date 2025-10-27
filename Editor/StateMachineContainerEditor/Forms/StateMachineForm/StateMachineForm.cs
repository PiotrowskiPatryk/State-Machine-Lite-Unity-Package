using System;
using Dev.Cortez.StateMachines.Core;
using Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.ViewModels;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Fields;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.Forms.StateMachineForm
{
    public class StateMachineForm : FormBaseWindow<StateMachineDefinitionViewModel>
    {
        private Button _submitButton;
        private Button _cancelButton;
        private PropertyField _propertyField;

        protected override bool IsInputDataValid()
        {
            return !string.IsNullOrWhiteSpace(Data.Name) && !string.IsNullOrWhiteSpace(Data.TypeName);
        }

        protected override void OnCreatedGUI()
        {
            _submitButton = rootVisualElement.Q<Button>("SubmitButton");
            _cancelButton = rootVisualElement.Q<Button>("CancelButton");
            _propertyField = rootVisualElement.Q<PropertyField>();
            _propertyField.BindProperty(Data.Payload);

            var typePicker = rootVisualElement.Q<TypePickerDropdownField>("TypePicker");
            typePicker.ChangeType(typeof(IStateMachine), Type.GetType(Data.TypeName));

            var transitionSolverPicker = rootVisualElement.Q<TypePickerDropdownField>("TransitionSolverPicker");
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