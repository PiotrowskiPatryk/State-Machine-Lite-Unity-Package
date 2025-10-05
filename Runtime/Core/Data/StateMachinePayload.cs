using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Dev.Cortez.StateMachines.Core.Data
{
    [Serializable]
    public class StateMachinePayload : IStateMachinePayload
    {
        public string StateMachineId { get; }
        public IStatePayload StatePayload { get; }
        public ITransitionSolver TransitionSolver { get; }
        public IState InitialState { get; }
        public IEnumerable<IState> States { get; }

        public StateMachinePayload([NotNull] string stateMachineId, [NotNull] IStatePayload statePayload,
            [NotNull] ITransitionSolver transitionSolver, [NotNull] IState initialState,
            [NotNull] IEnumerable<IState> states)
        {
            StateMachineId = stateMachineId;
            StatePayload = statePayload;
            TransitionSolver = transitionSolver;
            InitialState = initialState;
            States = states;
        }
    }
}