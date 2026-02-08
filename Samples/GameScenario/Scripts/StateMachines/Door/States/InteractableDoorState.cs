using System.Threading;
using Cysharp.Threading.Tasks;
using Samples.GameScenario.Scripts.StateMachines.Door.Context;
using Samples.GameScenario.Scripts.StateMachines.Door.Payload;

namespace Samples.GameScenario.Scripts.StateMachines.Door.States
{
    public sealed class InteractableDoorState : DoorStateBase<InteractableDoorPayload>
    {
        private InteractableDoorPayload _payload;

        protected override UniTask<bool> DoInitializeAsync(InteractableDoorPayload payload,
            CancellationToken cancellationToken)
        {
            _payload = payload;

            return UniTask.FromResult(true);
        }

        protected override UniTask DoEnterAsync(DoorContext context, CancellationToken cancellationToken)
        {
            context.DoorController.SetInteractable();
            context.DoorController.Interacted += TriggerDoorOpened;

            return UniTask.CompletedTask;
        }

        protected override UniTask DoExitAsync(DoorContext context, CancellationToken cancellationToken)
        {
            context.DoorController.Interacted -= TriggerDoorOpened;

            return UniTask.CompletedTask;
        }

        private void TriggerDoorOpened()
        {
            _payload.DoorInteractedTrigger.ResolveReferenceAsync(CancellationToken.None).
                ContinueWith(trigger => trigger.TriggerValueAsync(true, CancellationToken.None)).Forget();
        }
    }
}