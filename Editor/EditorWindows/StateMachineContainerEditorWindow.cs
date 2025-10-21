using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.EditorWindows
{
    [CustomEditor(typeof(StateMachineContainer))]
    public class StateMachineContainerEditorWindow : UnityEditor.Editor
    {
        [SerializeField]
        private VisualTreeAsset _visualTreeAsset;

        public override VisualElement CreateInspectorGUI()
        {
            return _visualTreeAsset.CloneTree();
        }
    }
}