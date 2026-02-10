using System.Threading;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Abstraction;
using JetBrains.Annotations;

namespace Samples.GameScenario.Scripts.StateMachines.Safe
{
    [UsedImplicitly]
    public sealed class SafeStateMachine : StateMachineBase<ISafeState, SafePayload, StateContext>
    {
        private StateContext _stateContext;

        protected override StateContext StateContext => _stateContext;

        protected override UniTask<bool> DoInitializeStateMachineAsync(SafePayload stateMachinePayload,
            CancellationToken cancellationToken)
        {
            _stateContext = new StateContext(SafeController.Instance, stateMachinePayload.OpenSafeSequence);

            return UniTask.FromResult(true);
        }
    }
}