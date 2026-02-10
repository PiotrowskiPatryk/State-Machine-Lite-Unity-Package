using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Samples.GameScenario.Scripts.StateMachines.Safe.Context;

namespace Samples.GameScenario.Scripts.StateMachines.Safe.States
{
    [UsedImplicitly]
    public sealed class UnlockedSafeState : SafeStateBase
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