using System.Collections.Generic;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Core.Configuration
{
    [CreateAssetMenu(menuName = "State Machines/Configuration/StateMachineContainerConfigurationScriptable")]
    public class StateMachineContainerConfigurationScriptable : ScriptableObject
    {
        [field: SerializeField]
        public List<TriggerConfigurationScriptable> Triggers { get; private set; } = new();
        [field: SerializeField]
        public List<ConditionConfigurationScriptable> Conditions { get; private set; } = new();
        [field: SerializeField]
        public List<StateMachineConfigurationScriptable> StateMachines { get; private set; } = new();
    }
}
