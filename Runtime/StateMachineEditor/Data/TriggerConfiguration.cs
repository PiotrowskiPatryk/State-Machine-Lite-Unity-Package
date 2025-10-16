using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dev.Cortez.StateMachines.StateMachineEditor.Data
{
    [Serializable]
    public class TriggerConfiguration
    {
        [SerializeField]
        private List<TriggerDefinition> _triggers = new();
    }
}
