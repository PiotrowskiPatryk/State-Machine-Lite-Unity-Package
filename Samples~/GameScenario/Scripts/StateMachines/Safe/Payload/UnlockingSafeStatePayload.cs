using System;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Core.ReferencePicker.Trigger;
using UnityEngine;

namespace Samples.GameScenario.Scripts.StateMachines.Safe.Payload
{
    [Serializable]
    public sealed class UnlockingSafeStatePayload : IPayload
    {
        [SerializeField]
        private TriggerReferencePicker _triggeredOpenedSafe;

        public TriggerReferencePicker TriggeredOpenedSafe => _triggeredOpenedSafe;

        public bool IsValid()
        {
            return _triggeredOpenedSafe.IsReferenceSelected;
        }
    }
}