using System;
using Dev.Cortez.StateMachines.Core.Interfaces;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Core.StateMachineConfiguration.Definition
{
    [Serializable]
    public sealed class TriggerDefinition : IValidatable
    {
        public static string ID_PROPERTY_NAME = nameof(_id);
        public static string NAME_PROPERTY_NAME = nameof(_name);
        public static string DESCRIPTION_PROPERTY_NAME = nameof(_description);
        public static string TYPE_NAME_PROPERTY_NAME = nameof(_typeName);
        public static string PAYLOAD_PROPERTY_NAME = nameof(_payload);

        [SerializeField]
        private string _id;

        [SerializeField]
        private string _name;

        [SerializeField]
        private string _description;

        [SerializeField]
        private string _typeName;

        [SerializeReference]
        private IPayload _payload;

        public string Id => _id;

        public string Name => _name;

        public string Description => _description;

        public string TypeName => _typeName;

        public IPayload Payload => _payload;

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
                   !string.IsNullOrWhiteSpace(_typeName) && Payload != null;
        }
    }
}