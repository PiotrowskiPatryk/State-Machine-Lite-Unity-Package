using System.Collections.Generic;
using System.Threading;
using Dev.Cortez.StateMachines.Core.Interfaces;
using UnityEditor;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Editor.Debugger
{
    public static class TriggerDrawer
    {
        public static void Draw(
            ITrigger trigger,
            DebuggerHistoryManager historyManager,
            Dictionary<string, bool> historyFoldouts)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            DrawHeader(trigger);
            DrawDetails(trigger);
            DrawActionButtons(trigger);
            HistoryDrawer.Draw($"trigger_{trigger.Id}", trigger.Name, historyManager, historyFoldouts);

            EditorGUILayout.EndVertical();
        }

        private static void DrawHeader(ITrigger trigger)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(trigger.Name, EditorStyles.boldLabel);
            DrawStatus(trigger.IsTriggered);
            EditorGUILayout.EndHorizontal();
        }

        private static void DrawStatus(bool isTriggered)
        {
            var previousColor = GUI.backgroundColor;
            GUI.backgroundColor = isTriggered ? Color.green : Color.gray;
            GUILayout.Label(isTriggered ? "● TRIGGERED" : "○ Not Triggered", EditorStyles.miniButton, GUILayout.Width(110));
            GUI.backgroundColor = previousColor;
        }

        private static void DrawDetails(ITrigger trigger)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("ID", trigger.Id, EditorStyles.miniLabel);

            if (!string.IsNullOrEmpty(trigger.Description))
            {
                EditorGUILayout.LabelField("Description", trigger.Description, EditorStyles.wordWrappedMiniLabel);
            }

            EditorGUI.indentLevel--;
        }

        private static void DrawActionButtons(ITrigger trigger)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUI.enabled = Application.isPlaying;

            if (GUILayout.Button("Trigger TRUE", GUILayout.Width(100)))
            {
                trigger.TriggerValueAsync(true, CancellationToken.None);
            }

            if (GUILayout.Button("Trigger FALSE", GUILayout.Width(100)))
            {
                trigger.TriggerValueAsync(false, CancellationToken.None);
            }

            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
        }
    }
}
