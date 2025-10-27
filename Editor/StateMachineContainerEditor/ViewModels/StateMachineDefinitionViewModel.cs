using System;
using System.Collections.Generic;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.Data.Definition;
using Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.Utilities;
using Unity.Properties;
using UnityEditor;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.ViewModels
{
    public sealed class StateMachineDefinitionViewModel : ViewModelBase
    {
        private readonly SerializedInstanceSwitcher<IPayload> _payloadSwitcher;

        [CreateProperty]
        public string Id
        {
            get => SerializedProperty.FindPropertyRelative(StateMachineDefinition.ID_PROPERTY_NAME).stringValue;
            set => ApplyPropertyValueString(StateMachineDefinition.ID_PROPERTY_NAME, value);
        }

        [CreateProperty]
        public string Name
        {
            get => SerializedProperty.
                FindPropertyRelative(StateMachineDefinition.NAME_PROPERTY_NAME).stringValue;
            set => ApplyPropertyValueString(StateMachineDefinition.NAME_PROPERTY_NAME, value);
        }

        [CreateProperty]
        public string Description
        {
            get => SerializedProperty.
                FindPropertyRelative(StateMachineDefinition.DESCRIPTION_PROPERTY_NAME).stringValue;
            set => ApplyPropertyValueString(StateMachineDefinition.DESCRIPTION_PROPERTY_NAME, value);
        }

        [CreateProperty]
        public string TypeName
        {
            get => SerializedProperty.
                FindPropertyRelative(StateMachineDefinition.STATE_MACHINE_TYPE_PROPERTY_NAME).stringValue;
            set
            {
                if (TypeName.Equals(value))
                {
                    return;
                }

                var stateMachinePayload = StateMachineReflectionUtilities.GetStateMachinePayloadType(value);

                ApplyPropertyValueString(StateMachineDefinition.STATE_MACHINE_TYPE_PROPERTY_NAME, value);

                _payloadSwitcher.SwitchTo(stateMachinePayload);
            }
        }

        [CreateProperty]
        public string TransitionSolverTypeName
        {
            get => SerializedProperty.
                FindPropertyRelative(StateMachineDefinition.TRANSITION_SOLVER_TYPE_PROPERTY_NAME).stringValue;
            set => ApplyPropertyValueString(StateMachineDefinition.TRANSITION_SOLVER_TYPE_PROPERTY_NAME, value);
        }

        [CreateProperty]
        public SerializedProperty Payload =>
            SerializedProperty.FindPropertyRelative(StateMachineDefinition.PAYLOAD_PROPERTY_NAME);

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

        public StateMachineDefinitionViewModel()
        {
            // TODO - Handle disposal
            var definitionWrapper = ScriptableObject.CreateInstance<StateMachineDefinitionWrapper>();
            SerializedProperty =
                new SerializedObject(definitionWrapper).FindProperty(StateMachineDefinitionWrapper.DATA_PROPERTY_NAME);
            Id = Guid.NewGuid().ToString("N");
            _payloadSwitcher = new SerializedInstanceSwitcher<IPayload>(Payload);
        }

        public StateMachineDefinitionViewModel(SerializedProperty serializedProperty)
        {
            SerializedProperty = serializedProperty;
            _payloadSwitcher = new SerializedInstanceSwitcher<IPayload>(Payload);
        }

        public void CopyFrom(StateMachineDefinitionViewModel other)
        {
            Id = other.Id;
            Name = other.Name;
            Description = other.Description;
            TypeName = other.TypeName;
            TransitionSolverTypeName = other.TransitionSolverTypeName;
            Payload.managedReferenceValue = other.Payload.managedReferenceValue;
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

        internal class StateMachineDefinitionWrapper : DefinitionWrapper<StateMachineDefinition>
        {
        }
    }
}