using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Data.Definition;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Windows
{
    public class StateMachineMenuWindow : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset _visualTreeAsset;

        public static void Show(StateMachineDefinition stateMachineDefinition)
        {
            var window = CreateInstance<TriggerForm>();
            window.titleContent = new GUIContent("State machine menu");
            window.minSize = new Vector2(640, 320);
            window.position = EditorWindowUtilities.GetCenteredPosition(new Vector2(640, 320));
            window.ShowUtility();
        }
    }
}