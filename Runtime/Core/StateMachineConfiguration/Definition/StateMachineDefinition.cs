using System;
using System.Collections.Generic;
using Dev.Cortez.StateMachines.Core.Interfaces;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Core.StateMachineConfiguration.Definition
{
    [Serializable]
    public sealed class StateMachineDefinition
    {
        public static string ID_PROPERTY_NAME = nameof(_id);
        public static string NAME_PROPERTY_NAME = nameof(_name);
        public static string DESCRIPTION_PROPERTY_NAME = nameof(_description);
        public static string STATE_MACHINE_TYPE_PROPERTY_NAME = nameof(_stateMachineTypeName);
        public static string TRANSITION_SOLVER_TYPE_PROPERTY_NAME = nameof(_transitionSolverTypeName);
        public static string STATES_PROPERTY_NAME = nameof(_states);
        public static string PAYLOAD_PROPERTY_NAME = nameof(_payload);

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
        private List<StateDefinition> _states;

        [SerializeReference]
        private IPayload _payload;

        public string Id => _id;
        public string Name => _name;
        public string Description => _description;
        public string StateMachineTypeName => _stateMachineTypeName;
        public string TransitionSolverTypeName => _transitionSolverTypeName;
        public List<StateDefinition> States => _states;
        public IPayload Payload => _payload;
    }
}