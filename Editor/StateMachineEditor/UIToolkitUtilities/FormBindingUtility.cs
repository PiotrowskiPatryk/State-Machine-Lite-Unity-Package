using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.UIToolkitUtilities
{
    /// <summary>
    ///     Small helpers to reduce boilerplate around creating a ScriptableObject wrapper,
    ///     binding a SerializedObject, and adding a PropertyField to a container.
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

        public static (TWrapper, SerializedObject) CreateWrapper<TWrapper>(SerializedProperty sourceProperty)
            where TWrapper : ScriptableObject
        {
            var wrapper = ScriptableObject.CreateInstance<TWrapper>();
            var so = new SerializedObject(wrapper);

            // Ensure the SO is up-to-date before touching properties
            so.Update();

            var destinationProperty = so.FindProperty("Data");

            if (destinationProperty == null)
            {
                Debug.LogError($"{typeof(TWrapper).Name} must declare a serializable field named 'Data'.");

                return (wrapper, so);
            }

            // If the source is a managed reference, prefer the dedicated API
            if (destinationProperty.propertyType == SerializedPropertyType.ManagedReference &&
                sourceProperty.propertyType == SerializedPropertyType.ManagedReference)
            {
                // This copies the referenced instance (by value) into the dst managed reference slot
                destinationProperty.managedReferenceValue = sourceProperty.managedReferenceValue;
            }
            else if (destinationProperty.propertyType == SerializedPropertyType.ObjectReference &&
                     sourceProperty.propertyType == SerializedPropertyType.ObjectReference)
            {
                // This copies the UnityEngine.Object reference
                destinationProperty.objectReferenceValue = sourceProperty.objectReferenceValue;
            }
            else
            {
                // Works for most serializable value types and structs
                // (Unity 2020.3+/2021+; on very old versions prefer type-specific setters)
                destinationProperty.boxedValue = sourceProperty.boxedValue;
            }

            so.ApplyModifiedPropertiesWithoutUndo();
            so.Update();

            return (wrapper, so);
        }

        public static PropertyField AddPropertyField(VisualElement container, SerializedObject so,
            SerializedProperty property, string name = null)
        {
            var field = new PropertyField(property)
            {
                name = string.IsNullOrEmpty(name) ? property != null ? property.propertyPath : "PropertyField" : name,
                style =
                {
                    flexGrow = 1f
                }
            };
            field.Bind(so);
            container.Add(field);

            return field;
        }
    }
}