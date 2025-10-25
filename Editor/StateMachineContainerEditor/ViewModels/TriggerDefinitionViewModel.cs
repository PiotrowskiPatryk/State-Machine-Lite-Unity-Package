using Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.Data.Definition;
using Unity.Properties;
using UnityEditor;

namespace Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.ViewModels
{
    public sealed class TriggerDefinitionViewModel : ViewModelBase
    {
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
            set => ApplyPropertyValueString(TriggerDefinition.TYPE_NAME_PROPERTY_NAME, value);
        }

        protected override SerializedProperty SerializedProperty { get; }

        public TriggerDefinitionViewModel(SerializedProperty serializedProperty)
        {
            SerializedProperty = serializedProperty;
        }
    }
}