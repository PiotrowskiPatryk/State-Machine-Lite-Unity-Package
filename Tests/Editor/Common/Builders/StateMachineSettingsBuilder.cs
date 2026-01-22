using System.Collections.Generic;
using Dev.Cortez.StateMachines.Core.Data;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Core.StateMachineConfiguration.Definition;

namespace Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Common.Builders
{
    /// <summary>
    /// Fluent builder for creating StateMachineSettings objects in tests.
    /// Reduces boilerplate and improves test readability.
    /// </summary>
    public sealed class StateMachineSettingsBuilder
    {
        private StateMachineDefinition _stateMachineDefinition;
        private ITransitionSolver _transitionSolver;
        private IState _initialState;
        private List<IState> _states = new();
        private Dictionary<IState, IReadOnlyList<TransitionRule>> _transitionRules = new();

        public StateMachineSettingsBuilder WithStateMachineDefinition(StateMachineDefinition definition)
        {
            _stateMachineDefinition = definition;
            return this;
        }

        public StateMachineSettingsBuilder WithTransitionSolver(ITransitionSolver solver)
        {
            _transitionSolver = solver;
            return this;
        }

        public StateMachineSettingsBuilder WithInitialState(IState state)
        {
            _initialState = state;
            return this;
        }

        public StateMachineSettingsBuilder WithState(IState state)
        {
            _states.Add(state);
            return this;
        }

        public StateMachineSettingsBuilder WithStates(IEnumerable<IState> states)
        {
            _states.AddRange(states);
            return this;
        }

        public StateMachineSettingsBuilder WithTransitionRulesForState(IState state, IReadOnlyList<TransitionRule> rules)
        {
            _transitionRules[state] = rules;
            return this;
        }

        public StateMachineSettings Build()
        {
            return new StateMachineSettings(
                _stateMachineDefinition,
                _transitionSolver,
                _initialState,
                _states,
                _transitionRules
            );
        }

        /// <summary>
        /// Creates a new builder with default values.
        /// </summary>
        public static StateMachineSettingsBuilder Create() => new();
    }
}
