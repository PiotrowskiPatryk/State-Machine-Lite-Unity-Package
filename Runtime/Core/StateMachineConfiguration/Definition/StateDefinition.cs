using System;
using System.Collections.Generic;
using Dev.Cortez.StateMachines.Core.Interfaces;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Core.StateMachineConfiguration.Definition
{
    [Serializable]
    public sealed class StateDefinition : IValidatable
    {
        public static string ID_PROPERTY_NAME = nameof(_id);
        public static string NODE_POSITION_PROPERTY_NAME = nameof(_nodePosition);
        public static string NAME_PROPERTY_NAME = nameof(_name);
        public static string DESCRIPTION_PROPERTY_NAME = nameof(_description);
        public static string PAYLOAD_PROPERTY_NAME = nameof(_payload);
        public static string TYPE_NAME_PROPERTY_NAME = nameof(_typeName);
        public static string STATE_MACHINE_TYPE_NAME_PROPERTY_NAME = nameof(_stateMachineTypeName);
        public static string TRANSITION_RULES_PROPERTY_NAME = nameof(_transitionRules);

        [SerializeField]
        private string _id;

        [SerializeField]
        private Vector2Int _nodePosition;

        [SerializeField]
        private string _name;

        [SerializeField]
        private string _description;

        [SerializeField]
        private string _typeName;

        [SerializeField]
        private string _stateMachineTypeName;

        [SerializeReference]
        private IPayload _payload;

        [SerializeField]
        private List<TransitionRuleDefinition> _transitionRules = new();

        public Vector2Int NodePosition => _nodePosition;
        public string Id => _id;
        public string Name => _name;
        public string Description => _description;
        public string TypeName => _typeName;
        public string StateMachineTypeName => _stateMachineTypeName;
        public IPayload Payload => _payload;
        public List<TransitionRuleDefinition> TransitionRules => _transitionRules;

        public StateDefinition(
            string id,
            string name,
            string description,
            string typeName,
            IPayload payload,
            List<TransitionRuleDefinition> transitionRules = null,
            Vector2Int nodePosition = default,
            string stateMachineTypeName = null)
        {
            _id = id;
            _name = name;
            _description = description;
            _typeName = typeName;
            _payload = payload;
            _transitionRules = transitionRules ?? new List<TransitionRuleDefinition>();
            _nodePosition = nodePosition;
            _stateMachineTypeName = stateMachineTypeName;
        }

        public StateDefinition()
        {
        }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(_id) &&
                   !string.IsNullOrWhiteSpace(_name) &&
                   !string.IsNullOrWhiteSpace(_typeName) &&
                   !string.IsNullOrWhiteSpace(_stateMachineTypeName) &&
                   _payload?.IsValid() == true &&
                   _transitionRules.TrueForAll(transitionRule => transitionRule?.IsValid() == true);
        }
    }
}