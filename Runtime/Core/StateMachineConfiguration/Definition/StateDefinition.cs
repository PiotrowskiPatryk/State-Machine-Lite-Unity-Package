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
    public sealed class StateDefinition : IValidatable, ISerializationCallbackReceiver
    {
        public const string ID_PROPERTY_NAME = nameof(_id);
        public const string NODE_POSITION_PROPERTY_NAME = nameof(_nodePosition);
        public const string NAME_PROPERTY_NAME = nameof(_name);
        public const string DESCRIPTION_PROPERTY_NAME = nameof(_description);
        public const string PAYLOAD_PROPERTY_NAME = nameof(_payload);
        public const string PAYLOAD_SCRIPT_GUID_PROPERTY_NAME = nameof(_payloadScriptGuid);
        public const string TYPE_NAME_PROPERTY_NAME = nameof(_typeName);
        public const string SCRIPT_GUID_PROPERTY_NAME = nameof(_scriptGuid);
        public const string STATE_MACHINE_TYPE_NAME_PROPERTY_NAME = nameof(_stateMachineTypeName);
        public const string STATE_MACHINE_SCRIPT_GUID_PROPERTY_NAME = nameof(_stateMachineScriptGuid);
        public const string TRANSITION_RULES_PROPERTY_NAME = nameof(_transitionRules);

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
        private string _scriptGuid;

        [SerializeField]
        private string _stateMachineTypeName;

        [SerializeField]
        private string _stateMachineScriptGuid;

        [SerializeReference]
        private IPayload _payload;

        [SerializeField]
        private string _payloadScriptGuid;

        [SerializeField]
        private List<TransitionRuleDefinition> _transitionRules = new();

        public Vector2Int NodePosition => _nodePosition;
        public string Id => _id;
        public string Name => _name;
        public string Description => _description;
        public string TypeName => _typeName;
        public string ScriptGuid => _scriptGuid;
        public string StateMachineTypeName => _stateMachineTypeName;
        public string StateMachineScriptGuid => _stateMachineScriptGuid;
        public IPayload Payload => _payload;
        public string PayloadScriptGuid => _payloadScriptGuid;
        public List<TransitionRuleDefinition> TransitionRules => _transitionRules;

#pragma warning disable S107
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
#pragma warning restore S107

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

        public void OnBeforeSerialize()
        {
            if (_payload != null)
            {
                _payloadScriptGuid = ScriptGuidUtility.GetGuidForType(_payload.GetType());
            }

            UpdateTypeName();
        }

        public void OnAfterDeserialize()
        {
#if UNITY_EDITOR
            EditorApplication.delayCall += UpdateTypeName;
#else
            UpdateTypeName();
#endif
        }

        private void UpdateTypeName()
        {
            var resolvedType = ScriptGuidUtility.GetTypeFromGuid(_scriptGuid);

            if (resolvedType != null)
            {
                _typeName = resolvedType.AssemblyQualifiedName;
            }

            var resolvedStateMachineType = ScriptGuidUtility.GetTypeFromGuid(_stateMachineScriptGuid);

            if (resolvedStateMachineType != null)
            {
                _stateMachineTypeName = resolvedStateMachineType.AssemblyQualifiedName;
            }
        }
    }
}