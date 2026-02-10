using System;
using Dev.Cortez.StateMachines.Core.Utilities;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Utilities
{
    /// <summary>
    /// Editor-specific wrapper for ScriptGuidUtility.
    /// Provides type-to-GUID resolution for the Editor assembly.
    /// </summary>
    public static class ScriptGuidResolver
    {
        /// <summary>
        /// Gets the GUID of the MonoScript that defines the given type.
        /// </summary>
        public static string GetGuidForType(Type type)
        {
            return ScriptGuidUtility.GetGuidForType(type);
        }

        /// <summary>
        /// Resolves a script GUID to the Type it defines.
        /// </summary>
        public static Type GetTypeFromGuid(string guid)
        {
            return ScriptGuidUtility.GetTypeFromGuid(guid);
        }

        /// <summary>
        /// Updates the typeName based on the stored GUID.
        /// Returns the resolved type name (AssemblyQualifiedName), or the fallback if resolution fails.
        /// </summary>
        public static string ResolveTypeName(string guid, string fallbackTypeName)
        {
            var resolvedType = ScriptGuidUtility.GetTypeFromGuid(guid);

            return resolvedType?.AssemblyQualifiedName ?? fallbackTypeName;
        }
    }
}

