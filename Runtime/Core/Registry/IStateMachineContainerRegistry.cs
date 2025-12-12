using System.Collections.Generic;

namespace Dev.Cortez.StateMachines.Core.Registry
{
    public interface IStateMachineContainerRegistry
    {
        public string Identifier { get; }
        public IReadOnlyDictionary<string, ITrigger> Triggers { get; }
        public IReadOnlyDictionary<string, IStateMachine> StateMachines { get; }
    }
}