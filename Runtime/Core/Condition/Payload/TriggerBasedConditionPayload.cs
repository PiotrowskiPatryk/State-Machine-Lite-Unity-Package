using System;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Core.ReferencePicker;
using Dev.Cortez.StateMachines.Core.ReferencePicker.Trigger;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Core.Condition.Payload
{
    [Serializable]
    public sealed class TriggerBasedConditionPayload : IPayload
    {
        [SerializeField]
        private TriggerReferencePicker _triggerReferencePicker;

        [SerializeField]
        private TriggerSatisfiedConditionType _triggerSatisfiedConditionType =
            TriggerSatisfiedConditionType.WhenTriggered;

        public TriggerReferencePicker TriggerReferencePicker => _triggerReferencePicker;
        public TriggerSatisfiedConditionType TriggerSatisfiedConditionType => _triggerSatisfiedConditionType;

        public TriggerBasedConditionPayload()
        {
        }

        public TriggerBasedConditionPayload(
            TriggerReferencePicker triggerReferencePicker,
            TriggerSatisfiedConditionType triggerSatisfiedConditionType)
        {
            _triggerReferencePicker = triggerReferencePicker;
            _triggerSatisfiedConditionType = triggerSatisfiedConditionType;
        }

        public bool IsValid()
        {
            return _triggerReferencePicker.IsReferenceSelected &&
                   _triggerSatisfiedConditionType != TriggerSatisfiedConditionType.Undefined;
        }
    }
}