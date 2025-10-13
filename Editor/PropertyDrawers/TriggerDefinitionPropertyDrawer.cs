using Dev.Cortez.StateMachines.Core.Configuration;
using UnityEditor;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(TriggerDefinition))]
    public class TriggerDefinitionPropertyDrawer : PropertyDrawer
    {
        private const string PATH = "Packages/dev.cortez.state-machines/Runtime/Visuals/PropertyDrawers/TriggerDefinitionWindow.uxml";
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var instance = (VisualTreeAsset) EditorGUIUtility.Load(PATH);
            
            return instance.Instantiate();
        }
    }
}
