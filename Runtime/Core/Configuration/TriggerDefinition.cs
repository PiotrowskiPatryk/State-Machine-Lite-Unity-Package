using System;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Core.Configuration
{
    [Serializable]
    public class TriggerDefinition
    {
        [SerializeField]
        private string _identifier;
        [SerializeField]
        private string _triggerName;
        [SerializeField]
        private Type _triggerType = null;
        [SerializeField]
        private string _triggerTypeString;
        
        public string Identifier => _identifier;

        public TriggerDefinition()
        {
            _identifier = Guid.NewGuid().ToString();
            _triggerName = "Trigger";
            
        }
    }
}
