using System;
using Dev.Cortez.StateMachines.Core;
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

        protected override bool IsInputDataValid()
        {
            return !string.IsNullOrWhiteSpace(Data.Id) && !string.IsNullOrWhiteSpace(Data.Name) &&
                   !string.IsNullOrWhiteSpace(Data.TypeName);
        }

        protected override void OnCreatedGUI()
        {
            _submitButton = rootVisualElement.Q<Button>("SubmitButton");
            _cancelButton = rootVisualElement.Q<Button>("CancelButton");
            _propertyField = rootVisualElement.Q<PropertyField>();
            _propertyField.BindProperty(Data.Payload);

            var typePicker = rootVisualElement.Q<TypePickerDropdownField>("TypePicker");
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