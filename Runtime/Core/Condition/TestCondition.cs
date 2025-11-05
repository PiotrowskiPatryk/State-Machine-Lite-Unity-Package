using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace Dev.Cortez.StateMachines.Core.Condition
{
    public sealed class TestCondition : ICondition<TestConditionPayload>
    {
        public event Action<ICondition, bool> SatisfiedChanged;
        public bool IsSatisfied { get; }

        public UniTask<bool> InitializeAsync(TestConditionPayload payload,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }
    }
}