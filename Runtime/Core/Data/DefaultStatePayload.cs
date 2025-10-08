using System;

namespace Dev.Cortez.StateMachines.Core.Data
{
    /// <summary>
    ///     Represents the default implementation of a state payload used within a state machine.
    /// </summary>
    /// <remarks>
    ///     This class is typically used when no custom payload behavior is necessary,
    ///     providing a simple mechanism to uniquely identify payload instances, particularly
    ///     in scenarios involving state machine transitions or operations.
    /// </remarks>
    [Serializable]
    public class DefaultStatePayload : IStatePayload
    {
        /// <summary>
        ///     Gets the unique identifier associated with the payload or state.
        ///     This identifier is used to distinguish instances and ensure proper
        ///     management and tracking within state machine operations.
        /// </summary>
        public string Id { get; }

        /// <summary>
        ///     Represents the default implementation of a state payload within the state machine framework.
        ///     Provides a unique identifier for the payload. Typically used when a simple payload structure
        ///     is required for state transitions without the need for additional properties or customization.
        /// </summary>
        public DefaultStatePayload(string id)
        {
            Id = id;
        }

        /// Represents the default implementation of a state payload.
        /// This payload can be used to associate an identifier with a state.
        public DefaultStatePayload()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}