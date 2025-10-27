using System;
using System.Collections.Generic;
using System.Linq;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.UIToolkitUtilities;
using UnityEditor;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Utilities
{
    public static class ReflectionUtilities
    {
        public static List<TypePickerUtility.TypeItem> GetAssignableConcreteTypes<TBase>()
        {
            return ListAssignableConcreteTypes<TBase>().
                Select(t => new TypePickerUtility.TypeItem(GetDisplayName(t), t)).ToList();
        }

        private static string GetDisplayName(Type type)
        {
            if (type == null)
            {
                return "None";
            }

            return type.AssemblyQualifiedName;
        }

        /// <summary>
        ///     Retrieves all concrete types assignable to TBase using UnityEditor.TypeCache for performance.
        /// </summary>
        private static List<Type> ListAssignableConcreteTypes<TBase>(Func<Type, bool> additionalFilter = null)
        {
            var all = TypeCache.GetTypesDerivedFrom(typeof(TBase));
            var list = new List<Type>(all.Count);

            foreach (var t in all)
            {
                if (t.IsAbstract)
                {
                    continue;
                }

                if (t.IsInterface)
                {
                    continue;
                }

                if (t.IsGenericTypeDefinition)
                {
                    continue;
                }

                if (additionalFilter != null && !additionalFilter(t))
                {
                    continue;
                }

                list.Add(t);
            }

            // Also consider the base type itself if it is concrete and assignable (rare for interfaces)
            var baseType = typeof(TBase);

            if (!baseType.IsInterface && !baseType.IsAbstract && !baseType.IsGenericTypeDefinition)
            {
                list.Add(baseType);
            }

            // Sort by namespace then name for consistency
            list.Sort((a, b) => string.CompareOrdinal(GetDisplayName(a), GetDisplayName(b)));

            return list;
        }
    }
}