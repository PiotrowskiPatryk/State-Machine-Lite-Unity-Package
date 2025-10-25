using System;
using System.Collections.Generic;
using Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.Data;
using Unity.Properties;
using UnityEditor;

namespace Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.ViewModels
{
    public sealed class StateMachineConfigurationViewModel : ViewModelBase
    {
        public event Action<StateMachineDefinitionViewModel> OnStateMachineAdded;

        [CreateProperty]
        public int StateMachinesCount => SerializedProperty.
            FindPropertyRelative(StateMachineConfiguration.STATE_MACHINES_PROPERTY_NAME).arraySize;

        [CreateProperty]
        public List<StateMachineDefinitionViewModel> StateMachines
        {
            get
            {
                var stateMachines = new List<StateMachineDefinitionViewModel>();

                for (var i = 0; i < StateMachinesCount; i++)
                {
                    stateMachines.Add(GetStateMachineByIndex(i));
                }

                return stateMachines;
            }
        }

        protected override SerializedProperty SerializedProperty { get; }

        public StateMachineConfigurationViewModel(SerializedProperty serializedProperty)
        {
            SerializedProperty = serializedProperty;
        }

        public StateMachineDefinitionViewModel GetStateMachineByIndex(int index)
        {
            if (StateMachinesCount <= index)
            {
                return null;
            }

            return new StateMachineDefinitionViewModel(
                SerializedProperty.FindPropertyRelative(StateMachineConfiguration.STATE_MACHINES_PROPERTY_NAME).
                    GetArrayElementAtIndex(index));
        }

        public void AddStateMachine()
        {
            var stateMachinesProperty =
                SerializedProperty.FindPropertyRelative(StateMachineConfiguration.STATE_MACHINES_PROPERTY_NAME);

            Undo.RecordObject(stateMachinesProperty.serializedObject.targetObject, "Create new state machine");

            stateMachinesProperty.serializedObject.Update();
            stateMachinesProperty.InsertArrayElementAtIndex(stateMachinesProperty.arraySize);
            stateMachinesProperty.serializedObject.ApplyModifiedProperties();

            var newStateMachineProperty =
                stateMachinesProperty.GetArrayElementAtIndex(stateMachinesProperty.arraySize - 1);

            var stateMachineDefinitionViewModel = new StateMachineDefinitionViewModel(newStateMachineProperty)
            {
                StateMachineId = Guid.NewGuid().ToString("N"),
                StateMachineName = "New State Machine"
            };

            stateMachinesProperty.serializedObject.ApplyModifiedProperties();

            OnStateMachineAdded?.Invoke(stateMachineDefinitionViewModel);

            Notify(nameof(StateMachines));
            Notify(nameof(StateMachinesCount));
        }

        public void RemoveStateMachine(int index)
        {
        }
    }
}