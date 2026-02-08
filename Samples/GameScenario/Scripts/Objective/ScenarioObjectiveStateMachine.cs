using System.Threading;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Abstraction;

namespace StateMachineExamples.Scenario.Scripts.Objective
{
    public class ScenarioObjectiveStateMachine : StateMachineBase<ScenarioObjectiveStateBase, ScenarioObjectivePayload,
        ScenarioObjectiveContext>
    {
        private ScenarioObjectiveContext _scenarioObjectiveContext;

        protected override ScenarioObjectiveContext StateContext => _scenarioObjectiveContext;

        protected override UniTask<bool> DoInitializeStateMachineAsync(ScenarioObjectivePayload stateMachinePayload,
            CancellationToken cancellationToken)
        {
            _scenarioObjectiveContext = new ScenarioObjectiveContext(stateMachinePayload.ObjectiveName);

            return UniTask.FromResult(true);
        }
    }
}