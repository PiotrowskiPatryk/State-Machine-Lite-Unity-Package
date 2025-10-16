using System;
using Dev.Cortez.StateMachines.Core;
using Dev.Cortez.StateMachines.Core.Interfaces;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Data
{
    [Serializable]
    public sealed class TriggerDefinition
    {
        [SerializeField]
        private string _id;
        [SerializeField]
        private string _name;
        [SerializeField]
        private string _description;
        [SerializeField]
        private string _typeName;
        
        // Managed reference to a payload instance when the trigger supports a payload
        [SerializeReference]
        private IPayload _payload;
    }
}
