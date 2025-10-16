using Dev.Cortez.StateMachines.StateMachineEditor.Data;
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
           var root = _visualTreeAsset.CloneTree();
           var listView = new ListView();
           
           return root;
        }
    }
}
