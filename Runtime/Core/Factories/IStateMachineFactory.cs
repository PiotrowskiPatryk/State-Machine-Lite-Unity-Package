#region

using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Data;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Core.StateMachineConfiguration.Definition;
using JetBrains.Annotations;

#endregion

namespace Dev.Cortez.StateMachines.Core.Factories
{
    public interface IStateMachineFactory
    {
        UniTask<IStateMachine> CreateStateMachineAsync(
            [NotNull] StateMachineDefinition stateMachineDefinition, CancellationToken cancellationToken);

        UniTask<Dictionary<IState, IReadOnlyList<TransitionRule>>> CreateTransitionRules(
            List<IState> states,
            [NotNull] StateMachineDefinition stateMachineDefinition, CancellationToken cancellationToken);

        UniTask<TransitionRule> CreateTransitionRuleAsync(List<IState> states,
            IState originState, TransitionRuleDefinition transitionRuleDefinition,
            CancellationToken cancellationToken);

        UniTask<ICondition> CreateConditionAsync(ConditionDefinition conditionDefinition,
            CancellationToken cancellationToken);

        UniTask<IState> CreateStateAsync([NotNull] StateDefinition stateDefinition,
            CancellationToken cancellationToken);

        UniTask<ITrigger> CreateTriggerAsync([NotNull] TriggerDefinition triggerDefinition,
            CancellationToken cancellationToken);
    }
}