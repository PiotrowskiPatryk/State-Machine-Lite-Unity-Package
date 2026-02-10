using System;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Core.ReferencePicker.Trigger;
using UnityEngine;

namespace Samples.GameScenario.Scripts.StateMachines.Key.Payload
{
    [Serializable]
    public sealed class ReadyToPickKeyStatePayload : IPayload
    {
        [SerializeField]
        private TriggerReferencePicker _keyPickedTrigger;

        public TriggerReferencePicker KeyPickedTrigger => _keyPickedTrigger;

        public bool IsValid()
        {
            return KeyPickedTrigger.IsReferenceSelected;
        }
    }
}