using System;

namespace Dev.Cortez.StateMachines.Core.Data
{
    public class StateSettings
    {
        public string Id { get; }
        public string Name { get; }
        public string Description { get; }
        public Type Type { get; }
        
        public StateSettings(string id, string name, string description,
            Type type)
        {
            Id = id;
            Name = name;
            Description = description;
            Type = type;
        }
    }
}
