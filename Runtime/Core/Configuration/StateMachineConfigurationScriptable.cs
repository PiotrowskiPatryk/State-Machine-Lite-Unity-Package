using System;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Core.Configuration
{
    public sealed class StateMachineConfigurationScriptable : ScriptableObject, IIdentifiable
    {
        [field: SerializeField]
        public string Id { get; private set; }
        [field: SerializeField]
        public string Name { get; private set; }
        [field: SerializeField]
        public string Description { get; private set; }
        [field: SerializeField]
        public StateConfigurationScriptable InitialState { get; private set; }
        [field: SerializeField]
        public StateConfigurationScriptable[] States { get; private set; }
        [field: SerializeField]
        public Type StateMachineType { get; private set; }
    }
}
