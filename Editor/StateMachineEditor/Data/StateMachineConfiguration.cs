using System;
using System.Collections.Generic;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Data.Definition;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Data
{
    [Serializable]
    public class StateMachineConfiguration
    {
        [SerializeField]
        private List<StateMachineDefinition> _stateMachines = new();
    }
}