using System;
using System.Collections.Generic;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Data;
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
