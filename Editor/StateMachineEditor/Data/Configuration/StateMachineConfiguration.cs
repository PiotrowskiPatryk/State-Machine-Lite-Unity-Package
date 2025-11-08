using System;
using System.Collections.Generic;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Data.Definition;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Data.Configuration
{
    [Serializable]
    public class StateMachineConfiguration
    {
        public static string STATE_MACHINES_PROPERTY_NAME = nameof(_stateMachines);

        [SerializeField]
        private List<StateMachineDefinition> _stateMachines = new();
    }
}