using Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.Data.Definition;
using Unity.Properties;
using UnityEditor;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.ViewModels
{
    public sealed class StateDefinitionViewModel : ViewModelBase
    {
        [CreateProperty]
        public string Id
        {
            get => SerializedProperty.FindPropertyRelative(StateDefinition.ID_PROPERTY_NAME).stringValue;
            set => ApplyPropertyValueString(StateDefinition.ID_PROPERTY_NAME, value);
        }

        [CreateProperty]
        public string StateName
        {
            get => SerializedProperty.FindPropertyRelative(StateDefinition.NAME_PROPERTY_NAME).stringValue;
            set => ApplyPropertyValueString(StateDefinition.NAME_PROPERTY_NAME, value);
        }

        [CreateProperty]
        public string StateDescription
        {
            get =>
                SerializedProperty.
                    FindPropertyRelative(StateDefinition.DESCRIPTION_PROPERTY_NAME).stringValue;
            set => ApplyPropertyValueString(StateDefinition.DESCRIPTION_PROPERTY_NAME, value);
        }

        [CreateProperty]
        public string StatePayloadTypeName
        {
            get => SerializedProperty.FindPropertyRelative(StateDefinition.PAYLOAD_PROPERTY_NAME).stringValue;
            set => ApplyPropertyValueString(StateDefinition.PAYLOAD_PROPERTY_NAME, value);
        }

        [CreateProperty]
        public string StateTypeName
        {
            get => SerializedProperty.FindPropertyRelative(StateDefinition.TYPE_NAME_PROPERTY_NAME).stringValue;
            set => ApplyPropertyValueString(StateDefinition.TYPE_NAME_PROPERTY_NAME, value);
        }

        [CreateProperty]
        public Vector2Int StatePosition
        {
            get =>
                SerializedProperty.
                    FindPropertyRelative(StateDefinition.NODE_POSITION_PROPERTY_NAME).vector2IntValue;
            set => ApplyPropertyValueVector2Int(StateDefinition.NODE_POSITION_PROPERTY_NAME, value);
        }

        protected override SerializedProperty SerializedProperty { get; }

        public StateDefinitionViewModel(SerializedProperty serializedProperty)
        {
            SerializedProperty = serializedProperty;
        }
    }
}