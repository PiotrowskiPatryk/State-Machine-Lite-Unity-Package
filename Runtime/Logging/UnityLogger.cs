#region

using System;
using UnityEngine;

#endregion

namespace Dev.Cortez.StateMachines.Logging
{
    internal sealed class UnityLogger : LoggerBase
    {
        public UnityLogger(object context, LogSeverity logSeverity) : base(context, logSeverity)
        {
        }

        protected override void WriteLogInfo(string message)
        {
            Debug.Log($"[{Context}]: {message}");
        }

        protected override void WriteLogTrace(string message)
        {
            Debug.Log($"[{Context}] (trace): {message}");
        }

        protected override void WriteLogWarning(string message)
        {
            Debug.LogWarning($"[{Context}]: {message}");
        }

        protected override void WriteLogError(string message)
        {
            Debug.LogError($"[{Context}]: {message}");
        }

        protected override void WriteLogException(Exception exception)
        {
            Debug.LogError($"[{Context}]: {exception}");
            Debug.LogException(exception);
        }
    }
}