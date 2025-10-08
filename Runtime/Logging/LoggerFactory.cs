namespace Dev.Cortez.StateMachines.Logging
{
    /// <summary>
    ///     Factory class for creating instances of <see cref="ILogger" /> based on the given context
    ///     and severity level. Provides a unified way to manage logging behavior across different environments.
    /// </summary>
    public static class LoggerFactory
    {
        /// <summary>
        ///     Creates an instance of <see cref="ILogger" /> based on the provided context and severity level.
        ///     If logging is enabled (via ENABLE_LOGGING directive), it creates a <see cref="UnityLogger" />.
        ///     Otherwise, it creates a <see cref="NullLogger" /> that performs no logging.
        /// </summary>
        /// <param name="context">The context object for the logger, typically used to identify the source of log messages.</param>
        /// <param name="severity">The initial severity level for the logger. Defaults to <see cref="LogSeverity.Info" />.</param>
        /// <returns>An instance of <see cref="ILogger" /> suitable for the current environment.</returns>
        public static ILogger Create(object context, LogSeverity severity = LogSeverity.Info)
        {
#if ENABLE_LOGGING
            var unityLogger = new UnityLogger(context, severity);

            return unityLogger;
#endif

            var nullLogger = new NullLogger(context, severity);

            return nullLogger;
        }
    }
}