using System;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Core.Utilities;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Core.StateMachineConfiguration.Definition
{
    [Serializable]
    public sealed class TriggerDefinition : IValidatable, ISerializationCallbackReceiver
    {
        public const string ID_PROPERTY_NAME = nameof(_id);
        public const string NAME_PROPERTY_NAME = nameof(_name);
        public const string DESCRIPTION_PROPERTY_NAME = nameof(_description);
        public const string TYPE_NAME_PROPERTY_NAME = nameof(_typeName);
        public const string SCRIPT_GUID_PROPERTY_NAME = nameof(_scriptGuid);
        public const string PAYLOAD_PROPERTY_NAME = nameof(_payload);
        public const string PAYLOAD_SCRIPT_GUID_PROPERTY_NAME = nameof(_payloadScriptGuid);

        [SerializeField]
        private string _id;

        [SerializeField]
        private string _name;

        [SerializeField]
        private string _description;

        [SerializeField]
        private string _typeName;

        [SerializeField]
        private string _scriptGuid;

        [SerializeReference]
        private IPayload _payload;

        [SerializeField]
        private string _payloadScriptGuid;

        public string Id => _id;

        public string Name => _name;

        public string Description => _description;

        public string TypeName => _typeName;

        public string ScriptGuid => _scriptGuid;

        public IPayload Payload => _payload;

        public string PayloadScriptGuid => _payloadScriptGuid;

        public TriggerDefinition(string id, string name, string description, string typeName, IPayload payload)
        {
            _id = id;
            _name = name;
            _description = description;
            _typeName = typeName;
            _payload = payload;
        }

        public TriggerDefinition()
        {
        }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(_id) && !string.IsNullOrWhiteSpace(_name) &&
                   !string.IsNullOrWhiteSpace(_typeName) && Payload?.IsValid() == true;
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
            UpdateTypeName();
        }

        private void UpdateTypeName()
        {
            var resolvedType = ScriptGuidUtility.GetTypeFromGuid(_scriptGuid);

            if (resolvedType != null)
            {
                _typeName = resolvedType.AssemblyQualifiedName;
            }
        }
    }
}