using System;
using Dev.Cortez.StateMachines.Core;
using Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.ViewModels;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Fields;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.Forms.Trigger
{
    public class TriggerForm : FormBaseWindow<TriggerDefinitionViewModel>
    {
        private Button _submitButton;
        private Button _cancelButton;
        private PropertyField _propertyField;

        protected override bool IsInputDataValid()
        {
            return !string.IsNullOrEmpty(Data.Name) && !string.IsNullOrEmpty(Data.TypeName);
        }

        protected override void OnCreatedGUI()
        {
            _submitButton = rootVisualElement.Q<Button>("SubmitButton");
            _cancelButton = rootVisualElement.Q<Button>("CancelButton");
            _propertyField = rootVisualElement.Q<PropertyField>();
            _propertyField.BindProperty(Data.Payload);

            var typePicker = rootVisualElement.Q<TypePickerDropdownField>();
            typePicker.ChangeType(typeof(ITrigger), Type.GetType(Data.TypeName));

            typePicker.RegisterValueChangedCallback(OnTypeValueChanged);

            _submitButton.clicked += Submit;
            _cancelButton.clicked += Cancel;
        }

        private void OnTypeValueChanged(ChangeEvent<string> triggerValue)
        {
            Data.TypeName = triggerValue.newValue;
            _propertyField.BindProperty(Data.Payload);
        }
    }
}