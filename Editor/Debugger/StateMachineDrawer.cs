using System.Collections.Generic;
using System.Linq;
using Dev.Cortez.StateMachines.Core.Interfaces;
using UnityEditor;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Editor.Debugger
{
    public static class StateMachineDrawer
    {
        public static void Draw(
            IStateMachine stateMachine,
            string searchFilter,
            DebuggerHistoryManager historyManager,
            Dictionary<string, bool> statesFoldouts,
            Dictionary<string, bool> transitionsFoldouts,
            Dictionary<string, bool> historyFoldouts)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            DrawHeader(stateMachine);
            DrawDetails(stateMachine);
            DrawStatesFoldout(stateMachine, searchFilter, historyManager, statesFoldouts, transitionsFoldouts, historyFoldouts);
            HistoryDrawer.Draw($"sm_{stateMachine.Id}", stateMachine.Name, historyManager, historyFoldouts);

            EditorGUILayout.EndVertical();
        }

        private static void DrawHeader(IStateMachine stateMachine)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(stateMachine.Name, EditorStyles.boldLabel);
            DrawActiveStatus(stateMachine.IsActive);
            EditorGUILayout.EndHorizontal();
        }

        private static void DrawActiveStatus(bool isActive)
        {
            var previousColor = GUI.backgroundColor;
            GUI.backgroundColor = isActive ? Color.green : Color.gray;
            GUILayout.Label(isActive ? "● ACTIVE" : "○ Inactive", EditorStyles.miniButton, GUILayout.Width(80));
            GUI.backgroundColor = previousColor;
        }

        private static void DrawDetails(IStateMachine stateMachine)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("ID", stateMachine.Id, EditorStyles.miniLabel);

            if (!string.IsNullOrEmpty(stateMachine.Description))
            {
                EditorGUILayout.LabelField("Description", stateMachine.Description, EditorStyles.wordWrappedMiniLabel);
            }

            EditorGUILayout.LabelField("Active State", stateMachine.ActiveState?.Name ?? "None");
            EditorGUI.indentLevel--;
        }

        private static void DrawStatesFoldout(
            IStateMachine stateMachine,
            string searchFilter,
            DebuggerHistoryManager historyManager,
            Dictionary<string, bool> statesFoldouts,
            Dictionary<string, bool> transitionsFoldouts,
            Dictionary<string, bool> historyFoldouts)
        {
            statesFoldouts.TryAdd(stateMachine.Id, false);

            var states = stateMachine.States;
            var filteredStates = states?.Where(s => MatchesFilter(s, searchFilter)).ToList() ?? new List<IState>();

            EditorGUILayout.BeginHorizontal();
            statesFoldouts[stateMachine.Id] = EditorGUILayout.Foldout(statesFoldouts[stateMachine.Id], $"States ({filteredStates.Count}/{states?.Count ?? 0})", true);
            EditorGUILayout.EndHorizontal();

            if (!statesFoldouts[stateMachine.Id])
            {
                return;
            }

            EditorGUI.indentLevel++;

            if (filteredStates.Count == 0)
            {
                EditorGUILayout.LabelField("No states found.", EditorStyles.miniLabel);
            }
            else
            {
                foreach (var state in filteredStates)
                {
                    var isActive = stateMachine.ActiveState?.Equals(state) ?? false;
                    StateDrawer.Draw(state, stateMachine, isActive, historyManager, transitionsFoldouts, historyFoldouts);
                }
            }

            EditorGUI.indentLevel--;
        }

        private static bool MatchesFilter(IIdentifiable item, string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return true;
            }

            return item.Name.Contains(filter, System.StringComparison.OrdinalIgnoreCase) ||
                   item.Id.Contains(filter, System.StringComparison.OrdinalIgnoreCase);
        }
    }
}
