using System.Collections.Generic;
using Dev.Cortez.StateMachines.Core.Data;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Core.StateMachineConfiguration.Definition;

namespace Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Common.Builders
{
    /// <summary>
    /// Fluent builder for creating StateDefinition objects in tests.
    /// Reduces boilerplate and improves test readability.
    /// </summary>
    public sealed class StateDefinitionBuilder
    {
        private string _id = "test-state";
        private string _name = "Test State";
        private string _description = "A test state";
        private string _typeName;
        private IPayload _payload = new EmptyPayload();
        private readonly List<TransitionRuleDefinition> _transitionRules = new();

        public StateDefinitionBuilder WithId(string id)
        {
            _id = id;

            return this;
        }

        public StateDefinitionBuilder WithName(string name)
        {
            _name = name;

            return this;
        }

        public StateDefinitionBuilder WithDescription(string description)
        {
            _description = description;

            return this;
        }

        public StateDefinitionBuilder WithType<T>() where T : IState
        {
            _typeName = typeof(T).AssemblyQualifiedName;

            return this;
        }

        public StateDefinitionBuilder WithTypeName(string typeName)
        {
            _typeName = typeName;

            return this;
        }

        public StateDefinitionBuilder WithPayload(IPayload payload)
        {
            _payload = payload;

            return this;
        }

        public StateDefinitionBuilder WithTransitionRule(TransitionRuleDefinition rule)
        {
            _transitionRules.Add(rule);

            return this;
        }

        public StateDefinition Build()
        {
            return new StateDefinition(
                _id,
                _name,
                _description,
                _typeName,
                _payload,
                _transitionRules
            );
        }

        /// <summary>
        /// Creates a new builder with default values.
        /// </summary>
        public static StateDefinitionBuilder Create()
        {
            return new StateDefinitionBuilder();
        }

        /// <summary>
        /// Creates a builder pre-configured for the specified type.
        /// </summary>
        public static StateDefinitionBuilder CreateForType<T>() where T : IState
        {
            return new StateDefinitionBuilder().WithType<T>();
        }
    }
}
