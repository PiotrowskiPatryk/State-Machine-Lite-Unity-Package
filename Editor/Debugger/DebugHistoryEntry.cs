using System;

namespace Dev.Cortez.StateMachines.Editor.Debugger
{
    public record DebugHistoryEntry
    {
        public DateTime Timestamp { get; }
        public string Category { get; }
        public string SourceName { get; }
        public string Message { get; }

        public DebugHistoryEntry(DateTime timestamp, string category, string sourceName, string message)
        {
            Timestamp = timestamp;
            Category = category;
            SourceName = sourceName;
            Message = message;
        }
    }
}