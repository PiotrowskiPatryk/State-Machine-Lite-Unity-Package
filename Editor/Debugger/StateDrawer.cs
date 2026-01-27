using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Dev.Cortez.StateMachines.Core.Data;
using Dev.Cortez.StateMachines.Core.Interfaces;
using UnityEditor;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Editor.Debugger
{
    public static class StateDrawer
    {
        public static void Draw(
            IState state,
            IStateMachine parentStateMachine,
            bool isCurrentActive,
            DebuggerHistoryManager historyManager,
            Dictionary<string, bool> transitionsFoldouts,
            Dictionary<string, bool> historyFoldouts)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            DrawHeader(state);
            DrawDetails(state);
            DrawTransitionsFoldout(state, parentStateMachine, historyManager, transitionsFoldouts, historyFoldouts);
            DrawActivateButton(state, parentStateMachine, isCurrentActive);
            HistoryDrawer.Draw($"state_{state.Id}", state.Name, historyManager, historyFoldouts);

            EditorGUILayout.EndVertical();
        }

        private static void DrawHeader(IState state)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(state.Name, EditorStyles.boldLabel);
            DrawStatus(state.StateStatus, state.IsActive);
            EditorGUILayout.EndHorizontal();
        }

        private static void DrawStatus(StateStatus status, bool isActive)
        {
            var previousColor = GUI.backgroundColor;

            GUI.backgroundColor = status switch
            {
                StateStatus.Active => Color.green,
                StateStatus.Activating => Color.yellow,
                StateStatus.Deactivating => Color.yellow,
                StateStatus.Inactive => Color.gray,
                _ => Color.red
            };

            var icon = isActive ? "●" : "○";
            GUILayout.Label($"{icon} {status}", EditorStyles.miniButton, GUILayout.Width(100));
            GUI.backgroundColor = previousColor;
        }

        private static void DrawDetails(IState state)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("ID", state.Id, EditorStyles.miniLabel);

            if (!string.IsNullOrEmpty(state.Description))
            {
                EditorGUILayout.LabelField("Description", state.Description, EditorStyles.wordWrappedMiniLabel);
            }

            EditorGUILayout.LabelField("Status", state.StateStatus.ToString());
            EditorGUI.indentLevel--;
        }

        private static void DrawTransitionsFoldout(
            IState state,
            IStateMachine stateMachine,
            DebuggerHistoryManager historyManager,
            Dictionary<string, bool> transitionsFoldouts,
            Dictionary<string, bool> historyFoldouts)
        {
            var key = $"trans_{state.Id}";
            transitionsFoldouts.TryAdd(key, false);

            var transitions = stateMachine.TransitionRules?.Where(r => r.CurrentState.Equals(state)).ToList() ?? new List<TransitionRule>();

            EditorGUILayout.BeginHorizontal();
            transitionsFoldouts[key] = EditorGUILayout.Foldout(transitionsFoldouts[key], $"Transitions ({transitions.Count})", true);
            EditorGUILayout.EndHorizontal();

            if (!transitionsFoldouts[key])
            {
                return;
            }

            EditorGUI.indentLevel++;

            if (transitions.Count == 0)
            {
                EditorGUILayout.LabelField("No outgoing transitions.", EditorStyles.miniLabel);
            }
            else
            {
                foreach (var rule in transitions)
                {
                    TransitionDrawer.Draw(rule, historyManager, historyFoldouts);
                }
            }

            EditorGUI.indentLevel--;
        }

        private static void DrawActivateButton(IState state, IStateMachine parentStateMachine, bool isCurrentActive)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUI.enabled = Application.isPlaying && !isCurrentActive;

            if (GUILayout.Button("Activate State", GUILayout.Width(100)))
            {
                parentStateMachine.MoveToStateAsync(state, CancellationToken.None);
            }

            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
        }
    }
}
