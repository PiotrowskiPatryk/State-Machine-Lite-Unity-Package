using System;

namespace Dev.Cortez.StateMachines.Core.Data
{
    [Serializable]
    public class DefaultStatePayload : IStatePayload
    {
        public string Id { get; }

        public DefaultStatePayload(string id)
        {
            Id = id;
        }

        public DefaultStatePayload()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}