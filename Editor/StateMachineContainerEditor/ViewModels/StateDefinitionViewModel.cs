using System;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.Data.Definition;
using Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.Utilities;
using Unity.Properties;
using UnityEditor;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.ViewModels
{
    public sealed class StateDefinitionViewModel : ViewModelBase
    {
        private readonly SerializedInstanceSwitcher<IPayload> _payloadSwitcher;

        [CreateProperty]
        public string Id
        {
            get => SerializedProperty.FindPropertyRelative(StateDefinition.ID_PROPERTY_NAME).stringValue;
            set => ApplyPropertyValueString(StateDefinition.ID_PROPERTY_NAME, value);
        }

        [CreateProperty]
        public string Name
        {
            get => SerializedProperty.FindPropertyRelative(StateDefinition.NAME_PROPERTY_NAME).stringValue;
            set => ApplyPropertyValueString(StateDefinition.NAME_PROPERTY_NAME, value);
        }

        [CreateProperty]
        public string Description
        {
            get =>
                SerializedProperty.
                    FindPropertyRelative(StateDefinition.DESCRIPTION_PROPERTY_NAME).stringValue;
            set => ApplyPropertyValueString(StateDefinition.DESCRIPTION_PROPERTY_NAME, value);
        }

        [CreateProperty]
        public SerializedProperty Payload =>
            SerializedProperty.FindPropertyRelative(StateDefinition.PAYLOAD_PROPERTY_NAME);

        [CreateProperty]
        public string TypeName
        {
            get => SerializedProperty.FindPropertyRelative(StateDefinition.TYPE_NAME_PROPERTY_NAME).stringValue;
            set => ApplyPropertyValueString(StateDefinition.TYPE_NAME_PROPERTY_NAME, value);
        }

        [CreateProperty]
        public string StateMachineTypeName
        {
            get => SerializedProperty.FindPropertyRelative(StateDefinition.STATE_MACHINE_TYPE_NAME_PROPERTY_NAME).
                stringValue;
            set => ApplyPropertyValueString(StateDefinition.STATE_MACHINE_TYPE_NAME_PROPERTY_NAME, value);
        }

        [CreateProperty]
        public string TypeNameShort => StateMachineReflectionUtilities.ToClassNameOnly(TypeName);

        [CreateProperty]
        public Vector2Int NodePosition
        {
            get =>
                SerializedProperty.
                    FindPropertyRelative(StateDefinition.NODE_POSITION_PROPERTY_NAME).vector2IntValue;
            set => ApplyPropertyValueVector2Int(StateDefinition.NODE_POSITION_PROPERTY_NAME, value);
        }

        public override SerializedProperty SerializedProperty { get; }

        public StateDefinitionViewModel(SerializedProperty serializedProperty)
        {
            SerializedProperty = serializedProperty;
        }

        public StateDefinitionViewModel(string stateMachineType)
        {
            // TODO - Handle disposal
            var definitionWrapper = ScriptableObject.CreateInstance<StateDefinitionWrapper>();
            SerializedProperty =
                new SerializedObject(definitionWrapper).FindProperty(StateDefinitionWrapper.DATA_PROPERTY_NAME);
            Id = Guid.NewGuid().ToString("N");
            _payloadSwitcher = new SerializedInstanceSwitcher<IPayload>(Payload);
            StateMachineTypeName = stateMachineType;
        }

        public void CopyFrom(StateDefinitionViewModel other)
        {
            Id = other.Id;
            Name = other.Name;
            Description = other.Description;
            TypeName = other.TypeName;
            StateMachineTypeName = other.StateMachineTypeName;
            NodePosition = other.NodePosition;
        }

        internal class StateDefinitionWrapper : DefinitionWrapper<StateDefinition>
        {
        }
    }
}