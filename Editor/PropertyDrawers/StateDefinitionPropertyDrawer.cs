using Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.Data.Definition;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(StateDefinition))]
    public class StateDefinitionPropertyDrawer : PropertyDrawer
    {
        [SerializeField]
        private VisualTreeAsset _visualTreeAsset;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return _visualTreeAsset.CloneTree();
        }
    }
}