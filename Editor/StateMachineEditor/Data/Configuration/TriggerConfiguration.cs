using System;
using System.Collections.Generic;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Data.Definition;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Data.Configuration
{
    [Serializable]
    public class TriggerConfiguration
    {
        public static string TRIGGERS_PROPERTY_NAME = nameof(_triggers);

        [SerializeField]
        private List<TriggerDefinition> _triggers = new();
    }
}