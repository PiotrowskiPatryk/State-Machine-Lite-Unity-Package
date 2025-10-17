using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Data;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Windows;
using UnityEditor;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Utilities
{
    /// <summary>
    /// Utilities for reading/writing TriggerDefinition fields from a SerializedProperty and opening edit forms.
    /// Rolled back to self-contained implementation without external generic utility dependency.
    /// </summary>
    public static class TriggerDefinitionEditorUtility
    {
        public static void WriteFromResult(SerializedProperty elementProp, TriggerForm.Result result)
        {
            if (elementProp == null || result == null) return;

            var idProp = elementProp.FindPropertyRelative(TriggerDefinitionPropertyNames.Id);
            var nameProp = elementProp.FindPropertyRelative(TriggerDefinitionPropertyNames.Name);
            var descriptionProp = elementProp.FindPropertyRelative(TriggerDefinitionPropertyNames.Description);
            var typeNameProp = elementProp.FindPropertyRelative(TriggerDefinitionPropertyNames.TypeName);
            var payloadProp = elementProp.FindPropertyRelative(TriggerDefinitionPropertyNames.Payload);

            if (idProp != null) idProp.stringValue = result.Id ?? string.Empty;
            if (nameProp != null) nameProp.stringValue = result.Name ?? string.Empty;
            if (descriptionProp != null) descriptionProp.stringValue = result.Description ?? string.Empty;
            if (typeNameProp != null) typeNameProp.stringValue = result.TypeName ?? string.Empty;

            if (payloadProp != null)
            {
                try { payloadProp.managedReferenceValue = result.Payload; } catch { /* ignore for older Unity versions */ }
            }
        }

        public static void ReadToArgs(SerializedProperty elementProp, out string id, out string name, out string description, out string typeName, out IPayload payload)
        {
            id = name = description = typeName = string.Empty;
            payload = null;
            if (elementProp == null) return;

            var idProp = elementProp.FindPropertyRelative(TriggerDefinitionPropertyNames.Id);
            var nameProp = elementProp.FindPropertyRelative(TriggerDefinitionPropertyNames.Name);
            var descriptionProp = elementProp.FindPropertyRelative(TriggerDefinitionPropertyNames.Description);
            var typeNameProp = elementProp.FindPropertyRelative(TriggerDefinitionPropertyNames.TypeName);
            var payloadProp = elementProp.FindPropertyRelative(TriggerDefinitionPropertyNames.Payload);

            if (idProp != null) id = idProp.stringValue;
            if (nameProp != null) name = nameProp.stringValue;
            if (descriptionProp != null) description = descriptionProp.stringValue;
            if (typeNameProp != null) typeName = typeNameProp.stringValue;
            if (payloadProp != null)
            {
                try { payload = payloadProp.managedReferenceValue as IPayload; } catch { payload = null; }
            }
        }

        public static void OpenEditForm(SerializedProperty arrayProp, int index, System.Action<TriggerForm.Result> onSaved)
        {
            if (arrayProp == null || index < 0 || index >= arrayProp.arraySize) return;
            var element = arrayProp.GetArrayElementAtIndex(index);
            ReadToArgs(element, out var id, out var name, out var description, out var typeName, out var payload);
            TriggerForm.ShowEdit(id, name, description, typeName, payload, onSaved);
        }
    }
}
