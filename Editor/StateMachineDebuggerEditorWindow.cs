using Dev.Cortez.StateMachines.Core.Mono;
using UnityEditor;

namespace Dev.Cortez.StateMachines.Editor
{
    public class StateMachineDebuggerEditorWindow : EditorWindow
    {
        private StateMachineContainerInstallerMono _stateMachineContainerInstaller;

        public static void ShowWindow(StateMachineContainerInstallerMono stateMachineContainerInstallerMono)
        {
            var window = GetWindow<StateMachineDebuggerEditorWindow>();
            window._stateMachineContainerInstaller = stateMachineContainerInstallerMono;
            window.Show();
        }
    }
}