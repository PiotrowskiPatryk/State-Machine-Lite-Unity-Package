using System;
using System.Collections.Generic;
using Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.Data;
using Unity.Properties;
using UnityEditor;

namespace Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.ViewModels
{
    public sealed class TriggerConfigurationViewModel : ViewModelBase
    {
        [CreateProperty]
        public int TriggersCount =>
            SerializedProperty.FindPropertyRelative(TriggerConfiguration.TRIGGERS_PROPERTY_NAME).arraySize;

        [CreateProperty]
        public List<TriggerDefinitionViewModel> Triggers
        {
            get
            {
                var triggers = new List<TriggerDefinitionViewModel>();

                for (var i = 0; i < TriggersCount; i++)
                {
                    triggers.Add(GetTriggerByIndex(i));
                }

                return triggers;
            }
        }

        protected override SerializedProperty SerializedProperty { get; }

        public TriggerConfigurationViewModel(SerializedProperty serializedProperty)
        {
            SerializedProperty = serializedProperty;
        }

        public TriggerDefinitionViewModel GetTriggerByIndex(int index)
        {
            if (TriggersCount <= index)
            {
                return null;
            }

            return new TriggerDefinitionViewModel(
                SerializedProperty.FindPropertyRelative(TriggerConfiguration.TRIGGERS_PROPERTY_NAME).
                    GetArrayElementAtIndex(index));
        }

        public void AddTrigger()
        {
            var triggersProperty =
                SerializedProperty.FindPropertyRelative(TriggerConfiguration.TRIGGERS_PROPERTY_NAME);

            Undo.RecordObject(triggersProperty.serializedObject.targetObject, "Create new trigger");

            triggersProperty.serializedObject.Update();
            triggersProperty.InsertArrayElementAtIndex(triggersProperty.arraySize);
            triggersProperty.serializedObject.ApplyModifiedProperties();

            var newStateMachineProperty =
                triggersProperty.GetArrayElementAtIndex(triggersProperty.arraySize - 1);

            var triggerDefinitionViewModel = new TriggerDefinitionViewModel(newStateMachineProperty)
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = "New Trigger"
            };

            triggersProperty.serializedObject.ApplyModifiedProperties();
            Notify(nameof(Triggers));
            Notify(nameof(TriggersCount));
        }
    }
}