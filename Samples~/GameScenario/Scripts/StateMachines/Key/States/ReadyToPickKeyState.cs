using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Samples.GameScenario.Scripts.StateMachines.Key.Context;
using Samples.GameScenario.Scripts.StateMachines.Key.Payload;

namespace Samples.GameScenario.Scripts.StateMachines.Key.States
{
    [UsedImplicitly]
    public sealed class ReadyToPickKeyState : KeyStateBase<ReadyToPickKeyStatePayload>
    {
        private ReadyToPickKeyStatePayload _payload;

        protected override UniTask<bool> DoInitializeAsync(ReadyToPickKeyStatePayload payload,
            CancellationToken cancellationToken)
        {
            _payload = payload;

            return UniTask.FromResult(true);
        }

        protected override UniTask DoEnterAsync(KeyContext context, CancellationToken cancellationToken)
        {
            context.KeyController.SetGrabbable();

            context.KeyController.Grabbed += TriggerGrabKey;

            return UniTask.CompletedTask;
        }

        protected override UniTask DoExitAsync(KeyContext context, CancellationToken cancellationToken)
        {
            context.KeyController.Grabbed -= TriggerGrabKey;

            return UniTask.CompletedTask;
        }

        private void TriggerGrabKey()
        {
            _payload.KeyPickedTrigger.ResolveReferenceAsync(CancellationToken.None).ContinueWith(trigger =>
                trigger.TriggerValueAsync(true, CancellationToken.None).Forget()).Forget();
        }
    }
}