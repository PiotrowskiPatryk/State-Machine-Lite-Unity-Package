using System.Reflection;
using Dev.Cortez.StateMachines.Core.Trigger;

namespace Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Common.Builders
{
    /// <summary>
    /// Fluent builder for creating DefaultTriggerPayload instances in tests.
    /// Uses reflection to set private [SerializeField] backing fields.
    /// </summary>
    public sealed class DefaultTriggerPayloadBuilder
    {
        private const BindingFlags FieldFlags = BindingFlags.NonPublic | BindingFlags.Instance;

        private TriggerActivationRule _activationRule = TriggerActivationRule.Immediately;
        private int _activationFrameDelay = 1;
        private float _activationTimeDelay;
        private TriggerDeactivationRule _deactivationRule = TriggerDeactivationRule.Never;
        private int _deactivationFrameDelay = 1;
        private float _deactivationTimeDelay;

        public DefaultTriggerPayloadBuilder WithActivationRule(TriggerActivationRule rule)
        {
            _activationRule = rule;

            return this;
        }

        public DefaultTriggerPayloadBuilder WithActivationFrameDelay(int frames)
        {
            _activationFrameDelay = frames;

            return this;
        }

        public DefaultTriggerPayloadBuilder WithActivationTimeDelay(float seconds)
        {
            _activationTimeDelay = seconds;

            return this;
        }

        public DefaultTriggerPayloadBuilder WithDeactivationRule(TriggerDeactivationRule rule)
        {
            _deactivationRule = rule;

            return this;
        }

        public DefaultTriggerPayloadBuilder WithDeactivationFrameDelay(int frames)
        {
            _deactivationFrameDelay = frames;

            return this;
        }

        public DefaultTriggerPayloadBuilder WithDeactivationTimeDelay(float seconds)
        {
            _deactivationTimeDelay = seconds;

            return this;
        }

        public DefaultTriggerPayload Build()
        {
            var payload = new DefaultTriggerPayload();
            var type = typeof(DefaultTriggerPayload);

            type.GetField("_triggerActivationRule", FieldFlags)?.SetValue(payload, _activationRule);
            type.GetField("_triggerActivationFrameDelay", FieldFlags)?.SetValue(payload, _activationFrameDelay);
            type.GetField("_triggerActivationTimeDelayInSeconds", FieldFlags)?.SetValue(payload, _activationTimeDelay);
            type.GetField("_triggerDeactivationRule", FieldFlags)?.SetValue(payload, _deactivationRule);
            type.GetField("_triggerDeactivationFrameDelay", FieldFlags)?.SetValue(payload, _deactivationFrameDelay);
            type.GetField("_triggerDeactivationTimeDelayInSeconds", FieldFlags)?.SetValue(payload, _deactivationTimeDelay);

            return payload;
        }

        /// <summary>
        /// Creates a new builder with defaults: Immediately activation, Never deactivation.
        /// </summary>
        public static DefaultTriggerPayloadBuilder Create()
        {
            return new DefaultTriggerPayloadBuilder();
        }
    }
}
