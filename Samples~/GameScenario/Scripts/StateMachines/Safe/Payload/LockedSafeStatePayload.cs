using System;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Core.ReferencePicker.Trigger;
using UnityEngine;

namespace Samples.GameScenario.Scripts.StateMachines.Safe.Payload
{
    [Serializable]
    public sealed class LockedSafeStatePayload : IPayload
    {
        [SerializeField]
        private TriggerReferencePicker _triggeredValidSafeButtonSequence;

        [SerializeField]
        private TriggerReferencePicker _triggeredInvalidSafeButtonSequence;

        public TriggerReferencePicker TriggeredValidSafeButtonSequence => _triggeredValidSafeButtonSequence;
        public TriggerReferencePicker TriggeredInvalidSafeButtonSequence => _triggeredInvalidSafeButtonSequence;

        public bool IsValid()
        {
            return TriggeredValidSafeButtonSequence.IsReferenceSelected &&
                   TriggeredInvalidSafeButtonSequence.IsReferenceSelected;
        }
    }
}