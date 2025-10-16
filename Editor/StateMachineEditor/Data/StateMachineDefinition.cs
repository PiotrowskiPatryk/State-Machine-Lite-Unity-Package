using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dev.Cortez.StateMachines.StateMachineEditor.Data
{
    [Serializable]
    public sealed class StateMachineDefinition
    {
        [field: SerializeField]
        public string Id { get; private set; }
        [field: SerializeField]
        public string Name { get; private set; }
        [field: SerializeField]
        public string Description { get; private set; }
        [field: SerializeField]
        public Type StateMachineType { get; private set; }
        [field: SerializeField]
        public Type TransitionSolverType { get; private set; }
        [field: SerializeField]
        public List<StateDefinition> States { get; private set; }
    }
}
