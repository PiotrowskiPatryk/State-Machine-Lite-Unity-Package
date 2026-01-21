using Dev.Cortez.StateMachines.Core.Mono;
using Dev.Cortez.StateMachines.Core.Registry;
using Dev.Cortez.StateMachines.Editor.Debugger;
using UnityEditor;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Editor
{
    [CustomEditor(typeof(StateMachineContainerInstallerMono))]
    public class StateMachineContainerInstallerEditor : UnityEditor.Editor
    {
        private bool IsInstalled => serializedObject.FindProperty("_isInstalled").boolValue;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Show debugger window"))
            {
                var stateMachineContainerInstaller = (StateMachineContainerInstallerMono)serializedObject.targetObject;

                StateMachineDebuggerEditorWindow.ShowWindow(stateMachineContainerInstaller);
            }
        }
    }
}