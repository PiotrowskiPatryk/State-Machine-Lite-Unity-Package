using System.Threading;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Abstraction;
using JetBrains.Annotations;

namespace Dev.Cortez.StateMachines.Core.Trigger
{
    [UsedImplicitly]
    public sealed class ManualTrigger : TriggerBase
    {
        public ManualTrigger(string id, string name, string description) : base(id, name, description)
        {
        }

        public override UniTask<bool> TriggerValueAsync(bool targetValue, CancellationToken cancellationToken)
        {
            IsTriggered = targetValue;

            return UniTask.FromResult(true);
        }
    }
}