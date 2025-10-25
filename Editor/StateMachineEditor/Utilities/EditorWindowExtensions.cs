using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Utilities
{
    public static class EditorWindowExtensions
    {
        public static bool TryBindSerializedPropertyToField(this EditorWindow editorWindow,
            [CanBeNull] string fieldName, [CanBeNull] SerializedProperty serializedProperty,
            [CanBeNull] string propertyPath)
        {
            if (serializedProperty == null)
            {
                Debug.LogError("Unable to bind field. Serialized property is null.");

                return false;
            }

            var root = editorWindow.rootVisualElement;

            if (root == null)
            {
                Debug.LogError("Unable to bind field. Root visual element is null.");

                return false;
            }

            var field = root.Q(fieldName);

            if (field == null)
            {
                Debug.LogError($"Unable to bind field. Field {fieldName} not found.");

                return false;
            }

            if (field is not IBindable bindable)
            {
                Debug.LogError($"Unable to bind field. Field {field} is not bindable.");

                return false;
            }

            var propertyRelativePath = serializedProperty.FindPropertyRelative(propertyPath);

            if (propertyRelativePath == null)
            {
                Debug.LogError("Unable to bind field. Property relative path is null.");

                return false;
            }

            bindable.BindProperty(propertyRelativePath);

            return true;
        }
    }
}