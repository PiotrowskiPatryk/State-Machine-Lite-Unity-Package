using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Editor.Debugger
{
    public static class HistoryDrawer
    {
        public static void Draw(
            string foldoutKey,
            string sourceName,
            DebuggerHistoryManager historyManager,
            Dictionary<string, bool> foldoutStates)
        {
            foldoutStates.TryAdd(foldoutKey, false);

            var entries = historyManager.GetEntriesForSource(sourceName);

            EditorGUILayout.BeginHorizontal();
            foldoutStates[foldoutKey] = EditorGUILayout.Foldout(foldoutStates[foldoutKey], $"History ({entries.Count})", true);

            if (entries.Count > 0 && GUILayout.Button("Clear", GUILayout.Width(50)))
            {
                historyManager.ClearForSource(sourceName);
            }

            EditorGUILayout.EndHorizontal();

            if (!foldoutStates[foldoutKey])
            {
                return;
            }

            EditorGUI.indentLevel++;

            if (entries.Count == 0)
            {
                EditorGUILayout.LabelField("No history entries.", EditorStyles.miniLabel);
            }
            else
            {
                foreach (var entry in entries)
                {
                    EditorGUILayout.LabelField($"[{entry.Timestamp:HH:mm:ss}] [{entry.Category}] {entry.Message}", EditorStyles.miniLabel);
                }
            }

            EditorGUI.indentLevel--;
        }
    }
}
