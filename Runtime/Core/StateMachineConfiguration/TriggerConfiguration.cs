using System;
using System.Collections.Generic;
using Dev.Cortez.StateMachines.Core.StateMachineConfiguration.Definition;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Core.StateMachineConfiguration
{
    [Serializable]
    public class TriggerConfiguration
    {
        public static string TRIGGERS_PROPERTY_NAME = nameof(_triggers);

        [SerializeField]
        private List<TriggerDefinition> _triggers = new();

        public IReadOnlyList<TriggerDefinition> Triggers => _triggers;
    }
}