using System;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Core.StateMachineConfiguration.Definition;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Utilities;
using Unity.Properties;
using UnityEditor;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.ViewModels
{
    public sealed class TriggerDefinitionViewModel : ViewModelBase
    {
        private readonly SerializedInstanceSwitcher<IPayload> _payloadSwitcher;

        public bool IsValid => ((TriggerDefinition)SerializedProperty.boxedValue).IsValid();

        [CreateProperty]
        public string Id
        {
            get => SerializedProperty.FindPropertyRelative(TriggerDefinition.ID_PROPERTY_NAME).stringValue;
            set => ApplyPropertyValueString(TriggerDefinition.ID_PROPERTY_NAME, value);
        }

        [CreateProperty]
        public string Name
        {
            get => SerializedProperty.FindPropertyRelative(TriggerDefinition.NAME_PROPERTY_NAME).stringValue;
            set => ApplyPropertyValueString(TriggerDefinition.NAME_PROPERTY_NAME, value);
        }

        [CreateProperty]
        public string Description
        {
            get => SerializedProperty.FindPropertyRelative(TriggerDefinition.DESCRIPTION_PROPERTY_NAME).stringValue;
            set => ApplyPropertyValueString(TriggerDefinition.DESCRIPTION_PROPERTY_NAME, value);
        }

        [CreateProperty]
        public string TypeName
        {
            get => SerializedProperty.FindPropertyRelative(TriggerDefinition.TYPE_NAME_PROPERTY_NAME).stringValue;
            set
            {
                if (TypeName.Equals(value))
                {
                    return;
                }

                var triggerPayloadType = StateMachineReflectionUtilities.GetTriggerPayloadType(value);

                ApplyPropertyValueString(TriggerDefinition.TYPE_NAME_PROPERTY_NAME, value);

                // Store the script GUID for resilient type tracking
                var type = Type.GetType(value);

                if (type != null)
                {
                    var guid = ScriptGuidResolver.GetGuidForType(type);
                    ApplyPropertyValueString(TriggerDefinition.SCRIPT_GUID_PROPERTY_NAME, guid);
                }

                _payloadSwitcher.SwitchTo(triggerPayloadType);

                // Store the payload GUID for resilient payload type tracking
                if (triggerPayloadType != null)
                {
                    var payloadGuid = ScriptGuidResolver.GetGuidForType(triggerPayloadType);
                    ApplyPropertyValueString(TriggerDefinition.PAYLOAD_SCRIPT_GUID_PROPERTY_NAME, payloadGuid);
                }
            }
        }

        [CreateProperty]
        public SerializedProperty Payload =>
            SerializedProperty.FindPropertyRelative(TriggerDefinition.PAYLOAD_PROPERTY_NAME);

        public override SerializedProperty SerializedProperty { get; }

        public TriggerDefinitionViewModel(SerializedProperty serializedProperty)
        {
            SerializedProperty = serializedProperty;
            _payloadSwitcher = new SerializedInstanceSwitcher<IPayload>(Payload);
        }

        public TriggerDefinitionViewModel()
        {
            // NOTE: Call Dispose() when done to clean up the ScriptableObject wrapper
            var definitionWrapper = ScriptableObject.CreateInstance<TriggerDefinitionEditorWrapper>();
            SerializedProperty =
                new SerializedObject(definitionWrapper).FindProperty(TriggerDefinitionEditorWrapper.
                    DATA_PROPERTY_NAME);
            Id = Guid.NewGuid().ToString("N");
            _payloadSwitcher = new SerializedInstanceSwitcher<IPayload>(Payload);
        }

        public TriggerDefinitionViewModel(TriggerDefinitionViewModel triggerDefinitionViewModel)
        {
            SerializedProperty = triggerDefinitionViewModel.SerializedProperty.Copy();
            _payloadSwitcher = new SerializedInstanceSwitcher<IPayload>(Payload);
        }

        public void CopyFrom(TriggerDefinitionViewModel other)
        {
            Id = other.Id;
            Name = other.Name;
            Description = other.Description;
            TypeName = other.TypeName;
            Payload.managedReferenceValue = other.Payload.managedReferenceValue;
        }
    }
}