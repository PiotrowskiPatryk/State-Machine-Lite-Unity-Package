using System;
using System.Collections.Generic;
using Dev.Cortez.StateMachines.Core.Interfaces;

namespace Dev.Cortez.StateMachines.Core.Registry
{
    /// <summary>
    /// Represents a registry that holds and provides access to state machines and their associated triggers
    /// within a specific container or scope.
    /// </summary>
    public interface IStateMachineContainerEntry : IAsyncDisposable
    {
        /// <summary>
        /// Gets the unique identifier for this state machine container registry.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets a read-only dictionary of triggers managed by this registry,
        /// where the key is the trigger's identifier and the value is the <see cref="ITrigger"/> instance.
        /// </summary>
        public IReadOnlyDictionary<string, ITrigger> Triggers { get; }

        /// <summary>
        /// Gets a read-only dictionary of state machines managed by this registry,
        /// where the key is the state machine's identifier and the value is the <see cref="IStateMachine"/> instance.
        /// </summary>
        public IReadOnlyDictionary<string, IStateMachine> StateMachines { get; }
    }
}