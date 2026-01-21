using System;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Data.Repository;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.ViewModels;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Views.VisualElements;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Forms.Condition
{
    public class ConditionForm : FormBaseWindow<ConditionDefinitionViewModel>
    {
        private Button _submitButton;
        private Button _cancelButton;
        private PropertyField _propertyField;

        protected override string FORM_PATH => StateMachineEditorViewRepository.CONDITION_FORM_PATH;

        protected override bool IsInputDataValid()
        {
            return !string.IsNullOrWhiteSpace(Data.Id) && !string.IsNullOrWhiteSpace(Data.Name) &&
                   !string.IsNullOrWhiteSpace(Data.TypeName);
        }

        protected override void OnShown()
        {
            _submitButton = RootVisualElement.Q<Button>("SubmitButton");
            _cancelButton = RootVisualElement.Q<Button>("CancelButton");
            _propertyField = RootVisualElement.Q<PropertyField>();
            _propertyField.BindProperty(Data.Payload);

            var typePicker = RootVisualElement.Q<TypePickerDropdownField>("TypePicker");
            typePicker.ChangeType(typeof(ICondition), Type.GetType(Data.TypeName));

            typePicker.RegisterValueChangedCallback(OnConditionTypeChanged);

            _submitButton.clicked += Submit;
            _cancelButton.clicked += Cancel;
        }

        private void OnConditionTypeChanged(ChangeEvent<string> conditionTypeValue)
        {
            Data.TypeName = conditionTypeValue.newValue;
            _propertyField.BindProperty(Data.Payload);
        }
    }
}