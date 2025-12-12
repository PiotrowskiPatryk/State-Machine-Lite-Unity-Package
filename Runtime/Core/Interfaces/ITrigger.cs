using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Dev.Cortez.StateMachines.Core
{
    public interface ITrigger : IIdentifiable, IAsyncDisposable
    {
        event Action<ITrigger, bool> TriggeredValueChanged;
        bool IsTriggered { get; }

        UniTask<bool> TriggerValueAsync(bool targetValue, CancellationToken cancellationToken);
    }
}