using Dev.Cortez.StateMachines.Core.Interfaces;
using UnityEditor;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Utilities
{
    /// <summary>
    /// Generic utilities for reading and writing a common Definition-like schema on a SerializedProperty.
    /// It operates purely on property names so it can be reused across different definitions (e.g., Triggers, States, etc.).
    /// </summary>
    public static class DefinitionEditorUtility
    {
        /// <summary>
        /// Writes provided values into child properties of the given element property using supplied property names.
        /// Safe against missing properties and older Unity versions that may throw on managedReference assignment.
        /// </summary>
        public static void WriteDefinition(
            SerializedProperty elementProp,
            string idPropName,
            string namePropName,
            string descriptionPropName,
            string typeNamePropName,
            string payloadPropName,
            string id,
            string name,
            string description,
            string typeName,
            IPayload payload)
        {
            if (elementProp == null) return;

            TrySetString(elementProp.FindPropertyRelative(idPropName), id);
            TrySetString(elementProp.FindPropertyRelative(namePropName), name);
            TrySetString(elementProp.FindPropertyRelative(descriptionPropName), description);
            TrySetString(elementProp.FindPropertyRelative(typeNamePropName), typeName);
            TrySetManagedReference(elementProp.FindPropertyRelative(payloadPropName), payload);
        }

        /// <summary>
        /// Reads common definition values from the element property into out parameters using the supplied property names.
        /// Missing properties are handled gracefully.
        /// </summary>
        public static void ReadDefinition(
            SerializedProperty elementProp,
            string idPropName,
            string namePropName,
            string descriptionPropName,
            string typeNamePropName,
            string payloadPropName,
            out string id,
            out string name,
            out string description,
            out string typeName,
            out IPayload payload)
        {
            id = name = description = typeName = string.Empty;
            payload = null;
            if (elementProp == null) return;

            id = elementProp.FindPropertyRelative(idPropName)?.stringValue;
            name = elementProp.FindPropertyRelative(namePropName)?.stringValue;
            description = elementProp.FindPropertyRelative(descriptionPropName)?.stringValue;
            typeName = elementProp.FindPropertyRelative(typeNamePropName)?.stringValue;
            var payloadProp = elementProp.FindPropertyRelative(payloadPropName);
            if (payloadProp != null)
            {
                try { payload = payloadProp.managedReferenceValue as IPayload; } catch { payload = null; }
            }
        }

        private static void TrySetString(SerializedProperty prop, string value)
        {
            if (prop == null) return;
            prop.stringValue = value ?? string.Empty;
        }

        private static void TrySetManagedReference(SerializedProperty prop, IPayload payload)
        {
            if (prop == null) return;
            try { prop.managedReferenceValue = payload; } catch { /* ignore for older Unity versions */ }
        }
    }
}
