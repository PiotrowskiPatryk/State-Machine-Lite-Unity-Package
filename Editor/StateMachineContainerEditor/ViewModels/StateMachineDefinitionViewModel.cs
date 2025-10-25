using System;
using System.Collections.Generic;
using Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.Data.Definition;
using Unity.Properties;
using UnityEditor;

namespace Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.ViewModels
{
    public sealed class StateMachineDefinitionViewModel : ViewModelBase
    {
        [CreateProperty]
        public string StateMachineId
        {
            get => SerializedProperty.FindPropertyRelative(StateMachineDefinition.ID_PROPERTY_NAME).stringValue;
            set => ApplyPropertyValueString(StateMachineDefinition.ID_PROPERTY_NAME, value);
        }

        [CreateProperty]
        public string StateMachineName
        {
            get => SerializedProperty.
                FindPropertyRelative(StateMachineDefinition.NAME_PROPERTY_NAME).stringValue;
            set => ApplyPropertyValueString(StateMachineDefinition.NAME_PROPERTY_NAME, value);
        }

        [CreateProperty]
        public string StateMachineDescription
        {
            get => SerializedProperty.
                FindPropertyRelative(StateMachineDefinition.DESCRIPTION_PROPERTY_NAME).stringValue;
            set => ApplyPropertyValueString(StateMachineDefinition.DESCRIPTION_PROPERTY_NAME, value);
        }

        [CreateProperty]
        public string StateMachineType
        {
            get => SerializedProperty.
                FindPropertyRelative(StateMachineDefinition.STATE_MACHINE_TYPE_PROPERTY_NAME).stringValue;
            set => ApplyPropertyValueString(StateMachineDefinition.STATE_MACHINE_TYPE_PROPERTY_NAME, value);
        }

        [CreateProperty]
        public string TransitionSolverType
        {
            get => SerializedProperty.
                FindPropertyRelative(StateMachineDefinition.TRANSITION_SOLVER_TYPE_PROPERTY_NAME).stringValue;
            set => ApplyPropertyValueString(StateMachineDefinition.TRANSITION_SOLVER_TYPE_PROPERTY_NAME, value);
        }

        [CreateProperty]
        public string StateMachinePayloadTypeName
        {
            get => SerializedProperty.
                FindPropertyRelative(StateMachineDefinition.PAYLOAD_PROPERTY_NAME).stringValue;
            set => ApplyPropertyValueString(StateMachineDefinition.PAYLOAD_PROPERTY_NAME, value);
        }

        [CreateProperty]
        public List<StateDefinitionViewModel> States
        {
            get
            {
                var states = new List<StateDefinitionViewModel>();
                var statesProperty =
                    SerializedProperty.FindPropertyRelative(StateMachineDefinition.STATES_PROPERTY_NAME);

                for (var i = 0; i < statesProperty.arraySize; ++i)
                {
                    var state = statesProperty.GetArrayElementAtIndex(i);
                    states.Add(new StateDefinitionViewModel(state));
                }

                return states;
            }
        }

        protected override SerializedProperty SerializedProperty { get; }

        public StateMachineDefinitionViewModel(SerializedProperty serializedProperty)
        {
            SerializedProperty = serializedProperty;
        }

        public void CreateNewState()
        {
            var statesProperty = SerializedProperty.FindPropertyRelative(StateMachineDefinition.STATES_PROPERTY_NAME);

            Undo.RecordObject(statesProperty.serializedObject.targetObject, "Create new state");

            statesProperty.serializedObject.Update();

            statesProperty.InsertArrayElementAtIndex(statesProperty.arraySize);
            statesProperty.serializedObject.ApplyModifiedProperties();

            var newStateProperty = statesProperty.GetArrayElementAtIndex(statesProperty.arraySize - 1);

            var stateDefinitionViewModel = new StateDefinitionViewModel(newStateProperty)
            {
                Id = Guid.NewGuid().ToString(),
                StateName = "New State"
            };

            statesProperty.serializedObject.ApplyModifiedProperties();
        }

        public void RemoveState(StateDefinitionViewModel state)
        {
        }
    }
}