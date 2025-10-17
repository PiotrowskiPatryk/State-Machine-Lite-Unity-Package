using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(StateMachineConfiguration))]
    public class StateMachineConfigurationPropertyDrawer : PropertyDrawer
    {
        [SerializeField]
        private VisualTreeAsset _visualTreeAsset;
        
        private SerializedObject _serializedObject;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = _visualTreeAsset.CloneTree();
            var listView = root.Q<MultiColumnListView>("ListView");
            var newStateMachineButton = root.Q<Button>("NewStateMachineButton");
            var stateMachinesProperty = property.FindPropertyRelative("_stateMachines");
            
            return root;
        }
    }
}