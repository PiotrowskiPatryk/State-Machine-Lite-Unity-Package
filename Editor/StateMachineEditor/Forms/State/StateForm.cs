using System;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Data.Repository;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Utilities;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.ViewModels;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Views.VisualElements;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Forms.State
{
    public class StateForm : FormBaseWindow<StateDefinitionViewModel>
    {
        private Button _submitButton;
        private Button _cancelButton;
        private PropertyField _propertyField;
        private TypePickerDropdownField _typePicker;

        protected override string FORM_PATH => StateMachineEditorViewRepository.STATE_FORM_PATH;

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

            _typePicker = RootVisualElement.Q<TypePickerDropdownField>("TypePicker");
            var baseStateFromStateMachine =
                StateMachineReflectionUtilities.GetBaseStateFromStateMachine(Data.StateMachineTypeName);

            _typePicker.ChangeType(baseStateFromStateMachine, Type.GetType(Data.TypeName));
            _typePicker.RegisterValueChangedCallback(OnTypeValueChanged);

            _submitButton.clicked += Submit;
            _cancelButton.clicked += Cancel;
        }

        private void OnTypeValueChanged(ChangeEvent<string> stateMachineTypeValue)
        {
            // Use SelectedTypeAssemblyQualifiedName to get the full type name for storage
            Data.TypeName = _typePicker.SelectedTypeAssemblyQualifiedName;
            _propertyField.BindProperty(Data.Payload);
        }
    }
}
