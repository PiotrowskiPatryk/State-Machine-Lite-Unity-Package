#region

using System;

#endregion

namespace Dev.Cortez.StateMachines.Logging
{
    internal class NullLogger : LoggerBase
    {
        public NullLogger(object context, LogSeverity logSeverity) : base(context, logSeverity)
        {
        }

        protected override void WriteLogTrace(string message)
        {
        }

        protected override void WriteLogWarning(string message)
        {
        }

        protected override void WriteLogError(string message)
        {
        }

        protected override void WriteLogException(Exception exception)
        {
        }

        protected override void WriteLogInfo(string message)
        {
        }
    }
}