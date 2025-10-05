namespace Dev.Cortez.StateMachines.Core
{
    public interface IStatePayload : IPayload
    {
        string Id { get; }
    }
}