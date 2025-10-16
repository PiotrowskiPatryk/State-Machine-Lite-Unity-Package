using Dev.Cortez.StateMachines.StateMachineEditor.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(TriggerDefinition))]
    public sealed class TriggerDefinitionPropertyDrawer : PropertyDrawer
    {
        [SerializeField]
        private VisualTreeAsset _visualTreeAsset;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return _visualTreeAsset.CloneTree();
        }
    }
}
