#region

using System;
using System.Diagnostics.CodeAnalysis;

#endregion

namespace Dev.Cortez.StateMachines.Logging
{
    internal abstract class LoggerBase : ILogger
    {
        private LogSeverity _logSeverity;

        [NotNull]
        protected object Context { get; }

        protected LoggerBase(object context, LogSeverity logSeverity)
        {
            _logSeverity = logSeverity;
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void LogTrace(string message)
        {
            if (_logSeverity <= LogSeverity.Trace)
            {
                WriteLogTrace(message);
            }
        }

        public void LogInfo(string message)
        {
            if (_logSeverity <= LogSeverity.Info)
            {
                WriteLogInfo(message);
            }
        }

        public void LogWarning(string message)
        {
            if (_logSeverity <= LogSeverity.Warning)
            {
                WriteLogWarning(message);
            }
        }

        public void LogError(string message)
        {
            if (_logSeverity <= LogSeverity.Error)
            {
                WriteLogError(message);
            }
        }

        public void LogException(Exception exception)
        {
            if (_logSeverity <= LogSeverity.Exception)
            {
                WriteLogException(exception);
            }
        }

        public void SetLogSeverity(LogSeverity logSeverity)
        {
            _logSeverity = logSeverity;
        }

        protected abstract void WriteLogTrace(string message);
        protected abstract void WriteLogWarning(string message);
        protected abstract void WriteLogError(string message);
        protected abstract void WriteLogException(Exception exception);
        protected abstract void WriteLogInfo(string message);
    }
}