using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Data;
using JetBrains.Annotations;

namespace Dev.Cortez.StateMachines.Core
{
    /// <summary>
    ///     Represents a state machine interface.
    /// </summary>
    public interface IStateMachine : IIdentifiable, IAsyncDisposable
    {
        bool IsActive { get; }

        [CanBeNull]
        IState ActiveState { get; }

        UniTask<bool> InitializeAsync(StateMachineSettings stateMachineSettings,
            CancellationToken cancellationToken);

        UniTask<bool> MoveToStateAsync([NotNull] IState state, CancellationToken cancellationToken);
        UniTask<bool> ActivateAsync(CancellationToken cancellationToken);
        UniTask<bool> DeactivateAsync(CancellationToken cancellationToken);
    }
}