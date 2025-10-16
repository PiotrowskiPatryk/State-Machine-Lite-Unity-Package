using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Dev.Cortez.StateMachines.Core.Data
{
    [UsedImplicitly]
    public sealed class StateMachineSettings
    {
        public Type Type { get; }
        public string Id { get; }
        public ITransitionSolver TransitionSolver { get; }
        public IState InitialState { get; }
        public List<IState> States { get; }
        public IReadOnlyDictionary<IState, IReadOnlyList<TransitionRule>> TransitionRules { get; }
        
        public StateMachineSettings(string id, ITransitionSolver transitionSolver, IState initialState,
            List<IState> states,
            IReadOnlyDictionary<IState, IReadOnlyList<TransitionRule>> transitionRules,
            Type type)
        {
            Id = id;
            TransitionSolver = transitionSolver;
            InitialState = initialState;
            States = states;
            TransitionRules = transitionRules;
            Type = type;
        }
    }
}
