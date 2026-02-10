namespace Dev.Cortez.StateMachines.Core.Data
{
    /// <summary>
    /// Represents the initialization status of a component or system.
    /// </summary>
    public enum InitializationStatus
    {
        /// <summary>
        /// The initialization status is undefined or not yet set.
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// The component has not yet been initialized.
        /// </summary>
        NotInitialized = 1,
        /// <summary>
        /// The component is currently undergoing initialization.
        /// </summary>
        Initializing = 2,
        /// <summary>
        /// The component has been successfully initialized.
        /// </summary>
        Initialized = 3,
        /// <summary>
        /// The initialization process failed.
        /// </summary>
        Failed = 4,
    }
}