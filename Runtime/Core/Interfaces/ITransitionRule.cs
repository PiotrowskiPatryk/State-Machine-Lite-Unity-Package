namespace Dev.Cortez.StateMachines.Core
{
    public interface ITransitionRule : IIdentifable
    {
        bool IsActive { get; }
        ICondition Condition { get; }
        IState TargetState { get; }
    }
}