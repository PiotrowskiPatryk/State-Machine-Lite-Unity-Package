using System;
using UnityEngine;

namespace Dev.Cortez.StateMachines.StateMachineEditor.Data
{
    [Serializable]
    public sealed class StateDefinition
    {
        [field: SerializeField]
        public string Id { get; private set; }
        [field: SerializeField]
        public string Name { get; private set; }
        [field: SerializeField]
        public string Description { get; private set; }
        [field: SerializeField]
        public Type StateType { get; private set; }
    }
}
