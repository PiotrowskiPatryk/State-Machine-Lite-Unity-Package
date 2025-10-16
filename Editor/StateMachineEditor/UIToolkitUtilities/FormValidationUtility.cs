using Dev.Cortez.StateMachines.Core.Interfaces;
using UnityEditor;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.UIToolkitUtilities
{
    /// <summary>
    /// Simple validators for editor forms.
    /// </summary>
    public static class FormValidationUtility
    {
        public static bool RequiredString(string value, string fieldName, out string message)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                message = fieldName + " is required.";
                return false;
            }
            message = null;
            return true;
        }

        public static bool RequiredType(string typeName, out string message)
        {
            if (string.IsNullOrWhiteSpace(typeName))
            {
                message = "Type is required.";
                return false;
            }
            message = null;
            return true;
        }

        public static bool ValidatePayload(IPayload payload, out string message)
        {
            if (payload == null)
            {
                message = null; // Payload is optional unless trigger type demands it; drawer ensures presence when needed.
                return true;
            }

            try
            {
                if (!payload.IsValid())
                {
                    message = "Payload is invalid.";
                    return false;
                }
            }
            catch
            {
                // In case payload implements a non-throwing IsValid, treat as valid.
            }

            message = null;
            return true;
        }
    }
}
