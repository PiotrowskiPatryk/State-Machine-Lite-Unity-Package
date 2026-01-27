namespace Dev.Cortez.StateMachines.Logging
{
    public static class LoggerService
    {
        public static ILogger Logger { get; } = LoggerFactory.Create("StateMachines", LogSeverity.Trace);
    }
}