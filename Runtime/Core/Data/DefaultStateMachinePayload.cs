using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Dev.Cortez.StateMachines.Core.Data
{
    [Serializable]
    public class DefaultStateMachinePayload : IStateMachinePayload
    {
        public string StateMachineId { get; }
        public ITransitionSolver TransitionSolver { get; }
        public IState InitialState { get; }
        public List<IState> States { get; }
        public IReadOnlyDictionary<IState, IReadOnlyList<TransitionRule>> TransitionRules { get; }

        public DefaultStateMachinePayload([NotNull] string stateMachineId,
            [NotNull] ITransitionSolver transitionSolver, [NotNull] IState initialState,
            [NotNull] List<IState> states,
            [NotNull] IReadOnlyDictionary<IState, IReadOnlyList<TransitionRule>> transitionRules)
        {
            StateMachineId = stateMachineId;
            TransitionSolver = transitionSolver;
            InitialState = initialState;
            States = states;
            TransitionRules = transitionRules;
        }
    }
}