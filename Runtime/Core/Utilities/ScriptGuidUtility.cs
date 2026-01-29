using System;
using System.Collections.Generic;
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
    /// Includes caching for performance optimization.
    /// </summary>
    public static class ScriptGuidUtility
    {
        /// <summary>
        /// Gets the GUID of the MonoScript that defines the given type.
        /// Returns null if the type is not found in any MonoScript.
        /// Results are cached for performance.
        /// </summary>
        public static string GetGuidForType(Type type)
        {
#if UNITY_EDITOR
            if (type == null)
            {
                return null;
            }

            // Check cache first
            if (TypeToGuidCache.TryGetValue(type, out var cachedGuid))
            {
                return cachedGuid;
            }

            // Perform expensive lookup
            var scripts = AssetDatabase.FindAssets($"t:MonoScript {type.Name}");

            foreach (var guid in scripts)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);

                if (script != null && script.GetClass() == type)
                {
                    // Cache both directions
                    TypeToGuidCache[type] = guid;
                    GuidToTypeCache[guid] = type;

                    return guid;
                }
            }

            // Cache null result to avoid repeated lookups for types without scripts
            TypeToGuidCache[type] = null;

            return null;
#else
            return null;
#endif
        }

        /// <summary>
        /// Resolves a script GUID to the Type it defines.
        /// Returns null if the GUID is invalid or the script has no class.
        /// Results are cached for performance.
        /// </summary>
        public static Type GetTypeFromGuid(string guid)
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(guid))
            {
                return null;
            }

            // Check cache first
            if (GuidToTypeCache.TryGetValue(guid, out var cachedType))
            {
                return cachedType;
            }

            // Perform lookup
            var path = AssetDatabase.GUIDToAssetPath(guid);

            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            var script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
            var type = script?.GetClass();

            // Cache the result (including null)
            GuidToTypeCache[guid] = type;

            if (type != null)
            {
                TypeToGuidCache[type] = guid;
            }

            return type;
#else
            return null;
#endif
        }

        /// <summary>
        /// Clears all cached GUID/Type mappings.
        /// Call this when scripts are reimported or the project structure changes.
        /// </summary>
        public static void ClearCache()
        {
#if UNITY_EDITOR
            TypeToGuidCache.Clear();
            GuidToTypeCache.Clear();
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
#if UNITY_EDITOR
        private static readonly Dictionary<Type, string> TypeToGuidCache = new();
        private static readonly Dictionary<string, Type> GuidToTypeCache = new();
#endif
    }
}