using Dev.Cortez.StateMachines.Core.Data;
using Dev.Cortez.StateMachines.Core.Interfaces;

namespace Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Common.Builders
{
    /// <summary>
    /// Fluent builder for creating TransitionRule objects in tests.
    /// Reduces boilerplate and improves test readability.
    /// </summary>
    public sealed class TransitionRuleBuilder
    {
        private IState _currentState;
        private IState _targetState;
        private ICondition _condition;
        private int _priority;

        public TransitionRuleBuilder From(IState state)
        {
            _currentState = state;
            return this;
        }

        public TransitionRuleBuilder To(IState state)
        {
            _targetState = state;
            return this;
        }

        public TransitionRuleBuilder WithCondition(ICondition condition)
        {
            _condition = condition;
            return this;
        }

        public TransitionRuleBuilder WithPriority(int priority)
        {
            _priority = priority;
            return this;
        }

        public TransitionRule Build()
        {
            return new TransitionRule(_currentState, _targetState, _condition, _priority);
        }

        /// <summary>
        /// Creates a new builder with default values.
        /// </summary>
        public static TransitionRuleBuilder Create() => new();
    }
}
