namespace Dev.Cortez.StateMachines.Core.Interfaces
{
    public interface IIdentifiable
    {
        string Id { get; }
        string Name { get; }
        string Description { get; }
    }
}