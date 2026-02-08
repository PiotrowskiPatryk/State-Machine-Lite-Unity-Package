using System.Threading;
using Cysharp.Threading.Tasks;
using Samples.GameScenario.Scripts.StateMachines.Door.Context;

namespace Samples.GameScenario.Scripts.StateMachines.Door.States
{
    public sealed class NonInteractableDoorState : DoorStateBase
    {
        protected override UniTask DoEnterAsync(DoorContext context, CancellationToken cancellationToken)
        {
            context.DoorController.SetNotInteractable();

            return UniTask.CompletedTask;
        }

        protected override UniTask DoExitAsync(DoorContext context, CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }
    }
}