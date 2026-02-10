using System.Collections.Generic;
using Dev.Cortez.StateMachines.Core.Data;
using UnityEditor;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Editor.Debugger
{
    public static class TransitionDrawer
    {
        public static void Draw(
            TransitionRule rule,
            DebuggerHistoryManager historyManager,
            Dictionary<string, bool> historyFoldouts)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField($"→ {rule.TargetState.Name}", EditorStyles.boldLabel);

            DrawConditionStatus(rule.Condition.IsSatisfied);
            EditorGUILayout.LabelField($"Priority: {rule.Priority}", EditorStyles.miniLabel);

            var transitionKey = $"{rule.CurrentState.Name}->{rule.TargetState.Name}";
            HistoryDrawer.Draw($"cond_{rule.CurrentState.Id}_{rule.TargetState.Id}", transitionKey, historyManager, historyFoldouts);

            EditorGUILayout.EndVertical();
        }

        private static void DrawConditionStatus(bool isSatisfied)
        {
            var previousColor = GUI.backgroundColor;
            GUI.backgroundColor = isSatisfied ? Color.green : Color.gray;
            EditorGUILayout.LabelField(isSatisfied ? "● Condition Satisfied" : "○ Condition Not Satisfied");
            GUI.backgroundColor = previousColor;
        }
    }
}
