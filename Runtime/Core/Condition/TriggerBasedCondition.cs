using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Abstraction;
using Dev.Cortez.StateMachines.Core.Condition.Payload;
using Dev.Cortez.StateMachines.Core.Interfaces;
using JetBrains.Annotations;

namespace Dev.Cortez.StateMachines.Core.Condition
{
    [UsedImplicitly]
    public sealed class TriggerBasedCondition : ConditionBase<TriggerBasedConditionPayload>
    {
        private ITrigger _targetTrigger;

        public override bool IsSatisfied => _targetTrigger?.IsTriggered == true;
        
        protected override UniTask<bool> InitializeAsync(TriggerBasedConditionPayload payload,
            CancellationToken cancellationToken)
        {
            if (!payload.TriggerReferencePicker.IsReferenceSelected)
            {
                Logging.LoggerService.Logger.LogError("Unable to initialize TriggerBasedCondition. Provided trigger reference picker is not provided.");
                return UniTask.FromResult(false);
            }
            
            payload.TriggerReferencePicker.Observe(OnResolvedTrigger, OnUnresolvedTrigger, cancellationToken);
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
        
        public override ValueTask DisposeAsync()
        {
            DisposeTriggerEventListener();
            
            return base.DisposeAsync();
        }
        
        private void OnTriggerValueChanged(ITrigger _, bool value)
        {
            PublishSatisfiedChangedEvent(value);
        }
    }
}