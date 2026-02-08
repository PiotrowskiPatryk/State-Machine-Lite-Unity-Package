using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;

namespace Samples.GameScenario.Scripts.StateMachines.Safe
{
    [UsedImplicitly]
    public sealed class LockingSafeState : SafeStateBase
    {
        protected override UniTask DoEnterAsync(StateContext context, CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        protected override UniTask DoExitAsync(StateContext context, CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }
    }
}