using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace Dev.Cortez.StateMachines.Core.Condition
{
    public sealed class TriggerBasedCondition : ICondition<ITrigger>
    {
        private ITrigger _trigger;
        private bool _isSatisfied;
        
        public event Action<ICondition, bool> SatisfiedChanged;

        public bool IsSatisfied
        {
            get => _isSatisfied;
            private set
            {
                if (value != _isSatisfied)
                {
                    return;
                }
                
                _isSatisfied = value;
                SatisfiedChanged?.Invoke(this, _isSatisfied);
            }
        }

        public UniTask<bool> InitializeAsync(ITrigger payload, CancellationToken cancellationToken = default)
        {
            _trigger = payload;
            _trigger.TriggeredValueChanged += OnTriggeredValueChanged;
            IsSatisfied = _trigger.IsTriggered;
            
            return UniTask.FromResult(true);
        }
        
        public ValueTask DisposeAsync()
        {
            _trigger.TriggeredValueChanged -= OnTriggeredValueChanged;
            _trigger = null;
            return UniTask.CompletedTask;
        }
        
        private void OnTriggeredValueChanged(ITrigger trigger, bool value)
        {
            IsSatisfied = value;
        }
    }
}
