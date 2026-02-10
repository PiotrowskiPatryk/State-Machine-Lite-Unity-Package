using System;
using System.Collections.Generic;
using System.Linq;

namespace Dev.Cortez.StateMachines.Editor.Debugger
{
    public sealed class DebuggerHistoryManager
    {
        private const int MaxEntries = 100;
        private readonly List<DebugHistoryEntry> _entries = new();

        public void Log(string category, string sourceName, string message)
        {
            _entries.Add(new DebugHistoryEntry(DateTime.Now, category, sourceName, message));

            if (_entries.Count > MaxEntries)
            {
                _entries.RemoveAt(0);
            }
        }

        public List<DebugHistoryEntry> GetEntriesForSource(string sourceName, int limit = 20)
        {
            return _entries.Where(debugHistoryEntry =>
                    debugHistoryEntry.SourceName.Contains(sourceName, StringComparison.OrdinalIgnoreCase)).
                OrderByDescending(debugHistoryEntry => debugHistoryEntry.Timestamp).Take(limit).ToList();
        }

        public void ClearForSource(string sourceName)
        {
            _entries.RemoveAll(debugHistoryEntry =>
                debugHistoryEntry.SourceName.Contains(sourceName, StringComparison.OrdinalIgnoreCase));
        }
    }
}