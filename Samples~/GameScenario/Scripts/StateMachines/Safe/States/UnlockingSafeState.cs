using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Samples.GameScenario.Scripts.StateMachines.Safe.Context;
using Samples.GameScenario.Scripts.StateMachines.Safe.Payload;

namespace Samples.GameScenario.Scripts.StateMachines.Safe.States
{
    [UsedImplicitly]
    public sealed class UnlockingSafeState : SafeStateBase<UnlockingSafeStatePayload>
    {
        private UnlockingSafeStatePayload _payload;

        protected override UniTask<bool> DoInitializeAsync(UnlockingSafeStatePayload payload,
            CancellationToken cancellationToken)
        {
            _payload = payload;

            return UniTask.FromResult(true);
        }

        protected override async UniTask DoEnterAsync(StateContext context, CancellationToken cancellationToken)
        {
            await context.SafeController.OpenSafeAsync(cancellationToken);

            var trigger = await _payload.TriggeredOpenedSafe.ResolveReferenceAsync(cancellationToken);
            trigger.TriggerValueAsync(true, cancellationToken).Forget();
        }

        protected override UniTask DoExitAsync(StateContext context, CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }
    }
}