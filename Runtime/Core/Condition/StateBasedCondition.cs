using System.Threading;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Abstraction;
using Dev.Cortez.StateMachines.Core.Condition.Payload;

namespace Dev.Cortez.StateMachines.Core.Condition
{
    public sealed class StateBasedCondition : ConditionBase<StateBasedConditionPayload>
    {
        public override bool IsSatisfied { get; }

        // TODO - apply fetching payload data
        protected override UniTask<bool> InitializeAsync(StateBasedConditionPayload payload,
            CancellationToken cancellationToken)
        {
            return UniTask.FromResult(true);
        }
    }
}