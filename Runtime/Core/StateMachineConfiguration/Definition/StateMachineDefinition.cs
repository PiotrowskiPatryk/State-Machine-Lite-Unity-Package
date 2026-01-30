using System;
using System.Collections.Generic;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Core.Utilities;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Dev.Cortez.StateMachines.Core.StateMachineConfiguration.Definition
{
    [Serializable]
    public sealed class StateMachineDefinition : IValidatable, ISerializationCallbackReceiver
    {
        public const string ID_PROPERTY_NAME = nameof(_id);
        public const string NAME_PROPERTY_NAME = nameof(_name);
        public const string DESCRIPTION_PROPERTY_NAME = nameof(_description);
        public const string STATE_MACHINE_TYPE_PROPERTY_NAME = nameof(_stateMachineTypeName);
        public const string STATE_MACHINE_SCRIPT_GUID_PROPERTY_NAME = nameof(_stateMachineScriptGuid);
        public const string TRANSITION_SOLVER_TYPE_PROPERTY_NAME = nameof(_transitionSolverTypeName);
        public const string TRANSITION_SOLVER_SCRIPT_GUID_PROPERTY_NAME = nameof(_transitionSolverScriptGuid);
        public const string STATES_PROPERTY_NAME = nameof(_states);
        public const string INITIAL_STATE_PROPERTY_NAME = nameof(_initialState);
        public const string PAYLOAD_PROPERTY_NAME = nameof(_payload);
        public const string PAYLOAD_SCRIPT_GUID_PROPERTY_NAME = nameof(_payloadScriptGuid);

        [SerializeField]
        private string _id;

        [SerializeField]
        private string _name;

        [SerializeField]
        private string _description;

        [SerializeField]
        private string _stateMachineTypeName;

        [SerializeField]
        private string _stateMachineScriptGuid;

        [SerializeField]
        private string _transitionSolverTypeName;

        [SerializeField]
        private string _transitionSolverScriptGuid;

        [SerializeField]
        private StateDefinition _initialState;

        [SerializeField]
        private List<StateDefinition> _states;

        [SerializeReference]
        private IPayload _payload;

        [SerializeField]
        private string _payloadScriptGuid;

        public string Id => _id;
        public string Name => _name;
        public string Description => _description;
        public string StateMachineTypeName => _stateMachineTypeName;
        public string StateMachineScriptGuid => _stateMachineScriptGuid;
        public string TransitionSolverTypeName => _transitionSolverTypeName;
        public string TransitionSolverScriptGuid => _transitionSolverScriptGuid;
        public StateDefinition InitialState => _initialState;
        public List<StateDefinition> States => _states;
        public IPayload Payload => _payload;
        public string PayloadScriptGuid => _payloadScriptGuid;

#pragma warning disable S107
        public StateMachineDefinition(
            string id,
            string name,
            string description,
            string stateMachineTypeName,
            string transitionSolverTypeName,
            StateDefinition initialState,
            List<StateDefinition> states,
            IPayload payload)
        {
            _id = id;
            _name = name;
            _description = description;
            _stateMachineTypeName = stateMachineTypeName;
            _transitionSolverTypeName = transitionSolverTypeName;
            _initialState = initialState;
            _states = states ?? new List<StateDefinition>();
            _payload = payload;
        }
#pragma warning restore S107

        public StateMachineDefinition()
        {
        }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(_id) &&
                   !string.IsNullOrWhiteSpace(_name) &&
                   !string.IsNullOrWhiteSpace(_stateMachineTypeName) &&
                   !string.IsNullOrWhiteSpace(_transitionSolverTypeName) &&
                   _initialState?.IsValid() == true &&
                   _states.Count > 0 &&
                   _states.TrueForAll(state => state?.IsValid() == true) &&
                   _payload?.IsValid() == true;
        }

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (_payload != null)
            {
                _payloadScriptGuid = ScriptGuidUtility.GetGuidForType(_payload.GetType());
            }

            UpdateTypeName();
#endif
        }

        public void OnAfterDeserialize()
        {
#if UNITY_EDITOR
            EditorApplication.delayCall -= UpdateTypeName;
            EditorApplication.delayCall += UpdateTypeName;
#endif
        }

#if UNITY_EDITOR
        private void UpdateTypeName()
        {
            var resolvedStateMachineType = ScriptGuidUtility.GetTypeFromGuid(_stateMachineScriptGuid);

            if (resolvedStateMachineType != null)
            {
                _stateMachineTypeName = resolvedStateMachineType.AssemblyQualifiedName;
            }

            var resolvedTransitionSolverType = ScriptGuidUtility.GetTypeFromGuid(_transitionSolverScriptGuid);

            if (resolvedTransitionSolverType != null)
            {
                _transitionSolverTypeName = resolvedTransitionSolverType.AssemblyQualifiedName;
            }
        }
#endif
    }
}