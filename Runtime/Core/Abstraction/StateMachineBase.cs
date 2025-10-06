using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Data;
using JetBrains.Annotations;

namespace Dev.Cortez.StateMachines.Core.Abstraction
{
    public abstract class StateMachineBase<TState> : StateMachineBase<TState, EmptyContext>
        where TState : IState<EmptyContext>
    {
    }

    public abstract class
        StateMachineBase<TState, TStateContext> : StateMachineBase<TState, DefaultStateMachinePayload, TStateContext>
        where TState : IState<TStateContext>
        where TStateContext : class
    {
    }

    public abstract class StateMachineBase<TState, TStateMachinePayload, TStateContext> : IStateMachine
        where TState : IState<TStateContext>
        where TStateMachinePayload : IStateMachinePayload
        where TStateContext : class
    {
        private readonly List<TState> _states = new();
        private ITransitionSolver _transitionSolver;
        private TState _defaultState;

        public string Id { get; private set; }
        public bool IsActive { get; private set; }

        public IState ActiveState { get; }

        public InitializationStatus InitializationStatus { get; private set; } = InitializationStatus.NotInitialized;

        protected abstract TStateContext StateContext { get; }

        public async UniTask<bool> ActivateAsync(CancellationToken cancellationToken)
        {
            using var linkedCancellationTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

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
            var properlyEnteredState = await EnterStateAsync(_defaultState, linkedCancellationTokenSource.Token);

            if (activatedSuccessfully)
            {
                IsActive = true;
            }

            return activatedSuccessfully;
        }

        public async UniTask<bool> DeactivateAsync(CancellationToken cancellationToken)
        {
            using var linkedCancellationTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            if (!IsActive)
            {
                return false;
            }

            var canDeactivate = await CanDeactivateAsync(linkedCancellationTokenSource.Token);

            if (!canDeactivate)
            {
                return false;
            }

            var deactivatedSuccessfully = await DoDeactivateAsync(linkedCancellationTokenSource.Token);

            if (deactivatedSuccessfully)
            {
                IsActive = false;
            }

            return deactivatedSuccessfully;
        }

        public ValueTask DisposeAsync()
        {
            return default;
        }

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

        protected virtual UniTask<bool> DoActivateAsync(CancellationToken cancellationToken)
        {
            return UniTask.FromResult(true);
        }

        protected virtual UniTask<bool> CanActivateAsync(CancellationToken cancellationToken)
        {
            return UniTask.FromResult(!IsActive);
        }

        protected virtual UniTask<bool> CanDeactivateAsync(CancellationToken cancellationToken)
        {
            return UniTask.FromResult(IsActive);
        }

        protected virtual UniTask<bool> DoInitializeStateMachineAsync(TStateMachinePayload stateMachinePayload,
            CancellationToken cancellationToken)
        {
            return UniTask.FromResult(true);
        }

        protected virtual UniTask<bool> DoDeactivateAsync(CancellationToken cancellationToken)
        {
            return UniTask.FromResult(true);
        }

        protected virtual ValueTask DoDisposeAsync()
        {
            return default;
        }

        private UniTask<bool> EnterStateAsync([NotNull] TState state, CancellationToken cancellationToken)
        {
            return state.EnterAsync(StateContext, cancellationToken);
        }

        private UniTask<bool> InitializeAsync(TStateMachinePayload stateMachinePayload,
            CancellationToken cancellationToken)
        {
            InitializationStatus = InitializationStatus.Initializing;

            ApplyVariables(stateMachinePayload);

            return DoInitializeStateMachineAsync(stateMachinePayload, cancellationToken);
        }

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
            _transitionSolver = stateMachinePayload.TransitionSolver;
        }
    }
}