using System.Threading;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Abstraction;

namespace Dev.Cortez.StateMachines.Core.Trigger
{
    public class OneFrameTrigger : TriggerBase
    {
        public OneFrameTrigger(string id, string name, string description) : base(id, name, description)
        {
        }

        public override async UniTask<bool> TriggerValueAsync(bool targetValue, CancellationToken cancellationToken)
        {
            if (targetValue == IsTriggered)
            {
                return true;
            }

            if (targetValue)
            {
                IsTriggered = true;
                await UniTask.Yield();
            }

            IsTriggered = false;

            return true;
        }
    }
}