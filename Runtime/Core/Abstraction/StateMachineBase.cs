using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Data;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Logging;
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
        : StateMachineBase<TState, EmptyPayload, TStateContext>
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
    ///     <see cref="IPayload" />.
    /// </typeparam>
    public abstract class StateMachineBase<TState, TStateMachinePayload, TStateContext> : IStateMachine
        where TState : IState<TStateContext>
        where TStateMachinePayload : IPayload
        where TStateContext : class
    {
        private readonly List<TState> _states = new();
        private readonly SemaphoreSlim _lifecycleLock = new(1, 1);

        private TState _activeState;
        private TState _defaultState;
        private CancellationTokenSource _transitionCts;

        /// <summary>
        ///     Gets the unique identifier of the state machine.
        ///     This property is used to identify and distinguish state machines.
        /// </summary>
        [NotNull]
        public string Id { get; private set; }

        public string Name { get; private set; }

        public string Description { get; private set; }

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
        [CanBeNull]
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
        [NotNull]
        protected abstract TStateContext StateContext { get; }

        /// <summary>
        ///     Provides access to the collection of states managed by the state machine.
        /// </summary>
        /// <remarks>
        ///     This property is a read-only list that contains all registered states within the state machine.
        ///     It is useful for inspecting or debugging the state machine's configuration.
        /// </remarks>
        [NotNull]
        protected IReadOnlyList<TState> States => _states;

        /// <summary>
        ///     A property representing the transition solver used within the state machine.
        ///     The <see cref="ITransitionSolver" /> is responsible for managing and applying
        ///     transition rules between states based on the provided configuration.
        /// </summary>
        [NotNull]
        protected ITransitionSolver TransitionSolver { get; private set; }

        /// Checks whether the state machine can be activated
        /// <return>
        ///     Boolean indicating whether the state machine can be activated.
        /// </return>
        protected virtual bool CanActivate => true;

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
                LoggerService.Logger.LogTrace("Activating state machine");

                if (IsActive)
                {
                    LoggerService.Logger.LogWarning("State machine is already active. Skipping activation.");

                    return false;
                }

                if (!CanActivate)
                {
                    LoggerService.Logger.LogError("State machine cannot be activated. Skipping activation.");

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

                var properlyEnteredState = await MoveToStateAsync(_defaultState, linkedCancellationTokenSource.Token);

                if (activatedSuccessfully && properlyEnteredState)
                {
                    LoggerService.Logger.LogInfo(
                        $"State machine {Name} successfully activated and properly entered default state.");

                    IsActive = true;
                    _activeState = _defaultState;

                    return true;
                }
                
                LoggerService.Logger.LogError($"State machine {Name} failed to activate.");

                return false;
            }
            finally
            {
                _lifecycleLock.Release();
            }
        }

        public async UniTask<bool> MoveToStateAsync(IState state, CancellationToken cancellationToken)
        {
            using var linkedCancellationTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            LoggerService.Logger.LogInfo($"Moving to state: {state.GetType().Name}");

            if (state is not TState targetState)
            {
                LoggerService.Logger.LogError(
                    $"State machine cannot move to state. State is not of the correct type. Expected state of type {typeof(TState).Name}, but got {state.GetType().Name}.");

                return false;
            }
            
            var previousState = _activeState;

            if (previousState != null)
            {
                var exitSucceeded = await ExitStateAsync(previousState, linkedCancellationTokenSource.Token);

                if (!exitSucceeded)
                {
                    LoggerService.Logger.LogError("Unable to exit previous state. Exiting state failed.");

                    return false;
                }
            }

            var enterSucceeded = await EnterStateAsync(targetState, linkedCancellationTokenSource.Token);

            if (!enterSucceeded)
            {
                LoggerService.Logger.LogError("Unable to enter target state. Entering state failed.");

                return false;
            }

            _activeState = targetState;

            return true;
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

            LoggerService.Logger.LogInfo("Deactivating state machine");

            await _lifecycleLock.WaitAsync(linkedCancellationTokenSource.Token).ConfigureAwait(false);

            try
            {
                if (!IsActive)
                {
                    LoggerService.Logger.LogWarning("State machine is not active. Skipping deactivation.");

                    return false;
                }

                var canDeactivate = await CanDeactivateAsync(linkedCancellationTokenSource.Token);

                if (!canDeactivate)
                {
                    LoggerService.Logger.LogError("State machine cannot be deactivated. Skipping deactivation.");

                    return false;
                }

                var exitSucceeded = true;

                if (_activeState != null)
                {
                    exitSucceeded = await ExitStateAsync(_activeState, linkedCancellationTokenSource.Token);
                }

                if (!exitSucceeded)
                {
                    LoggerService.Logger.LogError(
                        "State machine deactivation. Unable to exit current state. Exiting state failed.");

                    return false;
                }

                var deactivatedSuccessfully = await DoDeactivateAsync(linkedCancellationTokenSource.Token);

                if (deactivatedSuccessfully)
                {
                    LoggerService.Logger.LogInfo("State machine deactivated successfully.");

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

        public async ValueTask DisposeAsync()
        {
            LoggerService.Logger.LogInfo("Disposing state machine");

            await _lifecycleLock.WaitAsync().ConfigureAwait(false);

            try
            {
                await DoDisposeAsync();
                _states.Clear();
                _activeState = default;
                IsActive = false;
                InitializationStatus = InitializationStatus.NotInitialized;
                _defaultState = default;

                // Cancel and dispose in-flight transition if any.
                var cts = Interlocked.Exchange(ref _transitionCts, null);

                if (cts != null)
                {
                    try
                    {
                        cts.Cancel();
                    }
                    finally
                    {
                        cts.Dispose();
                    }
                }

                TransitionSolver.TransitionRuleApplied -= OnTransitionRuleApplied;
                TransitionSolver = null!;
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
        /// <param name="stateMachineSettings"></param>
        /// <param name="cancellationToken">
        ///     A CancellationToken that can be used to cancel the activation operation while it is in progress.
        /// </param>
        /// <returns>A UniTask that resolves to a boolean indicating whether initialization was successful.</returns>
        public async UniTask<bool> InitializeAsync(StateMachineSettings stateMachineSettings,
            CancellationToken cancellationToken)
        {
            using var linkedCancellationTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            LoggerService.Logger.LogInfo("Initializing state machine");

            if (stateMachineSettings.StateMachineDefinition.Payload is TStateMachinePayload stateMachinePayload)
            {
                var initializationResult =
                    await InitializeAsync(stateMachineSettings, stateMachinePayload,
                        linkedCancellationTokenSource.Token);

                InitializationStatus =
                    !initializationResult ? InitializationStatus.Failed : InitializationStatus.Initialized;

                return initializationResult;
            }

            LoggerService.Logger.LogError(
                $"Unable to initialize state machine. Invalid payload type. Expected payload of type {typeof(TStateMachinePayload).Name}, but got {stateMachineSettings.StateMachineDefinition?.Payload.GetType().Name}.");

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

        private UniTask<bool> EnterStateAsync([NotNull] TState state, CancellationToken cancellationToken)
        {
            return state.EnterAsync(StateContext, cancellationToken);
        }

        private UniTask<bool> ExitStateAsync([NotNull] TState state, CancellationToken cancellationToken)
        {
            return state.ExitAsync(StateContext, cancellationToken);
        }

        private async UniTask<bool> InitializeAsync(
            StateMachineSettings stateMachineSettings,
            TStateMachinePayload stateMachinePayload,
            CancellationToken cancellationToken)
        {
            await _lifecycleLock.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                InitializationStatus = InitializationStatus.Initializing;

                ApplyVariables(stateMachineSettings);
                InitializeTransitionSolver(stateMachineSettings.TransitionSolver);

                var initResult = await DoInitializeStateMachineAsync(stateMachinePayload, cancellationToken);

                InitializationStatus = initResult ? InitializationStatus.Initialized : InitializationStatus.Failed;

                return initResult;
            }
            finally
            {
                _lifecycleLock.Release();
            }
        }

        private void InitializeTransitionSolver([NotNull] ITransitionSolver transitionSolver)
        {
            transitionSolver.TransitionRuleApplied += OnTransitionRuleApplied;
        }

        private void ApplyVariables(StateMachineSettings stateMachineSettings)
        {
            Id = stateMachineSettings.StateMachineDefinition.Id;
            Name = stateMachineSettings.StateMachineDefinition.Name;
            Description = stateMachineSettings.StateMachineDefinition.Description;

            _states.Clear();

            foreach (var state in stateMachineSettings.States)
            {
                if (state is TState typedState)
                {
                    _states.Add(typedState);
                }
            }

            _defaultState = stateMachineSettings.InitialState is TState tState ? tState : default;
            TransitionSolver = stateMachineSettings.TransitionSolver;
        }

        private void OnTransitionRuleApplied(TransitionRule transitionRule)
        {
            if (transitionRule.TargetState is not TState targetState)
            {
                return;
            }

            MoveToStateAsync(targetState, CancellationToken.None).Forget();
        }
    }
}