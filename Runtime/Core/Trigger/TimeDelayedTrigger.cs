using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace Dev.Cortez.StateMachines.Core.Trigger
{
    public class TimeDelayedTrigger : ITrigger<TimeDelayTriggerPayload>
    {
        public async ValueTask DisposeAsync()
        {
            
        }

        public UniTask<bool> InitializeAsync(TimeDelayTriggerPayload payload, CancellationToken cancellationToken)
        {
            return UniTask.FromResult(true);
        }

        public string Id { get; }
        public string Name { get; }
        public string Description { get; }
        public bool IsTriggered { get; }
        public event Action<ITrigger, bool> TriggeredValueChanged;
        public UniTask<bool> TriggerValueAsync(bool targetValue, CancellationToken cancellationToken)
        {
            return UniTask.FromResult(true);
        }
    }
}