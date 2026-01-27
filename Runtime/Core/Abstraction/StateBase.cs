using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Data;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Core.StateMachineConfiguration.Definition;
using Dev.Cortez.StateMachines.Logging;
using JetBrains.Annotations;

namespace Dev.Cortez.StateMachines.Core.Abstraction
{
    /// <summary>
    ///     Represents the base class for creating a state in a state machine.
    ///     It is implemented with an empty context and default payload.
    /// </summary>
    /// <remarks>
    ///     This abstract class serves as a foundation for creating specific state implementations.
    ///     Derived classes must provide implementations for entering and exiting the state.
    /// </remarks>
    /// <example>
    ///     Derived classes such as <c>SimpleStateBase</c> extend this class, defining state-specific behavior.
    /// </example>
    public abstract class StateBase : StateBase<EmptyContext>
    {
    }

    /// <summary>
    ///     Represents the base class for defining a state within a state machine.
    /// </summary>
    /// <remarks>
    ///     StateBase serves as a simplified entry point for creating state machine states using the default context type
    ///     <see cref="EmptyContext" />
    ///     and default state payload type <see cref="EmptyPayload" />.
    ///     It abstracts key lifecycle operations such as initialization, entering, and exiting a state.
    /// </remarks>
    public abstract class StateBase<TStateContext> : StateBase<TStateContext, EmptyPayload>
    {
        protected override UniTask<bool> DoInitializeAsync(EmptyPayload uiStatePayload,
            CancellationToken cancellationToken)
        {
            return UniTask.FromResult(true);
        }
    }

