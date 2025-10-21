using UnityEditor;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Utilities
{
    public static class EditorWindowUtilities
    {
        public static Rect GetCenteredPosition(Vector2 size)
        {
            var main = EditorGUIUtility.GetMainWindowPosition();
            var x = main.x + (main.width - size.x) * 0.5f;
            var y = main.y + (main.height - size.y) * 0.5f;

            return new Rect(x, y, size.x, size.y);
        }
    }
}