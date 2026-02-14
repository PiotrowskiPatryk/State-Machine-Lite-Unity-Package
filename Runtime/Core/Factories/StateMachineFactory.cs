#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Abstraction;
using Dev.Cortez.StateMachines.Core.Condition;
using Dev.Cortez.StateMachines.Core.Data;
using Dev.Cortez.StateMachines.Core.Installer;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Core.StateMachineConfiguration.Definition;
using Dev.Cortez.StateMachines.Logging;
using JetBrains.Annotations;

#endregion

namespace Dev.Cortez.StateMachines.Core.Factories
{
    public sealed class StateMachineFactory : IStateMachineFactory
    {
        private readonly IStateMachineInstanceBuilder _stateMachineInstanceBuilder;

        public StateMachineFactory(IStateMachineInstanceBuilder stateMachineInstanceBuilder)
        {
            _stateMachineInstanceBuilder = stateMachineInstanceBuilder;
        }

        public static IStateMachineFactory Default()
        {
            return new StateMachineFactory(new ActivatorBasedStateMachineInstallerBuilder());
        }

        [ItemCanBeNull]
        public async UniTask<IStateMachine> CreateStateMachineAsync(
            [NotNull] StateMachineDefinition stateMachineDefinition, CancellationToken cancellationToken)
        {
            LoggerService.Logger.LogTrace(
                $"Creating state machine: [{stateMachineDefinition.Id} {stateMachineDefinition.Name}]");

            using var linkedCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            var stateMachineType = Type.GetType(stateMachineDefinition.StateMachineTypeName);

            if (stateMachineType == null)
            {
                LoggerService.Logger.LogError(
                    "Unable to create state machine. Provided state machine type is not valid.");

                return null;
            }

            if (stateMachineDefinition.InitialState == null)
            {
                LoggerService.Logger.LogError("Unable to create state machine. Initial state is null.");

                return null;
            }

            var isStateMachineType = typeof(IStateMachine).IsAssignableFrom(stateMachineType);

            if (!isStateMachineType)
            {
                LoggerService.Logger.LogError(
                    "Unable to create state machine. State machine type is not assignable from IStateMachine.");

                return null;
            }

            var transitionSolverType = Type.GetType(stateMachineDefinition.TransitionSolverTypeName);
            var isTransitionSolverType = typeof(ITransitionSolver).IsAssignableFrom(transitionSolverType);

            if (transitionSolverType == null || !isTransitionSolverType)
            {
                LoggerService.Logger.LogError(
                    "Unable to create transition solver. Provided transition solver type is not valid.");

                return null;
            }

            var stateMachineInstance = _stateMachineInstanceBuilder.CreateInstance<IStateMachine>(stateMachineType);

            if (stateMachineInstance == null)
            {
                LoggerService.Logger.LogError("Unable to create state machine instance");

                return null;
            }

            var transitionSolver = _stateMachineInstanceBuilder.CreateInstance<ITransitionSolver>(transitionSolverType);
            var states = await stateMachineDefinition.States.Select(state =>
                CreateStateAsync(state, linkedCancellationToken.Token));

            var initialState = states.First(state => state.Id.Equals(stateMachineDefinition.InitialState.Id));
            var transitionRules =
                await CreateTransitionRules(states.ToList(), stateMachineDefinition, linkedCancellationToken.Token);
            var stateMachineSettings = new StateMachineSettings(stateMachineDefinition, transitionSolver,
                initialState, states.ToList(), transitionRules);

            await stateMachineInstance.InitializeAsync(stateMachineSettings, linkedCancellationToken.Token);

            return stateMachineInstance;
        }

        [ItemCanBeNull]
        public async UniTask<Dictionary<IState, IReadOnlyList<TransitionRule>>> CreateTransitionRules(
            List<IState> states,
            [NotNull] StateMachineDefinition stateMachineDefinition, CancellationToken cancellationToken)
        {
            LoggerService.Logger.LogTrace(
                $"Creating transition rules for [{stateMachineDefinition.Id} {stateMachineDefinition.Name}]");

            using var linkedCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            var transitionRules = new Dictionary<IState, IReadOnlyList<TransitionRule>>();

            foreach (var state in stateMachineDefinition.States)
            {
                var stateTransitionRules = state.TransitionRules;
                var originState = states.FirstOrDefault(desiredState => desiredState.Id.Equals(state.Id));

                if (originState == null)
                {
                    LoggerService.Logger.LogError(
                        $"Unable to resolve transition rule. Cannot find origin state: [{state.Id} {state.Name}]");

                    return null;
                }

                var transitions = new List<TransitionRule>();

                foreach (var transitionRule in stateTransitionRules)
                {
                    transitions.Add(await CreateTransitionRuleAsync(states, originState, transitionRule,
                        linkedCancellationToken.Token));
                }

                transitionRules.Add(originState, transitions);
            }

            return transitionRules;
        }

