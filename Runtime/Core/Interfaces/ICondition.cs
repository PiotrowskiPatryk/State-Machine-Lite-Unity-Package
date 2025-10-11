using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Dev.Cortez.StateMachines.Core
{
    public interface ICondition : IAsyncDisposable
    {
        event Action<ICondition, bool> SatisfiedChanged;

        bool IsSatisfied { get; }
    }

    public interface ICondition<in TPayload> : ICondition
    {
        UniTask<bool> InitializeAsync(TPayload payload, CancellationToken cancellationToken = default);
    }
}