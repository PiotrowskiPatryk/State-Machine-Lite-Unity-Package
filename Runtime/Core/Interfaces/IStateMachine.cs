using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Data;
using JetBrains.Annotations;

namespace Dev.Cortez.StateMachines.Core.Interfaces
{
    /// <summary>
    ///     Represents a state machine interface.
    /// </summary>
    public interface IStateMachine : IIdentifiable, IAsyncDisposable
    {
        /// <summary>
        /// Gets a read-only list of all states in this state machine.
        /// </summary>
        IReadOnlyList<IState> States { get; }

        /// <summary>
        /// Gets a read-only list of all transition rules for this state machine.
        /// </summary>
        IReadOnlyList<TransitionRule> TransitionRules { get; }

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