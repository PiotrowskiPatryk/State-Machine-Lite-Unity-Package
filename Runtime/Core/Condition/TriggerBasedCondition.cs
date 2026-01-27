using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Abstraction;
using Dev.Cortez.StateMachines.Core.Condition.Payload;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Logging;
using JetBrains.Annotations;

namespace Dev.Cortez.StateMachines.Core.Condition
{
    [UsedImplicitly]
    public sealed class TriggerBasedCondition : ConditionBase<TriggerBasedConditionPayload>
    {
        private ITrigger _targetTrigger;
        private TriggerSatisfiedConditionType _triggerSatisfiedConditionType;

        public override bool IsSatisfied
        {
            get
            {
                return _triggerSatisfiedConditionType switch
                {
                    TriggerSatisfiedConditionType.Undefined => false,
                    TriggerSatisfiedConditionType.WhenTriggered => _targetTrigger?.IsTriggered == true,
                    TriggerSatisfiedConditionType.WhenNotTriggered => _targetTrigger?.IsTriggered == false,
                    _ => false
                };
            }
        }

        public override ValueTask DisposeAsync()
        {
            DisposeTriggerEventListener();

            return base.DisposeAsync();
        }

        protected override UniTask<bool> InitializeAsync(TriggerBasedConditionPayload payload,
            CancellationToken cancellationToken)
        {
            if (!payload.TriggerReferencePicker.IsReferenceSelected)
            {
                LoggerService.Logger.LogError(
                    "Unable to initialize TriggerBasedCondition. Provided trigger reference picker is not provided.");

                return UniTask.FromResult(false);
            }

            _triggerSatisfiedConditionType = payload.TriggerSatisfiedConditionType;
            payload.TriggerReferencePicker.Observe(OnResolvedTrigger, OnUnresolvedTrigger, DisposalCancellationToken);

            return UniTask.FromResult(true);
        }

        private void OnResolvedTrigger(ITrigger trigger)
        {
            DisposeTriggerEventListener();

            _targetTrigger = trigger;
            _targetTrigger.TriggeredValueChanged += OnTriggerValueChanged;
        }

        private void OnUnresolvedTrigger()
        {
            DisposeTriggerEventListener();
        }

        private void DisposeTriggerEventListener()
        {
            if (_targetTrigger == null)
            {
                return;
            }

            _targetTrigger.TriggeredValueChanged -= OnTriggerValueChanged;
            _targetTrigger = null;
        }

        private void OnTriggerValueChanged(ITrigger _, bool __)
        {
            PublishSatisfiedChangedEvent(IsSatisfied);
        }
    }
}