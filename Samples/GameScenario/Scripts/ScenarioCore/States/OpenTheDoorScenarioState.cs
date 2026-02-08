using System.Threading;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Data;

namespace Samples.GameScenario.Scripts.ScenarioCore.States
{
    public sealed class OpenTheDoorScenarioState : MainGameScenarioStateBase
    {
        protected override UniTask DoEnterAsync(EmptyContext context, CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        protected override UniTask DoExitAsync(EmptyContext context, CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }
    }
}