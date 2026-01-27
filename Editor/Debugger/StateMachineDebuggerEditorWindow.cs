using System;
using System.Collections.Generic;
using System.Linq;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Core.Mono;
using UnityEditor;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Editor.Debugger
{
    public sealed class StateMachineDebuggerEditorWindow : EditorWindow
    {
        private const string WINDOW_TITLE = "State Machine Debugger";
        private const float REFRESH_INVERVAL_IN_SECONDS = 0.5f;

        private readonly DebuggerHistoryManager _historyManager = new();
        private readonly Dictionary<string, bool> _statesFoldouts = new();
        private readonly Dictionary<string, bool> _transitionsFoldouts = new();
        private readonly Dictionary<string, bool> _historyFoldouts = new();

        private StateMachineContainerInstallerMono _containerInstaller;
        private Vector2 _scrollPosition;
        private string _searchFilter = "";
        private double _lastRepaintTime;
        private DebuggerEventSubscriber _eventSubscriber;
        private bool _triggersFoldout = true;
        private bool _stateMachinesFoldout = true;

        private bool CanRepaint => EditorApplication.timeSinceStartup - _lastRepaintTime > REFRESH_INVERVAL_IN_SECONDS;

        public static void ShowWindow(StateMachineContainerInstallerMono containerInstaller)
        {
            var window = GetWindow<StateMachineDebuggerEditorWindow>(WINDOW_TITLE);
            window._containerInstaller = containerInstaller;
            window.Show();
        }

        private void OnEnable()
        {
            titleContent = new GUIContent(WINDOW_TITLE);
            _eventSubscriber = new DebuggerEventSubscriber(_historyManager);
            TrySubscribe();
        }

        private void OnDisable()
        {
            _eventSubscriber?.Unsubscribe();
        }

        private void OnInspectorUpdate()
        {
            if (!CanRepaint)
            {
                return;
            }

            _lastRepaintTime = EditorApplication.timeSinceStartup;

            CheckContainerChanged();
            Repaint();
        }

        private void CheckContainerChanged()
        {
            var currentContainerEntry = IsContainerValid() ? _containerInstaller.StateMachineContainerEntry : null;
            var subscribedEntry = _eventSubscriber?.SubscribedContainerEntry;

            var containerChanged = currentContainerEntry != subscribedEntry;

            if (containerChanged && _eventSubscriber.IsSubscribed)
            {
                _eventSubscriber.Unsubscribe();
            }

            if (IsContainerValid() && !_eventSubscriber.IsSubscribed)
            {
                TrySubscribe();
            }
        }

        private void OnGUI()
        {
            if (!IsContainerValid())
            {
                DrawNoContainerMessage();

                return;
            }

            DrawSearchFilter();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            DrawTriggersSection();
            EditorGUILayout.Space(10);
            DrawStateMachinesSection();
            EditorGUILayout.EndScrollView();
        }

        private bool IsContainerValid()
        {
            return _containerInstaller && _containerInstaller.StateMachineContainerEntry != null;
        }

        private void TrySubscribe()
        {
            if (IsContainerValid())
            {
                _eventSubscriber.Subscribe(_containerInstaller.StateMachineContainerEntry);
            }
        }

        private void DrawNoContainerMessage()
        {
            EditorGUILayout.HelpBox(
                "No State Machine Container is assigned or installed.\nPlease assign a StateMachineContainerInstallerMono and ensure it is installed.",
                MessageType.Warning);
        }

        private void DrawSearchFilter()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Search:", GUILayout.Width(50));
            _searchFilter = EditorGUILayout.TextField(_searchFilter);

            if (GUILayout.Button("Clear", GUILayout.Width(50)))
            {
                _searchFilter = "";
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(5);
        }

        private void DrawTriggersSection()
        {
            var entry = _containerInstaller.StateMachineContainerEntry;
            var filtered = entry.Triggers.Values.Where(MatchesFilter).ToList();

            _triggersFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_triggersFoldout,
                $"Triggers ({filtered.Count}/{entry.Triggers.Count})");

            if (_triggersFoldout)
            {
                EditorGUI.indentLevel++;

                if (filtered.Count == 0)
                {
                    EditorGUILayout.LabelField("No triggers found.", EditorStyles.miniLabel);
                }
                else
                {
                    foreach (var trigger in filtered)
                    {
                        TriggerDrawer.Draw(trigger, _historyManager, _historyFoldouts);
                    }
                }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void DrawStateMachinesSection()
        {
            var entry = _containerInstaller.StateMachineContainerEntry;
            var filtered = entry.StateMachines.Values.Where(MatchesFilter).ToList();

            _stateMachinesFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_stateMachinesFoldout,
                $"State Machines ({filtered.Count}/{entry.StateMachines.Count})");

            if (_stateMachinesFoldout)
            {
                EditorGUI.indentLevel++;

                if (filtered.Count == 0)
                {
                    EditorGUILayout.LabelField("No state machines found.", EditorStyles.miniLabel);
                }
                else
                {
                    foreach (var sm in filtered)
                    {
                        StateMachineDrawer.Draw(sm, _searchFilter, _historyManager, _statesFoldouts,
                            _transitionsFoldouts, _historyFoldouts);
                    }
                }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private bool MatchesFilter(IIdentifiable item)
        {
            if (string.IsNullOrEmpty(_searchFilter))
            {
                return true;
            }

            return item.Name.Contains(_searchFilter, StringComparison.OrdinalIgnoreCase) ||
                   item.Id.Contains(_searchFilter, StringComparison.OrdinalIgnoreCase);
        }
    }
}