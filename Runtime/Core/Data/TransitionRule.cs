using JetBrains.Annotations;

namespace Dev.Cortez.StateMachines.Core.Data
{
    public sealed class TransitionRule
    {
        public IState CurrentState { get; }
        public IState TargetState { get; }
        public ICondition Condition { get; }
        public int Priority { get; }

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