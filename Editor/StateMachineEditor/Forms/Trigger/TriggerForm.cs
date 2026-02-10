using System;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Data.Repository;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.ViewModels;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Views.VisualElements;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Forms.Trigger
{
    public class TriggerForm : FormBaseWindow<TriggerDefinitionViewModel>
    {
        private Button _submitButton;
        private Button _cancelButton;
        private PropertyField _propertyField;
        private TypePickerDropdownField _typePicker;

        protected override string FORM_PATH => StateMachineEditorViewRepository.TRIGGER_FORM_PATH;

        protected override bool IsInputDataValid()
        {
            return Data.IsValid;
        }

        protected override void OnShown()
        {
            _submitButton = RootVisualElement.Q<Button>("SubmitButton");
            _cancelButton = RootVisualElement.Q<Button>("CancelButton");
            _propertyField = RootVisualElement.Q<PropertyField>();
            _propertyField.BindProperty(Data.Payload);

            _typePicker = RootVisualElement.Q<TypePickerDropdownField>();
            _typePicker.ChangeType(typeof(ITrigger), Type.GetType(Data.TypeName));

            _typePicker.RegisterValueChangedCallback(OnTypeValueChanged);

            _submitButton.clicked += Submit;
            _cancelButton.clicked += Cancel;
        }

        private void OnTypeValueChanged(ChangeEvent<string> triggerValue)
        {
            // Use SelectedTypeAssemblyQualifiedName to get the full type name for storage
            Data.TypeName = _typePicker.SelectedTypeAssemblyQualifiedName;
            _propertyField.BindProperty(Data.Payload);
        }
    }
}
