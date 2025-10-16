using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.UIToolkitUtilities
{
    /// <summary>
    /// Small helpers to reduce boilerplate around creating a ScriptableObject wrapper,
    /// binding a SerializedObject, and adding a PropertyField to a container.
    /// </summary>
    public static class FormBindingUtility
    {
        public static TWrapper CreateWrapper<TWrapper>(out SerializedObject so, out SerializedProperty dataProperty)
            where TWrapper : ScriptableObject
        {
            var wrapper = ScriptableObject.CreateInstance<TWrapper>();
            so = new SerializedObject(wrapper);
            dataProperty = so.FindProperty("Data");
            return wrapper;
        }

        public static PropertyField AddPropertyField(VisualElement container, SerializedObject so, SerializedProperty property, string name = null)
        {
            var field = new PropertyField(property)
            {
                name = string.IsNullOrEmpty(name) ? (property != null ? property.propertyPath : "PropertyField") : name
            };
            field.style.flexGrow = 1f;
            field.Bind(so);
            container.Add(field);
            return field;
        }
    }
}
