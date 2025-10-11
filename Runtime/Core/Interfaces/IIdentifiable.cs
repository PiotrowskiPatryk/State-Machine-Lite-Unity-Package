namespace Dev.Cortez.StateMachines.Core
{
    public interface IIdentifiable
    {
        string Id { get; }
        string Name { get; }
        string Description { get; }
    }
}