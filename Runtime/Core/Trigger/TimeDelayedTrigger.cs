using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Abstraction;

namespace Dev.Cortez.StateMachines.Core.Trigger
{
    public class TimeDelayedTrigger : TriggerBase<TimeDelayTriggerPayload>
    {
        public override bool IsTriggered { get; }

        public TimeDelayedTrigger(string id, string name, string description) : base(id, name, description)
        {
        }

        public override UniTask<bool> InitializeAsync(TimeDelayTriggerPayload payload,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override UniTask<bool> TriggerValueAsync(bool targetValue, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}