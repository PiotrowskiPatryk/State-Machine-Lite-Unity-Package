using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Data;
using JetBrains.Annotations;

namespace Dev.Cortez.StateMachines.Core.Abstraction
{
    /// <summary>
    ///     An abstract base class representing a state machine with a single state type
    ///     and a default state context.
    /// </summary>
    /// <typeparam name="TState">
    ///     The type of state managed by the state machine. Must implement
    ///     <see cref="IState{EmptyContext}" />.
    /// </typeparam>
    /// <remarks>
    ///     This class provides a simplified state machine implementation where the context
    ///     is fixed as <see cref="EmptyContext" />. It internally defines the default context instance.
    /// </remarks>
    public abstract class StateMachineBase<TState> : StateMachineBase<TState, EmptyContext>
        where TState : IState<EmptyContext>
    {
        /// <summary>
        ///     Represents the context associated with the state machine.
        ///     This context is used to provide a shared state or data accessibility
        ///     among states within the state machine.
        /// </summary>
        /// <remarks>
        ///     The type of the context is determined by generic type parameters in the state machine base implementation.
        ///     In the default implementation, it represents an <see cref="EmptyContext" />, which is a placeholder with no
        ///     additional functionality.
        ///     This property is abstract and must be implemented or specified by derived classes.
        /// </remarks>
        protected override EmptyContext StateContext { get; } = EmptyContext.Default;
    }

    /// <summary>
    ///     Represents the base class for a generalized state machine implementation.
    /// </summary>
    /// <typeparam name="TState">The type of state, implementing the IState interface, that the state machine will handle.</typeparam>
    /// <typeparam name="TStateContext">
    ///     The type of the state context required during state transitions.
    ///     Must be a class type.
    /// </typeparam>
    public abstract class StateMachineBase<TState, TStateContext>
        : StateMachineBase<TState, DefaultStateMachinePayload, TStateContext>
        where TState : IState<TStateContext>
        where TStateContext : class
    {
    }

