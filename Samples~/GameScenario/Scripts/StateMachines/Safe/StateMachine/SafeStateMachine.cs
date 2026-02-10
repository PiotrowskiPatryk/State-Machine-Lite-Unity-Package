using System.Threading;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Abstraction;
using JetBrains.Annotations;
using Samples.GameScenario.Scripts.Safe;
using Samples.GameScenario.Scripts.StateMachines.Safe.Context;
using Samples.GameScenario.Scripts.StateMachines.Safe.Payload;
using Samples.GameScenario.Scripts.StateMachines.Safe.States;

namespace Samples.GameScenario.Scripts.StateMachines.Safe.StateMachine
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