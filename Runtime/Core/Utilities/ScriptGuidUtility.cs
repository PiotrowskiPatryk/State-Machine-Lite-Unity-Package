using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Dev.Cortez.StateMachines.Core.Utilities
{
    /// <summary>
    /// Utility class for resolving MonoScript GUIDs to Types and vice versa.
    /// This enables resilient type tracking even when classes are renamed or moved to different namespaces.
    /// Editor-only functionality wrapped in #if UNITY_EDITOR.
    /// </summary>
    public static class ScriptGuidUtility
    {
        /// <summary>
        /// Gets the GUID of the MonoScript that defines the given type.
        /// Returns null if the type is not found in any MonoScript.
        /// </summary>
        public static string GetGuidForType(Type type)
        {
#if UNITY_EDITOR
            if (type == null)
            {
                return null;
            }

            var scripts = AssetDatabase.FindAssets($"t:MonoScript {type.Name}");

            foreach (var guid in scripts)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);

                if (script != null && script.GetClass() == type)
                {
                    return guid;
                }
            }

            return null;
#else
            return null;
#endif
        }

        /// <summary>
        /// Resolves a script GUID to the Type it defines.
        /// Returns null if the GUID is invalid or the script has no class.
        /// </summary>
        public static Type GetTypeFromGuid(string guid)
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(guid))
            {
                return null;
            }

            var path = AssetDatabase.GUIDToAssetPath(guid);

            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            var script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);

            return script?.GetClass();
#else
            return null;
#endif
        }

        /// <summary>
        /// Attempts to recreate a payload instance from its GUID when the original reference was lost due to a type rename.
        /// </summary>
        /// <typeparam name="T">The interface type the payload must implement.</typeparam>
        /// <param name="payloadGuid">The stored GUID of the payload type.</param>
        /// <param name="currentPayload">The current payload reference (may be null if type was renamed).</param>
        /// <returns>The original payload if valid, or a new instance if recreated, or null if recovery failed.</returns>
        public static T TryRecoverPayload<T>(string payloadGuid, T currentPayload) where T : class
        {
#if UNITY_EDITOR
            if (currentPayload != null)
            {
                return currentPayload;
            }

            if (string.IsNullOrEmpty(payloadGuid))
            {
                return null;
            }

            var payloadType = GetTypeFromGuid(payloadGuid);

            if (payloadType == null || !typeof(T).IsAssignableFrom(payloadType))
            {
                return null;
            }

            try
            {
                var newPayload = (T)Activator.CreateInstance(payloadType);
                Debug.LogWarning(
                    $"[StateMachine] Payload type was renamed. Created new instance of {payloadType.Name}. " +
                    "Previous configuration data was lost.");

                return newPayload;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[StateMachine] Failed to recreate payload: {ex.Message}");

                return null;
            }
#else
            return currentPayload;
#endif
        }
    }
}
