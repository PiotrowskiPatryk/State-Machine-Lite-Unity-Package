namespace Dev.Cortez.StateMachines.Core.Data
{
    /// <summary>
    /// Represents the current status of a state within a state machine.
    /// </summary>
    public enum StateStatus
    {
        /// <summary>
        /// The state is in an undefined or initial status.
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// The state is currently in the process of activating.
        /// </summary>
        Activating = 1,
        /// <summary>
        /// The state is fully active and operational.
        /// </summary>
        Active = 2,
        /// <summary>
        /// The state is currently in the process of deactivating.
        /// </summary>
        Deactivating = 3,
        /// <summary>
        /// The state is inactive and not operational.
        /// </summary>
        Inactive = 4,
    }
}