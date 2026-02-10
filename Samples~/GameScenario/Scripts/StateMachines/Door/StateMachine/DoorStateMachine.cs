using System.Threading;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Abstraction;
using Dev.Cortez.StateMachines.Core.Data;
using JetBrains.Annotations;
using Samples.GameScenario.Scripts.Door;
using Samples.GameScenario.Scripts.StateMachines.Door.Context;
using Samples.GameScenario.Scripts.StateMachines.Door.States;

namespace Samples.GameScenario.Scripts.StateMachines.Door.StateMachine
{
    [UsedImplicitly]
    public class DoorStateMachine : StateMachineBase<IDoorState, DoorContext>
    {
        private DoorContext _doorContext;

        protected override DoorContext StateContext => _doorContext;

        protected override UniTask<bool> DoInitializeStateMachineAsync(EmptyPayload stateMachinePayload,
            CancellationToken cancellationToken)
        {
            _doorContext = new DoorContext(DoorController.Instance);

            return UniTask.FromResult(true);
        }
    }
}