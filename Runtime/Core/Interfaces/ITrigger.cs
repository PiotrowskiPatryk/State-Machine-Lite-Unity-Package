using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Interfaces;

namespace Dev.Cortez.StateMachines.Core
{
    public interface ITrigger : IIdentifiable, IAsyncDisposable
    {
        event Action<ITrigger, bool> TriggeredValueChanged;
        bool IsTriggered { get; }

        UniTask<bool> TriggerValueAsync(bool targetValue, CancellationToken cancellationToken);
    }

    public interface ITrigger<in TPayload> : ITrigger where TPayload : IPayload
    {
        UniTask<bool> InitializeAsync(TPayload payload, CancellationToken cancellationToken);
    }
}