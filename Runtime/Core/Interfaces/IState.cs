using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Data;
using Dev.Cortez.StateMachines.Core.StateMachineConfiguration.Definition;
using JetBrains.Annotations;

namespace Dev.Cortez.StateMachines.Core.Interfaces
{
    /// <summary>
    ///     Represents a state in a state machine.
    /// </summary>
    public interface IState : IIdentifiable, IAsyncDisposable, IEquatable<IState>
    {
        event Action<StateStatus> StatusChanged;

        StateStatus StateStatus { get; }

        /// <summary>
        ///     Gets whether the state is currently active.
        /// </summary>
        bool IsActive { get; }

        UniTask<bool> InitializeAsync([NotNull] StateDefinition stateDefinition,
            CancellationToken cancellationToken);
    }

    public interface IState<in TContext, in TPayload> : IState<TContext>
    {
    }

    /// <summary>
    ///     Represents a generic state in a state machine with context and payload.
    /// </summary>
    /// <typeparam name="TContext">The type of the context.</typeparam>
    public interface IState<in TContext> : IState, IEqualityComparer<IState>
    {
        /// <summary>
        ///     Enters the state asynchronously.
        /// </summary>
        /// <param name="context">The context for the state.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        UniTask<bool> EnterAsync([NotNull] TContext context, CancellationToken cancellationToken);

        /// <summary>
        ///     Exits the state asynchronously.
        /// </summary>
        /// <param name="context">The context for the state.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        UniTask<bool> ExitAsync([NotNull] TContext context, CancellationToken cancellationToken);
    }
}