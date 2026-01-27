using System;
using System.Collections.Generic;
using Dev.Cortez.StateMachines.Core.Interfaces;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Core.StateMachineConfiguration.Definition
{
    [Serializable]
    public sealed class StateMachineDefinition : IValidatable
    {
        public const string ID_PROPERTY_NAME = nameof(_id);
        public const string NAME_PROPERTY_NAME = nameof(_name);
        public const string DESCRIPTION_PROPERTY_NAME = nameof(_description);
        public const string STATE_MACHINE_TYPE_PROPERTY_NAME = nameof(_stateMachineTypeName);
        public const string TRANSITION_SOLVER_TYPE_PROPERTY_NAME = nameof(_transitionSolverTypeName);
        public const string STATES_PROPERTY_NAME = nameof(_states);
        public const string INITIAL_STATE_PROPERTY_NAME = nameof(_initialState);
        public const string PAYLOAD_PROPERTY_NAME = nameof(_payload);

        [SerializeField]
        private string _id;

        [SerializeField]
        private string _name;

        [SerializeField]
        private string _description;

        [SerializeField]
        private string _stateMachineTypeName;

        [SerializeField]
        private string _transitionSolverTypeName;

        [SerializeField]
        private StateDefinition _initialState;

        [SerializeField]
        private List<StateDefinition> _states;

        [SerializeReference]
        private IPayload _payload;

        public string Id => _id;
        public string Name => _name;
        public string Description => _description;
        public string StateMachineTypeName => _stateMachineTypeName;
        public string TransitionSolverTypeName => _transitionSolverTypeName;
        public StateDefinition InitialState => _initialState;
        public List<StateDefinition> States => _states;
        public IPayload Payload => _payload;

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
                   _initialState != null &&
                   _states.TrueForAll(state => state?.IsValid() == true) &&
                   _payload?.IsValid() == true;
        }
    }
}