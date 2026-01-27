using System;
using System.Collections.Generic;
using Dev.Cortez.StateMachines.Core.StateMachineConfiguration.Definition;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Core.StateMachineConfiguration
{
    [Serializable]
    public class StateMachineConfiguration
    {
        public static string STATE_MACHINES_PROPERTY_NAME = nameof(_stateMachines);

        [SerializeField]
        private List<StateMachineDefinition> _stateMachines = new();

        public IReadOnlyList<StateMachineDefinition> StateMachines => _stateMachines;
    }
}