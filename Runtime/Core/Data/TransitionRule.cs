using JetBrains.Annotations;

namespace Dev.Cortez.StateMachines.Core.Data
{
    /// <summary>
    ///     Represents a transition rule that defines the conditions and states involved in a state machine transition.
    ///     This class is immutable and encapsulates the details of a state transition,
    ///     including the current state, target state, condition, and the priority of the transition.
    /// </summary>
    public sealed class TransitionRule
    {
        /// <summary>
        ///     Gets the current state of the transition rule within a state machine.
        /// </summary>
        /// <remarks>
        ///     Represents the state from which the transition originates. It is part of the logic
        ///     defining the conditions under which a state move can occur.
        /// </remarks>
        public IState CurrentState { get; }

        /// <summary>
        ///     Gets the target state to transition to when the associated condition is satisfied.
        /// </summary>
        /// <remarks>
        ///     The TargetState represents the state that the state machine will move into
        ///     upon fulfilling the specified condition for the transition rule. It is immutable
        ///     and defined at the time of creating the <c>TransitionRule</c>.
        /// </remarks>
        public IState TargetState { get; }

        /// Represents the condition that determines whether a state transition can occur in a state machine.
        /// This property returns an implementation of the `ICondition` interface, which defines the logic
        /// for evaluating whether the condition required for a specific transition between states is satisfied.
        /// It supports notifying subscribers when the condition's satisfaction status changes.
        public ICondition Condition { get; }

        /// <summary>
        ///     Gets the priority of the transition rule.
        /// </summary>
        /// <remarks>
        ///     A higher value indicates a higher priority, which determines the order of evaluation
        ///     when multiple transition rules are satisfied. If rules have the same priority,
        ///     the internal order of rule registration will be considered.
        /// </remarks>
        public int Priority { get; }

        /// <summary>
        ///     Represents a rule for transitioning between two states in a state machine.
        /// </summary>
        public TransitionRule([NotNull] IState currentState, [NotNull] IState targetState,
            [NotNull] ICondition condition, int priority = 0)
        {
            CurrentState = currentState;
            TargetState = targetState;
            Condition = condition;
            Priority = priority;
        }
    }
}