        public async UniTask<TransitionRule> CreateTransitionRuleAsync(List<IState> states,
            IState originState, TransitionRuleDefinition transitionRuleDefinition,
            CancellationToken cancellationToken)
        {
            using var linkedCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            LoggerService.Logger.LogInfo(
                $"Creating transition from: [{originState.Id} {originState.Name}] to [{transitionRuleDefinition.TargetState.Id} {transitionRuleDefinition.TargetState.Name}]");

            var targetState = states.FirstOrDefault(desiredState =>
                desiredState.Id.Equals(transitionRuleDefinition.TargetState.Id));

            if (targetState == null)
            {
                LoggerService.Logger.LogError(
                    $"Unable to create transition from: [{originState.Id} {originState.Name}] to [{transitionRuleDefinition.TargetState.Id} {transitionRuleDefinition.TargetState.Name}]");

                return null;
            }

            List<ICondition> conditions = new();

            foreach (var conditionDefinition in transitionRuleDefinition.ConditionDefinitions)
            {
                var condition = await CreateConditionAsync(conditionDefinition, linkedCancellationToken.Token);
                conditions.Add(condition);
            }

            var conditionComposite = new ConditionComposite(conditions, transitionRuleDefinition.ConditionFilterType);

            var transitionRule = new TransitionRule(originState, targetState, conditionComposite,
                transitionRuleDefinition.Priority);

            return transitionRule;
        }

        [ItemCanBeNull]
        public async UniTask<ICondition> CreateConditionAsync(ConditionDefinition conditionDefinition,
            CancellationToken cancellationToken)
        {
            using var linkedCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            var conditionType = Type.GetType(conditionDefinition.Type);

            if (conditionType == null)
            {
                LoggerService.Logger.LogError(
                    $"Unable to create condition. Provided condition type is not valid [{conditionDefinition.Type}]");

                return null;
            }

            if (!typeof(ICondition).IsAssignableFrom(conditionType))
            {
                LoggerService.Logger.LogError(
                    $"Unable to create condition. Provided condition type is not implementing {typeof(ICondition)}");

                return null;
            }

            if (_stateMachineInstanceBuilder.CreateInstance(conditionType) is not ICondition conditionInstance)
            {
                LoggerService.Logger.LogError("Unable to create condition instance");

                return null;
            }

            if (conditionInstance is IAsyncInitializable conditionAsyncInitializable)
            {
                LoggerService.Logger.LogTrace(
                    $"Initializing condition: [{conditionDefinition.Id} {conditionDefinition.Name}]");

                await conditionAsyncInitializable.InitializeAsync(conditionDefinition.Payload,
                    linkedCancellationToken.Token);
            }

            return conditionInstance;
        }

        public async UniTask<IState> CreateStateAsync([NotNull] StateDefinition stateDefinition,
            CancellationToken cancellationToken)
        {
            LoggerService.Logger.LogTrace($"Creating state: [{stateDefinition.Id} {stateDefinition.Name}]");

            using var linkedCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            if (stateDefinition.TypeName == null)
            {
                LoggerService.Logger.LogError("Unable to create state. Provided state type is null");

                return null;
            }

            var stateType = Type.GetType(stateDefinition.TypeName);

            if (stateType == null)
            {
                LoggerService.Logger.LogError("Unable to create state. Provided state type is not valid");

                return null;
            }

            if (_stateMachineInstanceBuilder.CreateInstance(stateType) is not IState stateInstance)
            {
                LoggerService.Logger.LogError("Unable to create state instance");

                return null;
            }

            await stateInstance.InitializeAsync(stateDefinition, linkedCancellationToken.Token);

            return stateInstance;
        }

        public async UniTask<ITrigger> CreateTriggerAsync([NotNull] TriggerDefinition triggerDefinition,
            CancellationToken cancellationToken)
        {
            LoggerService.Logger.LogTrace($"Creating trigger: [{triggerDefinition.Id} {triggerDefinition.Name}]");

            using var linkedCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            if (!triggerDefinition.IsValid())
            {
                throw new ArgumentException("Unable to create trigger. Provided trigger definition is not valid");
            }

            var type = Type.GetType(triggerDefinition.TypeName) ??
                       AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).
                           FirstOrDefault(t => t.FullName == triggerDefinition.TypeName);

            if (type == null)
            {
                throw new ArgumentException(
                    $"Unable to create trigger. Provided type {triggerDefinition.TypeName} is not valid");
            }

            var args = new object[] { triggerDefinition.Id, triggerDefinition.Name, triggerDefinition.Description };

            if (!typeof(TriggerBase).IsAssignableFrom(type))
            {
                throw new ArgumentException(
                    $"Type '{type.FullName}' must derive from TriggerBase or TriggerBase<TPayload>.");
            }

            var triggerInstance = _stateMachineInstanceBuilder.CreateInstance<ITrigger>(type, args);

            if (triggerInstance is not IAsyncInitializable triggerAsyncInitializable)
            {
                return triggerInstance;
            }

            var initializedSuccessfully =
                await triggerAsyncInitializable.InitializeAsync(triggerDefinition.Payload,
                    linkedCancellationToken.Token);

            return initializedSuccessfully
                ? triggerInstance
                : throw new ArgumentException("Unable to create trigger instance. Initialization process failed.");
        }
    }
}