using System;
using Dev.Cortez.StateMachines.Core.Interfaces;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Data.Definition
{
    [Serializable]
    public sealed class StateDefinition
    {
        [SerializeField]
        private string _id;
        [SerializeField]
        private string _name;
        [SerializeField]
        private string _description;
        [SerializeField]
        private string _typeName;
        [SerializeField]
        private IPayload _payload;
        
        public string Id => _id;
        public string Name => _name;
        public string Description => _description;
        public string TypeName => _typeName;
        public IPayload Payload => _payload;
    }
}
