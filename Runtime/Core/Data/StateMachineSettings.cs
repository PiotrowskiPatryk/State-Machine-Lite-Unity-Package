using System.Collections.Generic;
using Dev.Cortez.StateMachines.Core.StateMachineConfiguration.Definition;
using JetBrains.Annotations;

namespace Dev.Cortez.StateMachines.Core.Data
{
    [UsedImplicitly]
    public sealed class StateMachineSettings
    {
        public StateMachineDefinition StateMachineDefinition { get; }
        public ITransitionSolver TransitionSolver { get; }
        public IState InitialState { get; }
        public List<IState> States { get; }
        public IReadOnlyDictionary<IState, IReadOnlyList<TransitionRule>> TransitionRules { get; }

        public StateMachineSettings(StateMachineDefinition stateMachineDefinition, ITransitionSolver transitionSolver,
            IState initialState,
            List<IState> states,
            IReadOnlyDictionary<IState, IReadOnlyList<TransitionRule>> transitionRules)
        {
            StateMachineDefinition = stateMachineDefinition;
            TransitionSolver = transitionSolver;
            InitialState = initialState;
            States = states;
            TransitionRules = transitionRules;
        }
    }
}