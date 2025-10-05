using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Data;

namespace Dev.Cortez.StateMachines.Core.Abstraction
{
    public abstract class StateMachineBase<TState, TStateMachinePayload, TStateContext, TStatePayload> : IStateMachine
        where TState : IState<TStateContext, TStatePayload>
        where TStateMachinePayload : IStateMachinePayload
        where TStatePayload : IPayload
    {
        private readonly List<TState> _states = new();
        private ITransitionSolver _transitionSolver;

        public string Id { get; private set; }
        public bool IsActive { get; } = false;

        public IState ActiveState { get; }

        public InitializationStatus InitializationStatus { get; private set; }

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

            var activationResult = await DoActivateAsync(linkedCancellationTokenSource.Token);

            return activationResult;
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

            var deactivationResult = await DoDeactivateAsync(linkedCancellationTokenSource.Token);

            return deactivationResult;
        }

        public ValueTask DisposeAsync()
        {
            return default;
        }

        public async UniTask<bool> InitializeAsync(IPayload payload, CancellationToken cancellationToken)
        {
            using var linkedCancellationTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            InitializationStatus = InitializationStatus.Initializing;

            var stateMachineInitializationResult =
                await InitializeStateMachineAsync(payload, linkedCancellationTokenSource.Token);

            if (!stateMachineInitializationResult)
            {
                InitializationStatus = InitializationStatus.Failed;

                return false;
            }

            var statesInitializationResult = await InitializeStatesAsync(payload, linkedCancellationTokenSource.Token);

            if (!statesInitializationResult)
            {
                InitializationStatus = InitializationStatus.Failed;

                return false;
            }

            return true;
        }

        protected virtual UniTask<bool> DoActivateAsync(CancellationToken cancellationToken)
        {
            return UniTask.FromResult(true);
        }

        protected virtual UniTask<bool> CanActivateAsync(CancellationToken cancellationToken)
        {
            return UniTask.FromResult(true);
        }

        protected virtual UniTask<bool> CanDeactivateAsync(CancellationToken cancellationToken)
        {
            return UniTask.FromResult(true);
        }

        protected virtual UniTask<bool> InitializeStateMachineAsync(IPayload payload,
            CancellationToken cancellationToken)
        {
            if (payload is not TStateMachinePayload stateMachinePayload)
            {
                return UniTask.FromResult(false);
            }

            Id = stateMachinePayload.StateMachineId;
            _states.Clear();

            foreach (var state in stateMachinePayload.States)
            {
                if (state is TState typedState)
                {
                    _states.Add(typedState);
                }
            }

            _transitionSolver = stateMachinePayload.TransitionSolver;

            return DoInitializeStateMachineAsync(stateMachinePayload, cancellationToken);
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

        private async UniTask<bool> InitializeStatesAsync(IPayload payload, CancellationToken cancellationToken)
        {
            if (payload is not TStateMachinePayload stateMachinePayload)
            {
                return false;
            }

            if (stateMachinePayload.StatePayload is not TStatePayload statePayload)
            {
                return false;
            }

            var states = await stateMachinePayload.States.Where(state => state is TState typedState).
                Select(state => state.InitializeAsync(statePayload, cancellationToken));

            return states.All(state => state);
        }
    }
}