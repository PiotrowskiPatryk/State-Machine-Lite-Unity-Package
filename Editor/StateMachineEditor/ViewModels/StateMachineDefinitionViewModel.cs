using System;
using System.Collections.Generic;
using System.Linq;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Core.StateMachineConfiguration.Definition;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Utilities;
using Unity.Properties;
using UnityEditor;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.ViewModels
{
    public sealed class StateMachineDefinitionViewModel : ViewModelBase
    {
        private readonly SerializedInstanceSwitcher<IPayload> _payloadSwitcher;

        [CreateProperty]
        public bool IsValid => ((StateMachineDefinition)SerializedProperty.boxedValue).IsValid();

        [CreateProperty]
        public int StatesCount =>
            SerializedProperty.FindPropertyRelative(StateMachineDefinition.STATES_PROPERTY_NAME).arraySize;

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

                ApplyTypeWithGuid(
                    value,
                    StateMachineDefinition.STATE_MACHINE_TYPE_PROPERTY_NAME,
                    StateMachineDefinition.STATE_MACHINE_SCRIPT_GUID_PROPERTY_NAME);

                foreach (var state in States)
                {
                    state.StateMachineTypeName = value;
                }

                _payloadSwitcher.SwitchTo(stateMachinePayload);

                // Store the payload GUID for resilient payload type tracking
                ApplyGuidForType(stateMachinePayload, StateMachineDefinition.PAYLOAD_SCRIPT_GUID_PROPERTY_NAME);
            }
        }

        [CreateProperty]
        public string TypeNameShort => StateMachineReflectionUtilities.ToClassNameOnly(TypeName);

        [CreateProperty]
        public string TransitionSolverTypeName
        {
            get => SerializedProperty.
                FindPropertyRelative(StateMachineDefinition.TRANSITION_SOLVER_TYPE_PROPERTY_NAME).stringValue;
            set => ApplyTypeWithGuid(
                value,
                StateMachineDefinition.TRANSITION_SOLVER_TYPE_PROPERTY_NAME,
                StateMachineDefinition.TRANSITION_SOLVER_SCRIPT_GUID_PROPERTY_NAME);
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
            set
            {
                var statesProperty =
                    SerializedProperty.FindPropertyRelative(StateMachineDefinition.STATES_PROPERTY_NAME);

                var so = statesProperty.serializedObject;
                so.Update();

                statesProperty.ClearArray();
                so.ApplyModifiedProperties();

                if (value != null)
                {
                    for (var i = 0; i < value.Count; ++i)
                    {
                        statesProperty.InsertArrayElementAtIndex(i);
                        var elementProperty = statesProperty.GetArrayElementAtIndex(i);
                        var elementViewModel = new StateDefinitionViewModel(elementProperty);
                        elementViewModel.CopyFrom(value[i]);
                    }
                }

                so.ApplyModifiedProperties();

                Notify();
                Notify(nameof(StatesCount));
                Notify(nameof(AvailableStates));
                Notify(nameof(InitialState));
                Notify(nameof(InitialStateIndex));
            }
        }

        [CreateProperty]
        public StateDefinitionViewModel InitialState
        {
            get
            {
                var initialStateProperty =
                    SerializedProperty.FindPropertyRelative(StateMachineDefinition.INITIAL_STATE_PROPERTY_NAME);

                return new StateDefinitionViewModel(initialStateProperty);
            }
            set
            {
                InitialState.CopyFrom(value);
                Notify();
            }
        }

        [CreateProperty]
        public List<string> AvailableStates => States.Select(state => state.Name).ToList();

        [CreateProperty]
        public int InitialStateIndex
        {
            get
            {
                if (InitialState == null)
                {
                    return -1;
                }

                return States.FindIndex(state => state.Id.Equals(InitialState.Id));
            }

            set
            {
                InitialState = States[value];
                Notify();
            }
        }

        public SerializedProperty StatesSerializedProperty =>
            SerializedProperty.FindPropertyRelative(StateMachineDefinition.STATES_PROPERTY_NAME);

        public override SerializedProperty SerializedProperty { get; }

        public StateMachineDefinitionViewModel()
        {
            // NOTE: Call Dispose() when done to clean up the ScriptableObject wrapper
            var definitionWrapper = ScriptableObject.CreateInstance<StateMachineDefinitionEditorWrapper>();
            SerializedProperty =
                new SerializedObject(definitionWrapper).FindProperty(StateMachineDefinitionEditorWrapper.
                    DATA_PROPERTY_NAME);
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
            States = other.States;
            InitialState = other.InitialState;
        }

        public void AddState(StateDefinitionViewModel stateDefinitionViewModel)
        {
            var statesProperty = SerializedProperty.FindPropertyRelative(StateMachineDefinition.STATES_PROPERTY_NAME);

            Undo.RecordObject(statesProperty.serializedObject.targetObject, "Create new state");

            var so = statesProperty.serializedObject;
            so.Update();

            statesProperty.InsertArrayElementAtIndex(statesProperty.arraySize);
            so.ApplyModifiedProperties();

            var newStateProperty = statesProperty.GetArrayElementAtIndex(statesProperty.arraySize - 1);
            var newState = new StateDefinitionViewModel(newStateProperty);
            newState.CopyFrom(stateDefinitionViewModel);

            so.ApplyModifiedProperties();

            Notify(nameof(States));
            Notify(nameof(StatesCount));
            Notify(nameof(AvailableStates));
        }

        public void UpdateState(StateDefinitionViewModel updatedState)
        {
            var oldState = States.FirstOrDefault(trigger => trigger.Id.Equals(updatedState.Id));

            oldState?.CopyFrom(updatedState);

            Notify(nameof(States));
        }

        public void RemoveStateAtIndex(int index)
        {
            if (index < 0 || index >= StatesCount)
            {
                Debug.LogError($"Invalid index {index} for states count {StatesCount}");

                return;
            }

            var statesProperty =
                SerializedProperty.FindPropertyRelative(StateMachineDefinition.STATES_PROPERTY_NAME);

            Undo.RecordObject(statesProperty.serializedObject.targetObject, "Remove state");

            statesProperty.serializedObject.Update();
            statesProperty.DeleteArrayElementAtIndex(index);
            statesProperty.serializedObject.ApplyModifiedProperties();

            Notify(nameof(States));
            Notify(nameof(StatesCount));
            Notify(nameof(InitialState));
            Notify(nameof(InitialStateIndex));
            Notify(nameof(AvailableStates));
        }

        public void MoveStateAtIndex(int sourceIndex, int destinationIndex)
        {
            var statesProperty =
                SerializedProperty.FindPropertyRelative(StateMachineDefinition.STATES_PROPERTY_NAME);

            Undo.RecordObject(statesProperty.serializedObject.targetObject, "Reorder state");

            statesProperty.serializedObject.Update();
            statesProperty.MoveArrayElement(sourceIndex, destinationIndex);
            statesProperty.serializedObject.ApplyModifiedProperties();

            Notify(nameof(States));
            Notify(nameof(StatesCount));
            Notify(nameof(AvailableStates));
        }
    }
}