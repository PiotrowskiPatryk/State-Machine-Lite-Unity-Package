using Dev.Cortez.StateMachines.Core.Data;
using UnityEditor;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.PropertyDrawer
{
    [CustomPropertyDrawer(typeof(EmptyPayload))]
    public class EmptyPayloadPropertyDrawer : UnityEditor.PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new VisualElement();
        }
    }
}