using System.Threading;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Abstraction;
using Dev.Cortez.StateMachines.Core.Data;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Samples.GameScenario.Scripts.StateMachines.Door.Context;

namespace Samples.GameScenario.Scripts.StateMachines.Door.States
{
    public abstract class DoorStateBase : DoorStateBase<EmptyPayload>
    {
        protected override UniTask<bool> DoInitializeAsync(EmptyPayload payload,
            CancellationToken cancellationToken)
        {
            return UniTask.FromResult(true);
        }
    }

    public abstract class DoorStateBase<TStatePayload> : StateBase<DoorContext, TStatePayload>, IDoorState
        where TStatePayload : IPayload
    {
    }
}