using System;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Core.ReferencePicker.Trigger;
using UnityEngine;

namespace Samples.GameScenario.Scripts.StateMachines.Door.Payload
{
    [Serializable]
    public class InteractableDoorPayload : IPayload
    {
        [SerializeField]
        private TriggerReferencePicker _doorInteractedTrigger;

        public TriggerReferencePicker DoorInteractedTrigger => _doorInteractedTrigger;

        public bool IsValid()
        {
            return _doorInteractedTrigger.IsReferenceSelected;
        }
    }
}