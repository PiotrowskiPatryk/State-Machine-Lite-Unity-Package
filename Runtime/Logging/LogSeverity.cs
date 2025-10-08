namespace Dev.Cortez.StateMachines.Logging
{
    /// <summary>
    ///     Represents the severity levels for logging messages.
    ///     Used to filter and categorize log messages based on their importance.
    /// </summary>
    public enum LogSeverity
    {
        /// <summary>
        ///     Undefined severity level. Typically used as a default or placeholder value.
        /// </summary>
        Undefined = 0,

        /// <summary>
        ///     No logging. Messages of this severity will not be logged.
        /// </summary>
        None = 1,

        /// <summary>
        ///     Trace logging messages.
        /// </summary>
        Trace = 2,

        /// <summary>
        ///     Informational messages that provide general context or state information.
        /// </summary>
        Info = 3,

        /// <summary>
        ///     Warning messages that indicate potential issues or non-critical problems.
        /// </summary>
        Warning = 4,

        /// <summary>
        ///     Error messages indicating significant issues that require attention.
        /// </summary>
        Error = 5,

        /// <summary>
        ///     Exceptions or critical errors that provide detailed information about failures.
        /// </summary>
        Exception = 6,

        /// <summary>
        ///     All severity levels. Typically used to enable logging of all messages.
        /// </summary>
        All = 7
    }
}