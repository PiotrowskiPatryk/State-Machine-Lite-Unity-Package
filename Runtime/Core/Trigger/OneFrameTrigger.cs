using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Logging;

namespace Dev.Cortez.StateMachines.Core.Trigger
{
    public class OneFrameTrigger : ITrigger
    {
        private bool _value;
        
        public event Action<ITrigger, bool> TriggeredValueChanged;
        
        public string Id { get; }
        public string Name { get; }
        public string Description { get; }
        public bool IsTriggered
        {
            get => _value;
            private set
            {
                if (value == _value)
                {
                    return;
                }
                
                _value = value;
                TriggeredValueChanged?.Invoke(this, value);
            }
        }
        
        public OneFrameTrigger(string id)
        {
            Id = id;
        }
        
        public ValueTask DisposeAsync()
        {
            TriggeredValueChanged = null;
            
            return default;
        }
        
        public UniTask<bool> TriggerValueAsync(bool targetValue, CancellationToken cancellationToken)
        {
            if (targetValue == IsTriggered)
            {
                LoggerService.Logger.LogWarning($"Signal {Id} was already triggered");
                return UniTask.FromResult(false);
            }
            
            IsTriggered = targetValue;

            if (targetValue)
            {
                ResetTriggerAsync().Forget();
            }
            
            return UniTask.FromResult(true);
        }

        private async UniTaskVoid ResetTriggerAsync()
        {
            await UniTask.NextFrame();
            IsTriggered = false;
        }
    }
}
