using System;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.Data;
using Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.Data.Definition;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.UIToolkitUtilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Windows
{
    public class TriggerForm : BaseEditorFormWindow<TriggerForm.Result>
    {
        private Mode _mode = Mode.Create;
        private string _initialId;
        private string _initialName;
        private string _initialDescription;
        private string _initialTypeName;
        private IPayload _initialPayload;
        private TriggerDefinitionWrapper _triggerDefinitionWrapper;
        private SerializedObject _serializedWrapper;
        private SerializedProperty _dataProperty;
        private PropertyField _triggerPropertyField;

        protected override string SubmitButtonLabel => _mode == Mode.Create ? "Create" : "Save";
        protected override string CancelButtonLabel => "Cancel";

        public static void Show(Action<Result> onCreate)
        {
            var window = CreateInstance<TriggerForm>();
            window._mode = Mode.Create;
            window.OnSubmit = onCreate;
            window.titleContent = new GUIContent("New Trigger");
            window.minSize = new Vector2(640, 320);
            window.position = GetCenteredPosition(new Vector2(640, 320));
            window.ShowUtility();
        }

        public static void ShowEdit(string id, string name, string description, string typeName, IPayload payload,
            Action<Result> onSave)
        {
            var window = CreateInstance<TriggerForm>();
            window._mode = Mode.Edit;
            window.OnSubmit = onSave;
            window._initialId = id;
            window._initialName = name;
            window._initialDescription = description;
            window._initialTypeName = typeName;
            window._initialPayload = payload;
            window.titleContent = new GUIContent("Edit Trigger");
            window.minSize = new Vector2(640, 320);
            window.position = GetCenteredPosition(new Vector2(640, 320));
            window.ShowUtility();
        }

        protected override void BuildView()
        {
            _triggerDefinitionWrapper =
                FormBindingUtility.CreateWrapper<TriggerDefinitionWrapper>(out _serializedWrapper,
                    out _dataProperty);

            // Prefill based on mode before binding
            _serializedWrapper.Update();

            if (_mode == Mode.Create)
            {
                _dataProperty.FindPropertyRelative(TriggerDefinitionPropertyNames.Id).stringValue =
                    Guid.NewGuid().ToString();
                _dataProperty.FindPropertyRelative(TriggerDefinitionPropertyNames.Name).stringValue = string.Empty;
                _dataProperty.FindPropertyRelative(TriggerDefinitionPropertyNames.Description).stringValue =
                    string.Empty;
                _dataProperty.FindPropertyRelative(TriggerDefinitionPropertyNames.TypeName).stringValue = string.Empty;
                var payloadPropCreate = _dataProperty.FindPropertyRelative(TriggerDefinitionPropertyNames.Payload);

                if (payloadPropCreate != null)
                {
                    try
                    {
                        payloadPropCreate.managedReferenceValue = null;
                    }
                    catch
                    {
                    }
                }
            }
            else
            {
                _dataProperty.FindPropertyRelative(TriggerDefinitionPropertyNames.Id).stringValue =
                    _initialId ?? string.Empty;
                _dataProperty.FindPropertyRelative(TriggerDefinitionPropertyNames.Name).stringValue =
                    _initialName ?? string.Empty;
                _dataProperty.FindPropertyRelative(TriggerDefinitionPropertyNames.Description).stringValue =
                    _initialDescription ?? string.Empty;
                _dataProperty.FindPropertyRelative(TriggerDefinitionPropertyNames.TypeName).stringValue =
                    _initialTypeName ?? string.Empty;
                var payloadPropInit = _dataProperty.FindPropertyRelative(TriggerDefinitionPropertyNames.Payload);

                if (payloadPropInit != null)
                {
                    try
                    {
                        payloadPropInit.managedReferenceValue = _initialPayload;
                    }
                    catch
                    {
                    }
                }
            }

            _serializedWrapper.ApplyModifiedPropertiesWithoutUndo();

            var triggerContainer = rootVisualElement.Q<VisualElement>("TriggerContainer") ??
                                   _rootContainer ?? rootVisualElement;
            _triggerPropertyField = FormBindingUtility.AddPropertyField(triggerContainer, _serializedWrapper,
                _dataProperty, "TriggerDefinitionField");
        }

        protected override bool Validate(out string errorMessage)
        {
            _serializedWrapper.ApplyModifiedPropertiesWithoutUndo();

            var id = _dataProperty.FindPropertyRelative(TriggerDefinitionPropertyNames.Id).stringValue?.Trim() ??
                     string.Empty;
            var name = _dataProperty.FindPropertyRelative(TriggerDefinitionPropertyNames.Name).stringValue?.Trim() ??
                       string.Empty;
            var typeName =
                _dataProperty.FindPropertyRelative(TriggerDefinitionPropertyNames.TypeName).stringValue?.Trim() ??
                string.Empty;

            if (!FormValidationUtility.RequiredString(id, "ID", out errorMessage))
            {
                return false;
            }

            if (!FormValidationUtility.RequiredString(name, "Name", out errorMessage))
            {
                return false;
            }

            if (!FormValidationUtility.RequiredType(typeName, out errorMessage))
            {
                return false;
            }

            IPayload payload = null;
            var payloadProp = _dataProperty.FindPropertyRelative(TriggerDefinitionPropertyNames.Payload);

            if (payloadProp != null)
            {
                try
                {
                    payload = payloadProp.managedReferenceValue as IPayload;
                }
                catch
                {
                    payload = null;
                }
            }

            if (!FormValidationUtility.ValidatePayload(payload, out errorMessage))
            {
                return false;
            }

            return true;
        }

        protected override Result GatherResult()
        {
            var id = _dataProperty.FindPropertyRelative(TriggerDefinitionPropertyNames.Id).stringValue?.Trim() ??
                     string.Empty;
            var name = _dataProperty.FindPropertyRelative(TriggerDefinitionPropertyNames.Name).stringValue?.Trim() ??
                       string.Empty;
            var description =
                _dataProperty.FindPropertyRelative(TriggerDefinitionPropertyNames.Description).stringValue?.Trim() ??
                string.Empty;
            var typeName =
                _dataProperty.FindPropertyRelative(TriggerDefinitionPropertyNames.TypeName).stringValue?.Trim() ??
                string.Empty;

            IPayload payload = null;
            var payloadProp = _dataProperty.FindPropertyRelative(TriggerDefinitionPropertyNames.Payload);

            if (payloadProp != null)
            {
                try
                {
                    payload = payloadProp.managedReferenceValue as IPayload;
                }
                catch
                {
                    payload = null;
                }
            }

            return new Result(id, name, description, typeName, payload);
        }

        internal class TriggerDefinitionWrapper : DefinitionWrapper<TriggerDefinition>
        {
        }

        public sealed class Result
        {
            public string Id { get; }
            public string Name { get; }
            public string Description { get; }
            public string TypeName { get; }
            public IPayload Payload { get; }

            public Result(string id, string name, string description, string typeName, IPayload payload)
            {
                Id = id;
                Name = name;
                Description = description;
                TypeName = typeName;
                Payload = payload;
            }
        }

        private enum Mode
        {
            Undefined = 0,
            Create = 1,
            Edit = 2
        }
    }
}