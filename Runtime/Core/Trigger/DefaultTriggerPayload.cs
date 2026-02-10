using System;
using Dev.Cortez.StateMachines.Core.Interfaces;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Core.Trigger
{
    [Serializable]
    public sealed class DefaultTriggerPayload : IPayload
    {
        [SerializeField]
        private TriggerActivationRule _triggerActivationRule;

        [SerializeField, Min(1)]
        private int _triggerActivationFrameDelay;

        [SerializeField, Min(0f)]
        private float _triggerActivationTimeDelayInSeconds;

        [SerializeField]
        private TriggerDeactivationRule _triggerDeactivationRule;

        [SerializeField, Min(1)]
        private int _triggerDeactivationFrameDelay;

        [SerializeField, Min(0f)]
        private float _triggerDeactivationTimeDelayInSeconds;

        public TriggerActivationRule ActivationRule => _triggerActivationRule;
        public int ActivationFrameDelay => _triggerActivationFrameDelay;
        public float ActivationTimeDelay => _triggerActivationTimeDelayInSeconds;
        public TriggerDeactivationRule DeactivationRule => _triggerDeactivationRule;
        public int DeactivationFrameDelay => _triggerDeactivationFrameDelay;
        public float DeactivationTimeDelay => _triggerDeactivationTimeDelayInSeconds;

        public bool IsValid()
        {
            var isActivationValid = _triggerActivationRule switch
            {
                TriggerActivationRule.Immediately => true,
                TriggerActivationRule.AfterFixedFrame => _triggerActivationFrameDelay >= 1,
                TriggerActivationRule.AfterTime or TriggerActivationRule.AfterTimeUnscaled =>
                    _triggerActivationTimeDelayInSeconds >= 0f,
                _ => false
            };

            var isDeactivationValid = _triggerDeactivationRule switch
            {
                TriggerDeactivationRule.Never => true,
                TriggerDeactivationRule.NextFrame => true,
                TriggerDeactivationRule.AfterFixedFrame => _triggerDeactivationFrameDelay >= 1,
                TriggerDeactivationRule.AfterTime or TriggerDeactivationRule.AfterTimeUnscaled =>
                    _triggerDeactivationTimeDelayInSeconds >= 0f,
                _ => false
            };

            return isActivationValid && isDeactivationValid;
        }
    }
}