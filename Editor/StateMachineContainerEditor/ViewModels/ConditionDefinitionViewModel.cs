using System;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.Data.Definition;
using Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.Utilities;
using Unity.Properties;
using UnityEditor;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.ViewModels
{
    public class ConditionDefinitionViewModel : ViewModelBase
    {
        private readonly SerializedInstanceSwitcher<IPayload> _payloadSwitcher;

        [CreateProperty]
        public string Id
        {
            get => SerializedProperty.FindPropertyRelative(ConditionDefinition.ID_PROPERTY_NAME).stringValue;
            set => ApplyPropertyValueString(ConditionDefinition.ID_PROPERTY_NAME, value);
        }

        [CreateProperty]
        public string Name
        {
            get => SerializedProperty.FindPropertyRelative(ConditionDefinition.NAME_PROPERTY_NAME).stringValue;
            set => ApplyPropertyValueString(ConditionDefinition.NAME_PROPERTY_NAME, value);
        }

        [CreateProperty]
        public string Description
        {
            get => SerializedProperty.FindPropertyRelative(ConditionDefinition.DESCRIPTION_PROPERTY_NAME).stringValue;
            set => ApplyPropertyValueString(ConditionDefinition.DESCRIPTION_PROPERTY_NAME, value);
        }

        [CreateProperty]
        public SerializedProperty Payload =>
            SerializedProperty.FindPropertyRelative(ConditionDefinition.PAYLOAD_PROPERTY_NAME);

        [CreateProperty]
        public string TypeName
        {
            get => SerializedProperty.FindPropertyRelative(ConditionDefinition.TYPE_NAME_PROPERTY_NAME).stringValue;
            set
            {
                if (TypeName.Equals(value))
                {
                    return;
                }

                var conditionPayload = StateMachineReflectionUtilities.GetConditionPayloadType(value);

                ApplyPropertyValueString(ConditionDefinition.TYPE_NAME_PROPERTY_NAME, value);

                _payloadSwitcher.SwitchTo(conditionPayload);
            }
        }

        [CreateProperty]
        public string TypeNameShort => StateMachineReflectionUtilities.ToClassNameOnly(TypeName);

        public override SerializedProperty SerializedProperty { get; }

        public ConditionDefinitionViewModel()
        {
            // TODO - Handle disposal
            var definitionWrapper = ScriptableObject.CreateInstance<ConditionDefinitionWrapper>();
            SerializedProperty =
                new SerializedObject(definitionWrapper).FindProperty(ConditionDefinitionWrapper.DATA_PROPERTY_NAME);
            Id = Guid.NewGuid().ToString("N");
            _payloadSwitcher = new SerializedInstanceSwitcher<IPayload>(Payload);
        }

        public ConditionDefinitionViewModel(SerializedProperty serializedProperty)
        {
            SerializedProperty = serializedProperty;
            _payloadSwitcher = new SerializedInstanceSwitcher<IPayload>(Payload);
        }

        public void CopyFrom(ConditionDefinitionViewModel other)
        {
            Id = other.Id;
            Name = other.Name;
            Description = other.Description;
            TypeName = other.TypeName;
            Payload.managedReferenceValue = other.Payload.managedReferenceValue;
        }

        internal class ConditionDefinitionWrapper : DefinitionWrapper<ConditionDefinition>
        {
        }
    }
}