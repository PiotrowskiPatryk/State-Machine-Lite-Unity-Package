using System;
using Dev.Cortez.StateMachines.Core.Interfaces;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.Data.Definition
{
    [Serializable]
    public sealed class StateDefinition
    {
        public static string ID_PROPERTY_NAME = nameof(_id);
        public static string NODE_POSITION_PROPERTY_NAME = nameof(_nodePosition);
        public static string NAME_PROPERTY_NAME = nameof(_name);
        public static string DESCRIPTION_PROPERTY_NAME = nameof(_description);
        public static string PAYLOAD_PROPERTY_NAME = nameof(_payload);
        public static string TYPE_NAME_PROPERTY_NAME = nameof(_typeName);
        public static string STATE_MACHINE_TYPE_NAME_PROPERTY_NAME = nameof(_stateMachineTypeName);

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

        public Vector2Int NodePosition => _nodePosition;
        public string Id => _id;
        public string Name => _name;
        public string Description => _description;
        public string TypeName => _typeName;
        public string StateMachineTypeName => _stateMachineTypeName;
        public IPayload Payload => _payload;
    }
}