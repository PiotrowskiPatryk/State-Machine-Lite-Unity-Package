using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;

namespace Samples.GameScenario.Scripts.StateMachines.Safe
{
    [UsedImplicitly]
    public sealed class UnlockingSafeState : SafeStateBase
    {
        protected override async UniTask DoEnterAsync(StateContext context, CancellationToken cancellationToken)
        {
            await context.SafeController.OpenSafeAsync(cancellationToken);
        }

        protected override UniTask DoExitAsync(StateContext context, CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }
    }
}