    /// <summary>
    ///     An abstract base class that represents a framework for creating state machines.
    ///     Provides generic functionality to manage states, transitions, and activation lifecycle.
    /// </summary>
    /// <typeparam name="TState">The type of state. Must implement the <see cref="IState{TStateContext}" /> interface.</typeparam>
    /// <typeparam name="TStateContext">The context type used by the state. Must be a reference type.</typeparam>
    /// <typeparam name="TStateMachinePayload">
    ///     The payload type used during initialization. Must implement
    ///     <see cref="IStateMachinePayload" />.
    /// </typeparam>
    public abstract class StateMachineBase<TState, TStateMachinePayload, TStateContext> : IStateMachine
        where TState : IState<TStateContext>
        where TStateMachinePayload : IStateMachinePayload
        where TStateContext : class
    {
        private readonly List<TState> _states = new();
        private readonly SemaphoreSlim _lifecycleLock = new(1, 1);

        private TState _activeState;
        private TState _defaultState;

        /// <summary>
        ///     Gets the unique identifier of the state machine.
        ///     This property is used to identify and distinguish state machines.
        /// </summary>
        public string Id { get; private set; }

        /// Represents the active state of the state machine.
        /// This property indicates whether the state machine is currently active.
        /// A state machine becomes active after a successful activation process, and it is deactivated
        /// either through an explicit deactivation operation or in scenarios such as disposal.
        /// Returns true if the state machine is active, otherwise returns false.
        public bool IsActive { get; private set; }

        /// <summary>
        ///     Gets the currently active state of the state machine.
        /// </summary>
        /// <remarks>
        ///     This property provides access to the instance of the state that is currently active
        ///     in the state machine. It returns an object that implements the <see cref="IState" />
        ///     interface, or null if no state is active. This property is useful for querying or
        ///     interacting with the current state of the state machine.
        /// </remarks>
        public IState ActiveState => _activeState;

        /// <summary>
        ///     Gets the current initialization status of the state machine.
        ///     This property reflects whether the state machine is ready for activation or has encountered issues during setup.
        /// </summary>
        public InitializationStatus InitializationStatus { get; private set; } = InitializationStatus.NotInitialized;

        /// <summary>
        ///     Gets the context object associated with the state machine, which defines the data or configuration
        ///     that is shared across states during transitions.
        /// </summary>
        /// <remarks>
        ///     This property is abstract and must be implemented in derived classes to provide the necessary context
        ///     object for the specific state machine implementation. The context can be used to pass state-specific
        ///     information or handles required for the transition logic between states.
        /// </remarks>
        protected abstract TStateContext StateContext { get; }

        /// <summary>
        ///     Provides access to the collection of states managed by the state machine.
        /// </summary>
        /// <remarks>
        ///     This property is a read-only list that contains all registered states within the state machine.
        ///     It is useful for inspecting or debugging the state machine's configuration.
        /// </remarks>
        protected IReadOnlyList<TState> States => _states;

        /// <summary>
        ///     A property representing the transition solver used within the state machine.
        ///     The <see cref="ITransitionSolver" /> is responsible for managing and applying
        ///     transition rules between states based on the provided configuration.
        /// </summary>
        protected ITransitionSolver TransitionSolver { get; private set; }

        /// Activates the state machine asynchronously, transitioning it to an operational state.
        /// This involves evaluating conditions necessary for activation and entering the default state if possible.
        /// If the state machine is already active, the method will terminate early with a result indicating no action performed.
        /// <param name="cancellationToken">
        ///     A CancellationToken that can be used to cancel the activation operation while it is in progress.
        /// </param>
        /// <return>
        ///     A UniTask containing a boolean value indicating whether the state machine was successfully activated (true)
        ///     or if activation could not proceed (false).
        /// </return>
        public async UniTask<bool> ActivateAsync(CancellationToken cancellationToken)
        {
            using var linkedCancellationTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            await _lifecycleLock.WaitAsync(linkedCancellationTokenSource.Token).ConfigureAwait(false);

            try
            {
                if (IsActive)
                {
                    return false;
                }

                var canActivate = await CanActivateAsync(linkedCancellationTokenSource.Token);

                if (!canActivate)
                {
                    return false;
                }

                var activatedSuccessfully = await DoActivateAsync(linkedCancellationTokenSource.Token);

                if (_defaultState == null)
                {
                    if (activatedSuccessfully)
                    {
                        await DoDeactivateAsync(linkedCancellationTokenSource.Token);
                    }

                    return false;
                }

                var properlyEnteredState = await EnterStateAsync(_defaultState, linkedCancellationTokenSource.Token);

                if (activatedSuccessfully && properlyEnteredState)
                {
                    IsActive = true;
                    _activeState = _defaultState;

                    return true;
                }

                if (activatedSuccessfully)
                {
                    await DoDeactivateAsync(linkedCancellationTokenSource.Token);
                }

                return false;
            }
            finally
            {
                _lifecycleLock.Release();
            }
        }

        /// Asynchronously deactivates the current state machine.
        /// Ensures that the state machine is stopped properly and all necessary cleanup is performed.
        /// This method is thread-safe and can handle concurrent calls.
        /// <param name="cancellationToken">
        ///     A CancellationToken that can be used to cancel the activation operation while it is in progress.
        /// </param>
        /// <returns>
        ///     A task that represents the asynchronous operation. The task result contains a boolean value:
        ///     true if the state machine was successfully deactivated; otherwise, false.
        /// </returns>
        public async UniTask<bool> DeactivateAsync(CancellationToken cancellationToken)
        {
            using var linkedCancellationTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            await _lifecycleLock.WaitAsync(linkedCancellationTokenSource.Token).ConfigureAwait(false);

            try
            {
                if (!IsActive)
                {
                    return false;
                }

                var canDeactivate = await CanDeactivateAsync(linkedCancellationTokenSource.Token);

                if (!canDeactivate)
                {
                    return false;
                }

                var exitSucceeded = true;

                if (_activeState != null)
                {
                    exitSucceeded = await ExitStateAsync(_activeState, linkedCancellationTokenSource.Token);
                }

                if (!exitSucceeded)
                {
                    return false;
                }

                var deactivatedSuccessfully = await DoDeactivateAsync(linkedCancellationTokenSource.Token);

                if (deactivatedSuccessfully)
                {
                    IsActive = false;
                    _activeState = default;
                }

                return deactivatedSuccessfully;
            }
            finally
            {
                _lifecycleLock.Release();
            }
        }

        /// Releases all resources used by the state machine asynchronously and resets its state.
        /// This includes clearing the states, resetting the active state, deactivating the state machine,
        /// and resetting initialization status. It ensures that resources like locks and transition solvers
        /// are disposed to avoid memory leaks.
        /// <returns>A task representing the asynchronous dispose operation.</returns>
        public async ValueTask DisposeAsync()
        {
            await _lifecycleLock.WaitAsync().ConfigureAwait(false);

            try
            {
                await DoDisposeAsync();
                _states.Clear();
                _activeState = default;
                IsActive = false;
                InitializationStatus = InitializationStatus.NotInitialized;
                _defaultState = default;
                TransitionSolver = null;
            }
            finally
            {
                _lifecycleLock.Release();
            }
        }

        /// <summary>
        ///     Asynchronously initializes the state machine using the provided payload and cancellation token.
        ///     Updates the state machine's initialization status based on the result.
        /// </summary>
        /// <param name="payload">The payload used for initialization.</param>
        /// <param name="cancellationToken">
        ///     A CancellationToken that can be used to cancel the activation operation while it is in progress.
        /// </param>
        /// <returns>A UniTask that resolves to a boolean indicating whether initialization was successful.</returns>
        public async UniTask<bool> InitializeAsync(IPayload payload, CancellationToken cancellationToken)
        {
            using var linkedCancellationTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            if (payload is TStateMachinePayload stateMachinePayload)
            {
                var initializationResult =
                    await InitializeAsync(stateMachinePayload, linkedCancellationTokenSource.Token);

                InitializationStatus =
                    !initializationResult ? InitializationStatus.Failed : InitializationStatus.Initialized;

                return initializationResult;
            }

            InitializationStatus = InitializationStatus.Failed;

            return false;
        }

        /// <summary>
        ///     Executes the custom activation logic for the state machine asynchronously.
        /// </summary>
        /// <param name="cancellationToken">
        ///     A CancellationToken that can be used to cancel the activation operation while it is in progress.
        /// </param>
        /// <returns>A task that resolves to a boolean indicating whether the activation was successful.</returns>
        protected virtual UniTask<bool> DoActivateAsync(CancellationToken cancellationToken)
        {
            return UniTask.FromResult(true);
        }

        /// Checks whether the state machine can be activated asynchronously.
        /// <param name="cancellationToken">
        ///     A CancellationToken that can be used to cancel the activation operation while it is in progress.
        /// </param>
        /// <return>
        ///     A task that represents the asynchronous operation. The task result is a boolean
        ///     indicating whether the state machine can be activated.
        /// </return>
        protected virtual UniTask<bool> CanActivateAsync(CancellationToken cancellationToken)
        {
            return UniTask.FromResult(!IsActive);
        }

        /// <summary>
        ///     Determines whether the state machine can be deactivated asynchronously.
        /// </summary>
        /// <param name="cancellationToken">
        ///     A CancellationToken that can be used to cancel the activation operation while it is in progress.
        /// </param>
        /// <returns>
        ///     A task that represents the asynchronous operation. The task result is a boolean value indicating whether the
        ///     state machine can be deactivated.
        /// </returns>
        protected virtual UniTask<bool> CanDeactivateAsync(CancellationToken cancellationToken)
        {
            return UniTask.FromResult(IsActive);
        }

        /// <summary>
        ///     Initializes the state machine asynchronously with the provided payload and cancellation token.
        ///     This method is a part of the internal initialization workflow for the state machine.
        /// </summary>
        /// <param name="stateMachinePayload">
        ///     The payload to be used for initializing the state machine. Must conform to the specified state machine payload
        ///     type.
        /// </param>
        /// <param name="cancellationToken">
        ///     A CancellationToken that can be used to cancel the activation operation while it is in progress.
        /// </param>
        /// <returns>
        ///     A task that represents the asynchronous initialization operation.
        ///     The task result contains <c>true</c> if the initialization was successful; otherwise, <c>false</c>.
        /// </returns>
        protected virtual UniTask<bool> DoInitializeStateMachineAsync(TStateMachinePayload stateMachinePayload,
            CancellationToken cancellationToken)
        {
            return UniTask.FromResult(true);
        }

        /// Performs the deactivation process for the state machine asynchronously.
        /// <param name="cancellationToken">
        ///     A CancellationToken that can be used to cancel the activation operation while it is in progress.
        /// </param>
        /// <returns>
        ///     A UniTask that represents the asynchronous deactivation operation.
        ///     The task result indicates whether the deactivation succeeded (true) or not (false).
        /// </returns>
        protected virtual UniTask<bool> DoDeactivateAsync(CancellationToken cancellationToken)
        {
            return UniTask.FromResult(true);
        }

        /// Performs custom asynchronous disposal logic for the state machine.
        /// This method is called during the disposal process to clean up resources and states specific to the derived implementation.
        /// It is intended to be overridden by subclass implementations if additional asynchronous disposal steps are required.
        /// <return>
        ///     A ValueTask representing the asynchronous operation of the disposal process.
        ///     If no asynchronous disposal logic is necessary, this method can return a completed ValueTask.
        /// </return>
        protected virtual ValueTask DoDisposeAsync()
        {
            return default;
        }

        /// Attempts to enter the specified state asynchronously within the state machine.
        /// The method invokes the `EnterAsync` method of the given state, utilizing the state machine's context.
        /// <param name="state">
        ///     The target state to transition into. Must not be null.
        /// </param>
        /// <param name="cancellationToken">
        ///     A CancellationToken that can be used to cancel the activation operation while it is in progress.
        /// </param>
        /// <return>
        ///     A task that represents the asynchronous operation. The result indicates whether the state was successfully entered.
        /// </return>
        private UniTask<bool> EnterStateAsync([NotNull] TState state, CancellationToken cancellationToken)
        {
            return state.EnterAsync(StateContext, cancellationToken);
        }

        /// Exits the specified state asynchronously, invoking the state's exit logic with the provided context and cancellation token.
        /// <param name="state">The state to exit. Must not be null.</param>
        /// <param name="cancellationToken">
        ///     A CancellationToken that can be used to cancel the activation operation while it is in progress.
        /// </param>
        /// <returns>
        ///     A task that represents the asynchronous operation, containing a boolean value indicating whether the state
        ///     exited successfully.
        /// </returns>
        private UniTask<bool> ExitStateAsync([NotNull] TState state, CancellationToken cancellationToken)
        {
            return state.ExitAsync(StateContext, cancellationToken);
        }

        /// Initializes the state machine asynchronously.
        /// <param name="stateMachinePayload">
        ///     The payload containing initialization data for the state machine.
        /// </param>
        /// <param name="cancellationToken">
        ///     A CancellationToken that can be used to cancel the activation operation while it is in progress.
        /// </param>
        /// <returns>
        ///     A UniTask that resolves to a boolean indicating whether the initialization was successful or not.
        /// </returns>
        private async UniTask<bool> InitializeAsync(TStateMachinePayload stateMachinePayload,
            CancellationToken cancellationToken)
        {
            await _lifecycleLock.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                InitializationStatus = InitializationStatus.Initializing;

                ApplyVariables(stateMachinePayload);

                var initResult = await DoInitializeStateMachineAsync(stateMachinePayload, cancellationToken);

                InitializationStatus = initResult ? InitializationStatus.Initialized : InitializationStatus.Failed;

                return initResult;
            }
            finally
            {
                _lifecycleLock.Release();
            }
        }

        /// <summary>
        ///     Applies the variables from the provided state machine payload.
        /// </summary>
        /// <param name="stateMachinePayload">
        ///     The payload containing initialization data, including states, initial state, state
        ///     machine ID, and the transition solver.
        /// </param>
        private void ApplyVariables(TStateMachinePayload stateMachinePayload)
        {
            Id = stateMachinePayload.StateMachineId;

            _states.Clear();

            foreach (var state in stateMachinePayload.States)
            {
                if (state is TState typedState)
                {
                    _states.Add(typedState);
                }
            }

            _defaultState = stateMachinePayload.InitialState is TState tState ? tState : default;
            TransitionSolver = stateMachinePayload.TransitionSolver;
        }
    }
}