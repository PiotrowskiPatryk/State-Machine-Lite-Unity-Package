using System.Collections.Generic;
using Dev.Cortez.StateMachines.Core.Data;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Core.StateMachineConfiguration.Definition;

namespace Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Common.Builders
{
    /// <summary>
    /// Fluent builder for creating StateMachineDefinition objects in tests.
    /// Reduces boilerplate and improves test readability.
    /// </summary>
    public sealed class StateMachineDefinitionBuilder
    {
        private string _id = "test-state-machine";
        private string _name = "Test State Machine";
        private string _description = "A test state machine";
        private string _stateMachineTypeName;

        private string _transitionSolverTypeName =
            "Dev.Cortez.StateMachines.Core.TransitionSolver.DefaultTransitionSolver, Dev.Cortez.StateMachines";

        private IPayload _payload = new EmptyPayload();
        private StateDefinition _initialState;
        private readonly List<StateDefinition> _states = new();

        public StateMachineDefinitionBuilder WithId(string id)
        {
            _id = id;

            return this;
        }

        public StateMachineDefinitionBuilder WithName(string name)
        {
            _name = name;

            return this;
        }

        public StateMachineDefinitionBuilder WithDescription(string description)
        {
            _description = description;

            return this;
        }

        public StateMachineDefinitionBuilder WithType<T>() where T : IStateMachine
        {
            _stateMachineTypeName = typeof(T).AssemblyQualifiedName;

            return this;
        }

        public StateMachineDefinitionBuilder WithTypeName(string typeName)
        {
            _stateMachineTypeName = typeName;

            return this;
        }

        public StateMachineDefinitionBuilder WithTransitionSolverType<T>() where T : ITransitionSolver
        {
            _transitionSolverTypeName = typeof(T).AssemblyQualifiedName;

            return this;
        }

        public StateMachineDefinitionBuilder WithTransitionSolverTypeName(string typeName)
        {
            _transitionSolverTypeName = typeName;

            return this;
        }

        public StateMachineDefinitionBuilder WithPayload(IPayload payload)
        {
            _payload = payload;

            return this;
        }

        public StateMachineDefinitionBuilder WithInitialState(StateDefinition state)
        {
            _initialState = state;

            if (!_states.Contains(state))
            {
                _states.Add(state);
            }

            return this;
        }

        public StateMachineDefinitionBuilder WithState(StateDefinition state)
        {
            _states.Add(state);

            return this;
        }

        public StateMachineDefinition Build()
        {
            return new StateMachineDefinition(
                _id,
                _name,
                _description,
                _stateMachineTypeName,
                _transitionSolverTypeName,
                _initialState,
                _states,
                _payload
            );
        }

        /// <summary>
        /// Creates a new builder with default values.
        /// </summary>
        public static StateMachineDefinitionBuilder Create()
        {
            return new StateMachineDefinitionBuilder();
        }

        /// <summary>
        /// Creates a builder pre-configured for the specified type.
        /// </summary>
        public static StateMachineDefinitionBuilder CreateForType<T>() where T : IStateMachine
        {
            return new StateMachineDefinitionBuilder().WithType<T>();
        }
    }
}