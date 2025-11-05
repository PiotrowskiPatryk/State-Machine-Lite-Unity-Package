using System.Collections.Generic;
using System.Linq;
using Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.Data;
using Unity.Properties;
using UnityEditor;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.ViewModels
{
    public sealed class StateMachineConfigurationViewModel : ViewModelBase
    {
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

        public override SerializedProperty SerializedProperty { get; }

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

        public void AddStateMachine(StateMachineDefinitionViewModel stateMachineDefinitionViewModel)
        {
            var stateMachinesProperty =
                SerializedProperty.FindPropertyRelative(StateMachineConfiguration.STATE_MACHINES_PROPERTY_NAME);

            Undo.RecordObject(stateMachinesProperty.serializedObject.targetObject, "Create new state machine");

            var so = stateMachinesProperty.serializedObject;
            so.Update();

            stateMachinesProperty.InsertArrayElementAtIndex(stateMachinesProperty.arraySize);

            so.ApplyModifiedProperties();

            var newStateMachineProperty =
                stateMachinesProperty.GetArrayElementAtIndex(stateMachinesProperty.arraySize - 1);

            var newStateMachine = new StateMachineDefinitionViewModel(newStateMachineProperty);
            newStateMachine.CopyFrom(stateMachineDefinitionViewModel);

            so.ApplyModifiedProperties();

            Notify(nameof(StateMachines));
            Notify(nameof(StateMachinesCount));
        }

        public void RemoveStateMachineAtIndex(int index)
        {
            if (index < 0 || index >= StateMachinesCount)
            {
                Debug.LogError($"Invalid index {index} for state machine count {StateMachinesCount}");

                return;
            }

            var stateMachinesProperty =
                SerializedProperty.FindPropertyRelative(StateMachineConfiguration.STATE_MACHINES_PROPERTY_NAME);

            Undo.RecordObject(stateMachinesProperty.serializedObject.targetObject, "Remove state machine");

            stateMachinesProperty.serializedObject.Update();
            stateMachinesProperty.DeleteArrayElementAtIndex(index);
            stateMachinesProperty.serializedObject.ApplyModifiedProperties();

            Notify(nameof(StateMachines));
            Notify(nameof(StateMachinesCount));
        }

        public void UpdateState(StateMachineDefinitionViewModel stateMachineDefinitionViewModel)
        {
            var oldStateMachine = StateMachines.FirstOrDefault(stateMachine =>
                stateMachine.Id.Equals(stateMachineDefinitionViewModel.Id));

            oldStateMachine?.CopyFrom(stateMachineDefinitionViewModel);

            Notify(nameof(StateMachines));
            Notify(nameof(StateMachinesCount));
        }
    }
}