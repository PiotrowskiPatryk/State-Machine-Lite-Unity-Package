using Dev.Cortez.StateMachines.StateMachineEditor.Data;
using UnityEditor;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(TriggerConfiguration))]
    public class TriggerConfigurationPropertyDrawer : PropertyDrawer
    {
        private const string PATH = "Packages/dev.cortez.state-machines/Runtime/Visuals/PropertyDrawers/TriggersMenuWindow.uxml";
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var instance = (VisualTreeAsset) EditorGUIUtility.Load(PATH);
            
            return instance.Instantiate();
        }
    }
}
