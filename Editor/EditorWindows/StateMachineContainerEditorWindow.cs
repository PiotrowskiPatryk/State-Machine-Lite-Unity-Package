using Dev.Cortez.StateMachines.Core.Configuration;
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

           var tabView = root.Q<TabView>();
           
           return root;
        }
    }
}
