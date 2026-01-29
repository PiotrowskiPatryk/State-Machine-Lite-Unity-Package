using System;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Core.ReferencePicker;
using Dev.Cortez.StateMachines.Core.ReferencePicker.State;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Core.Condition.Payload
{
    [Serializable]
    public sealed class StateBasedConditionPayload : IPayload
    {
        [SerializeField]
        private StateReferencePicker _stateReferencePicker;

        [SerializeField]
        private StateSatisfiedConditionType _stateSatisfiedConditionType;

        public StateReferencePicker StateReferencePicker => _stateReferencePicker;
        public StateSatisfiedConditionType StateSatisfiedConditionType => _stateSatisfiedConditionType;

        public StateBasedConditionPayload()
        {
        }

        public StateBasedConditionPayload(
            StateReferencePicker stateReferencePicker,
            StateSatisfiedConditionType stateSatisfiedConditionType)
        {
            _stateReferencePicker = stateReferencePicker;
            _stateSatisfiedConditionType = stateSatisfiedConditionType;
        }

        public bool IsValid()
        {
            return _stateReferencePicker.IsReferenceSelected;
        }
    }
}