using System;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Core.Configuration
{
    public sealed class StateConfigurationScriptable : ScriptableObject, IIdentifiable
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
