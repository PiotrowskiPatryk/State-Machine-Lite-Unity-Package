#region

using System;

#endregion

namespace Dev.Cortez.StateMachines.Logging
{
    /// <summary>
    ///     Interface for a logging system that provides methods for logging messages and exceptions
    ///     with varying levels of severity, as well as configuring the logging behavior.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        ///     Logs a trace-level message typically used for debugging purposes.
        /// </summary>
        /// <param name="message">The trace message to log.</param>
        void LogTrace(string message);

        /// <summary>
        ///     Logs an informational message.
        /// </summary>
        /// <param name="message">The informational message to log.</param>
        void LogInfo(string message);

        /// <summary>
        ///     Logs a warning message.
        /// </summary>
        /// <param name="message">The warning message to log.</param>
        void LogWarning(string message);

        /// <summary>
        ///     Logs an error message.
        /// </summary>
        /// <param name="message">The error message to log.</param>
        void LogError(string message);

        /// <summary>
        ///     Logs an exception with its details.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        void LogException(Exception exception);

        /// <summary>
        ///     Sets the minimum severity level for logging messages.
        ///     Messages below this severity will not be logged.
        /// </summary>
        /// <param name="logSeverity">The minimum severity level to set for logging.</param>
        void SetLogSeverity(LogSeverity logSeverity);
    }
}