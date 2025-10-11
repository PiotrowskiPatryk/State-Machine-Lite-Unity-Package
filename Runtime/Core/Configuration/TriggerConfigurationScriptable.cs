using UnityEngine;

namespace Dev.Cortez.StateMachines.Core.Configuration
{
    public class TriggerConfigurationScriptable : ScriptableObject, IIdentifiable
    {
        public string Id { get; }
        public string Name { get; }
        public string Description { get; }
    }
}
