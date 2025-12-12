using System;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Core.ReferencePicker;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Core.Condition.Payload
{
    [Serializable]
    public sealed class StateBasedConditionPayload : IPayload
    {
        [SerializeField]
        private StateReferencePicker _stateReferencePicker;

        public StateReferencePicker StateReferencePicker => _stateReferencePicker;

        public bool IsValid()
        {
            return _stateReferencePicker.IsReferenceSelected;
        }
    }
}