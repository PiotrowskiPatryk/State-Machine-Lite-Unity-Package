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
        public IEnumerable<IState> States { get; }

        public DefaultStateMachinePayload([NotNull] string stateMachineId,
            [NotNull] ITransitionSolver transitionSolver, [NotNull] IState initialState,
            [NotNull] IEnumerable<IState> states)
        {
            StateMachineId = stateMachineId;
            TransitionSolver = transitionSolver;
            InitialState = initialState;
            States = states;
        }
    }
}