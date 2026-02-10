using System.Threading;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Abstraction;

namespace StateMachineExamples.Scenario.Scripts.Objective
{
    public abstract class ScenarioObjectiveStateBase : StateBase<ScenarioObjectiveContext>
    {
        protected override UniTask DoEnterAsync(ScenarioObjectiveContext context, CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        protected override UniTask DoExitAsync(ScenarioObjectiveContext context, CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }
    }
}