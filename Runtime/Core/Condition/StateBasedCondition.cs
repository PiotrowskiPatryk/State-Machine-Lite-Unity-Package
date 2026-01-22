using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Abstraction;
using Dev.Cortez.StateMachines.Core.Condition.Payload;
using Dev.Cortez.StateMachines.Core.Data;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Logging;
using JetBrains.Annotations;

namespace Dev.Cortez.StateMachines.Core.Condition
{
    [UsedImplicitly]
    public sealed class StateBasedCondition : ConditionBase<StateBasedConditionPayload>
    {
        private IState _targetState;
        private StateSatisfiedConditionType _stateSatisfiedConditionType;

        public override bool IsSatisfied => _stateSatisfiedConditionType.IsValidStatement(_targetState.StateStatus);

        public override ValueTask DisposeAsync()
        {
            DisposeStateEventListener();

            return base.DisposeAsync();
        }

        protected override UniTask<bool> InitializeAsync(StateBasedConditionPayload payload,
            CancellationToken cancellationToken)
        {
            if (!payload.StateReferencePicker.IsReferenceSelected)
            {
                LoggerService.Logger.LogError(
                    "Unable to initialize StateBasedCondition. Provided state reference picker is not provided.");

                return UniTask.FromResult(false);
            }

            _stateSatisfiedConditionType = payload.StateSatisfiedConditionType;
            payload.StateReferencePicker.Observe(OnResolvedState, OnUnresolvedState, DisposalCancellationToken);

            return UniTask.FromResult(true);
        }

        private void DisposeStateEventListener()
        {
            if (_targetState == null)
            {
                return;
            }

            _targetState.StatusChanged -= OnStateStatusChanged;
            _targetState = null;
        }

        private void OnUnresolvedState()
        {
            DisposeStateEventListener();
        }

        private void OnResolvedState(IState state)
        {
            _targetState = state;
            _targetState.StatusChanged += OnStateStatusChanged;
        }

        private void OnStateStatusChanged(StateStatus _)
        {
            PublishSatisfiedChangedEvent(IsSatisfied);
        }
    }
}