    /// <summary>
    ///     Represents the base implementation of a state within a state machine.
    ///     This abstract class provides common functionality for managing the
    ///     lifecycle of a state, including initialization, activation, deactivation,
    ///     and disposal.
    /// </summary>
    public abstract class StateBase<TStateContext, TStatePayload> : IState<TStateContext, TStatePayload>
        where TStatePayload : IPayload
    {
        /// <summary>
        ///     An event that is triggered whenever the status of the state changes.
        /// </summary>
        /// <remarks>
        ///     The event provides the new <see cref="StateStatus" /> value upon state status updates.
        ///     It allows external systems to react to state transitions dynamically.
        /// </remarks>
        public event Action<StateStatus> StatusChanged;

        private StateStatus _stateStatus;

        /// <summary>
        ///     Represents the current status of the state within the state machine.
        /// </summary>
        public StateStatus StateStatus
        {
            get => _stateStatus;
            private set
            {
                _stateStatus = value;
                StatusChanged?.Invoke(value);
            }
        }

        /// <summary>
        ///     Represents the current initialization status of a state.
        /// </summary>
        /// <remarks>
        ///     This status is updated during state initialization processes to reflect the current state lifecycle.
        /// </remarks>
        /// <value>
        ///     A value of the <see cref="Dev.Cortez.StateMachines.Core.Data.InitializationStatus" /> enumeration indicating the
        ///     initialization state.
        /// </value>
        /// <seealso cref="Dev.Cortez.StateMachines.Core.Data.InitializationStatus" />
        public InitializationStatus InitializationStatus { get; private set; } = InitializationStatus.NotInitialized;

        /// <summary>
        ///     Indicates whether the state is currently active within the state machine.
        /// </summary>
        /// <remarks>
        ///     This property returns true if the state is active, meaning it has successfully
        ///     entered and is currently in use. Returns false if the state is inactive, either
        ///     because it has not been activated yet, or it has been exited and is no longer active.
        ///     Changing the state status directly updates this value to reflect the current activity state.
        /// </remarks>
        public bool IsActive { get; private set; }

        /// <summary>
        ///     Gets the unique identifier for the state.
        /// </summary>
        [NotNull]
        public string Id { get; private set; }

        public string Name { get; private set; }
        public string Description { get; private set; }

        [CanBeNull]
        protected TStatePayload Payload { get; private set; }

        /// <summary>
        ///     Asynchronously initializes the state using the provided payload instance and cancellation token.
        /// </summary>
        /// <param name="stateDefinition"></param>
        /// <param name="cancellationToken">
        ///     The cancellation token used to propagate notifications if the operation should be
        ///     canceled.
        /// </param>
        /// <returns>A task that resolves to a boolean indicating whether the initialization was successful or failed.</returns>
        public UniTask<bool> InitializeAsync(StateDefinition stateDefinition,
            CancellationToken cancellationToken)
        {
            if (stateDefinition.Payload is TStatePayload statePayload)
            {
                return InitializeAsyncInternal(stateDefinition, statePayload, cancellationToken);
            }

            InitializationStatus = InitializationStatus.Failed;
            Id = stateDefinition.Id;

            LoggerService.Logger.LogError(
                $"Unable to initialize state. Provided payload is not of the expected type. Expected payload of type {typeof(TStatePayload).Name}, but got {stateDefinition.Payload?.GetType().Name}.");

            return UniTask.FromResult(false);
        }

        /// <summary>
        ///     Performs an asynchronous operation to transition the state to an "active" status.
        /// </summary>
        /// <param name="context">The context required by the state during the activation process.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>
        ///     A task that resolves to a boolean value indicating whether the state transition to the "active" status was
        ///     successful.
        /// </returns>
        public async UniTask<bool> EnterAsync(TStateContext context, CancellationToken cancellationToken)
        {
            using var linkedCancellationTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            if (IsActive)
            {
                return false;
            }

            StateStatus = StateStatus.Activating;
            await DoEnterAsync(context, linkedCancellationTokenSource.Token);
            StateStatus = StateStatus.Active;

            IsActive = true;

            return true;
        }

        /// <summary>
        ///     Exits the current state asynchronously, deactivating it and updating its status.
        /// </summary>
        /// <param name="context">The context associated with the current state.</param>
        /// <param name="cancellationToken">A token that can be used to cancel the exit operation.</param>
        /// <returns>A UniTask containing a boolean indicating whether the exit was successful.</returns>
        public async UniTask<bool> ExitAsync(TStateContext context, CancellationToken cancellationToken)
        {
            using var linkedCancellationTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            if (!IsActive)
            {
                return false;
            }

            StateStatus = StateStatus.Deactivating;
            await DoExitAsync(context, linkedCancellationTokenSource.Token);
            StateStatus = StateStatus.Inactive;
            IsActive = false;

            return true;
        }

        /// Releases resources asynchronously used by the state.
        /// This method is typically overridden to include custom cleanup logic for derived classes.
        /// <return>The task representing the asynchronous dispose operation.</return>
        public ValueTask DisposeAsync()
        {
            StatusChanged = null;

            return DoDisposeAsync();
        }

        public bool Equals(IState other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (other is null)
            {
                return false;
            }

            var rightId = other.Id;

            return StringComparer.Ordinal.Equals(Id, rightId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj is IState state && Equals(state);
        }

        public override int GetHashCode()
        {
            return StringComparer.Ordinal.GetHashCode(Id);
        }

        public static bool operator ==(StateBase<TStateContext, TStatePayload> left,
            StateBase<TStateContext, TStatePayload> right)
        {
            return left?.Equals(right) == true;
        }

        public static bool operator !=(StateBase<TStateContext, TStatePayload> left,
            StateBase<TStateContext, TStatePayload> right)
        {
            return left?.Equals(right) == false;
        }

        public bool Equals(IState x, IState y)
        {
            return x?.Id.Equals(y?.Id) == true;
        }

        public int GetHashCode(IState state)
        {
            return state.Id.GetHashCode();
        }

        /// Executes the logic required when entering the state asynchronously.
        /// <param name="context">The context of the state which provides data or functionality necessary for entering this state.</param>
        /// <param name="cancellationToken">A token used to cancel the enter operation if required.</param>
        /// <returns>A UniTask representing the asynchronous operation of entering the state.</returns>
        protected abstract UniTask DoEnterAsync(TStateContext context, CancellationToken cancellationToken);

        /// Performs the exit logic for the current state asynchronously. This method is
        /// intended to be implemented by derived classes to provide state-specific
        /// cleanup or transition functionality.
        /// <param name="context">
        ///     The context used during the exit operation, providing necessary details to execute the exit
        ///     logic.
        /// </param>
        /// <param name="cancellationToken">Token used to signal cancellation of the operation.</param>
        /// <return>Returns a UniTask that represents the asynchronous exit operation.</return>
        protected abstract UniTask DoExitAsync(TStateContext context, CancellationToken cancellationToken);

        /// Performs the disposal process for the current state asynchronously.
        /// Override this method to implement custom disposal logic.
        /// <return>Returns a ValueTask representing the asynchronous disposal operation.</return>
        protected virtual ValueTask DoDisposeAsync()
        {
            return default;
        }

        /// <summary>
        ///     Performs the initialization of the state with the given payload and cancellation token.
        /// </summary>
        /// <param name="uiStatePayload">The payload required for the state initialization.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>
        ///     A UniTask representing the asynchronous initialization operation. Returns true if initialization is
        ///     successful; otherwise, false.
        /// </returns>
        protected abstract UniTask<bool> DoInitializeAsync(TStatePayload uiStatePayload,
            CancellationToken cancellationToken);

        private async UniTask<bool> InitializeAsyncInternal(StateDefinition stateDefinition, TStatePayload payload,
            CancellationToken cancellationToken)
        {
            using var linkedCancellationTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            Id = stateDefinition.Id;
            Name = stateDefinition.Name;
            Description = stateDefinition.Description;
            Payload = payload;

            switch (InitializationStatus)
            {
                case InitializationStatus.Initialized:
                    return true;
                case InitializationStatus.Failed:
                    return false;
            }

            var initializationResult = await DoInitializeAsync(payload, linkedCancellationTokenSource.Token);

            if (!initializationResult)
            {
                InitializationStatus = InitializationStatus.Failed;

                return false;
            }

            InitializationStatus = InitializationStatus.Initialized;
            StateStatus = StateStatus.Inactive;

            return true;
        }
    }
}