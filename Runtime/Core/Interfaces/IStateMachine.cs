using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;

namespace Dev.Cortez.StateMachines.Core
{
    /// <summary>
    ///     Represents a state machine interface.
    /// </summary>
    public interface IStateMachine : IIdentifable, IAsyncInitializable, IAsyncDisposable
    {
        bool IsActive { get; }

        [CanBeNull]
        IState ActiveState { get; }

        UniTask<bool> ActivateAsync(CancellationToken cancellationToken);
        UniTask<bool> DeactivateAsync(CancellationToken cancellationToken);
    }
}