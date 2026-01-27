namespace Dev.Cortez.StateMachines.Core.Interfaces
{
    public interface ITransitionRule : IIdentifiable
    {
        bool IsActive { get; }
        ICondition Condition { get; }
        IState TargetState { get; }
    }
}