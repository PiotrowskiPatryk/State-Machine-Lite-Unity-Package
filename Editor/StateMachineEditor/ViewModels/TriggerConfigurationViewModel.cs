using System.Collections.Generic;
using System.Linq;
using Dev.Cortez.StateMachines.Core.StateMachineConfiguration;
using Unity.Properties;
using UnityEditor;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.ViewModels
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

        public override SerializedProperty SerializedProperty { get; }

        public TriggerConfigurationViewModel(SerializedProperty serializedProperty)
        {
            SerializedProperty = serializedProperty;
        }

        public TriggerDefinitionViewModel GetTriggerByIndex(int index)
        {
            if (index < 0 || TriggersCount <= index)
            {
                return null;
            }

            return new TriggerDefinitionViewModel(
                SerializedProperty.FindPropertyRelative(TriggerConfiguration.TRIGGERS_PROPERTY_NAME).
                    GetArrayElementAtIndex(index));
        }

        public void UpdateTrigger(TriggerDefinitionViewModel updatedTrigger)
        {
            var oldTrigger = Triggers.FirstOrDefault(trigger => trigger.Id.Equals(updatedTrigger.Id));

            oldTrigger?.CopyFrom(updatedTrigger);

            Notify(nameof(Triggers));
            Notify(nameof(TriggersCount));
        }

        public void AddTrigger(TriggerDefinitionViewModel triggerDefinitionViewModel)
        {
            var triggersProperty = SerializedProperty.FindPropertyRelative(TriggerConfiguration.TRIGGERS_PROPERTY_NAME);

            Undo.RecordObject(triggersProperty.serializedObject.targetObject, "Create new trigger");

            var so = triggersProperty.serializedObject;
            so.Update();

            // 1) Structural change
            triggersProperty.InsertArrayElementAtIndex(triggersProperty.arraySize);

            // 2) Commit structure and 3) immediately reacquire
            so.ApplyModifiedProperties();

            // 4) Reacquire the array and the new element after the structural Apply
            triggersProperty = SerializedProperty.FindPropertyRelative(TriggerConfiguration.TRIGGERS_PROPERTY_NAME);
            var newTriggerProperty = triggersProperty.GetArrayElementAtIndex(triggersProperty.arraySize - 1);

            // 5) Now it’s safe to build a VM and copy
            var newTrigger = new TriggerDefinitionViewModel(newTriggerProperty);
            newTrigger.CopyFrom(triggerDefinitionViewModel);

            // Final apply if CopyFrom didn’t already (your CopyFrom does apply once)
            so.ApplyModifiedProperties();

            Notify(nameof(Triggers));
            Notify(nameof(TriggersCount));
        }

        public void RemoveTriggerAtIndex(int index)
        {
            if (index < 0 || index >= TriggersCount)
            {
                Debug.LogError($"Invalid index {index} for trigger count {TriggersCount}");

                return;
            }

            var triggersProperty =
                SerializedProperty.FindPropertyRelative(TriggerConfiguration.TRIGGERS_PROPERTY_NAME);

            Undo.RecordObject(triggersProperty.serializedObject.targetObject, "Remove trigger");

            triggersProperty.serializedObject.Update();
            triggersProperty.DeleteArrayElementAtIndex(index);
            triggersProperty.serializedObject.ApplyModifiedProperties();

            Notify(nameof(Triggers));
            Notify(nameof(TriggersCount));
        }
    }
}