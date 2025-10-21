using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.UIToolkitUtilities
{
    /// <summary>
    ///     Generic utility for creating a Type picker VisualElement for UIToolkit.
    ///     It lists all non-abstract, non-generic, concrete types assignable to the provided base type.
    /// </summary>
    public static class TypePickerUtility
    {
        /// <summary>
        ///     Creates a dropdown field that allows picking a concrete type assignable to TBase.
        /// </summary>
        /// <typeparam name="TBase">The base type or interface to filter by.</typeparam>
        /// <param name="current">Currently selected type (can be null).</param>
        /// <param name="onChanged">Invoked when user picks a different type.</param>
        /// <param name="label">Optional label for the field.</param>
        /// <param name="additionalFilter">Optional additional filter for types.</param>
        /// <param name="sorter">Optional custom sorter for items.</param>
        /// <returns>DropdownField bound to the provided values.</returns>
        public static DropdownField CreateForBase<TBase>(Type current, Action<Type> onChanged, string label = null,
            Func<Type, bool> additionalFilter = null,
            Comparison<TypeItem> sorter = null)
        {
            var items = GetAssignableConcreteTypes<TBase>(additionalFilter).
                Select(t => new TypeItem(GetNiceDisplayName(t), t)).ToList();

            // Add None option at the top
            items.Insert(0, new TypeItem("None", null));

            // If the current type is not in the discovered list (e.g., not yet loaded by TypeCache),
            // inject it so the dropdown can display the existing value instead of defaulting to None.
            if (current != null && items.All(i => i.Type != current))
            {
                items.Insert(1, new TypeItem(GetNiceDisplayName(current), current));
            }

            // Optional external sorter over the final list (excluding the fixed None at index 0)
            if (sorter != null && items.Count > 1)
            {
                var none = items[0];
                var rest = items.Skip(1).ToList();
                rest.Sort(sorter);
                items = new List<TypeItem> { none };
                items.AddRange(rest);
            }

            var choices = items.Select(i => i.DisplayName).ToList();
            var dropdown = new DropdownField(label ?? "Type", choices, 0) { name = "TypeDropdown" };

            // Set initial value
            var selectedIndex = 0;

            if (current != null)
            {
                var found = items.FindIndex(i => i.Type == current);

                if (found >= 0)
                {
                    selectedIndex = found;
                }
            }

            dropdown.index = selectedIndex;

            dropdown.RegisterValueChangedCallback(evt =>
            {
                var idx = dropdown.index;

                if (idx < 0 || idx >= items.Count)
                {
                    return;
                }

                onChanged?.Invoke(items[idx].Type);
            });

            // Store items in userData for optional external inspection
            dropdown.userData = items;

            return dropdown;
        }

        /// <summary>
        ///     Retrieves all concrete types assignable to TBase using UnityEditor.TypeCache for performance.
        /// </summary>
        public static List<Type> GetAssignableConcreteTypes<TBase>(Func<Type, bool> additionalFilter = null)
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
            list.Sort((a, b) => string.CompareOrdinal(GetNiceDisplayName(a), GetNiceDisplayName(b)));

            return list;
        }

        public static string GetNiceDisplayName(Type type)
        {
            if (type == null)
            {
                return "None";
            }

            var ns = type.Namespace;
            var name = type.Name;

            return string.IsNullOrEmpty(ns) ? name : $"{ns}.{name}";
        }

        /// <summary>
        ///     Tries to set a SerializedProperty that stores a System.Type value.
        ///     Prefers boxedValue when supported; otherwise falls back to storing AssemblyQualifiedName in a string field.
        /// </summary>
        public static bool TrySetTypeOnProperty(SerializedProperty typeProp, Type selectedType)
        {
            if (typeProp == null)
            {
                return false;
            }

            // Prefer explicit handling for string-backed fields (most common in Unity, since System.Type isn't serializable)
            if (typeProp.propertyType == SerializedPropertyType.String)
            {
                typeProp.stringValue = selectedType != null ? selectedType.AssemblyQualifiedName : string.Empty;
                typeProp.serializedObject.ApplyModifiedProperties();

                return true;
            }

            // Fallback: try boxedValue for managed-reference like fields if Unity supports it in this context
            try
            {
                typeProp.boxedValue = selectedType;
                typeProp.serializedObject.ApplyModifiedProperties();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Tries to get a System.Type from a SerializedProperty prioritizing string-backed storage; otherwise
        ///     falls back to boxedValue when supported. Includes robust resolution across loaded assemblies.
        /// </summary>
        public static Type TryGetTypeFromProperty(SerializedProperty typeProp)
        {
            if (typeProp == null)
            {
                return null;
            }

            // Prefer string-backed retrieval first to avoid boxedValue pitfalls on string properties
            if (typeProp.propertyType == SerializedPropertyType.String)
            {
                var aqn = typeProp.stringValue;

                if (string.IsNullOrWhiteSpace(aqn))
                {
                    return null;
                }

                // 1) Direct AQN lookup
                var resolved = Type.GetType(aqn);

                if (resolved != null)
                {
                    return resolved;
                }

                // 2) Try without assembly part (FullName only)
                var nameOnly = aqn;
                var commaIdx = aqn.IndexOf(',');

                if (commaIdx > 0)
                {
                    nameOnly = aqn.Substring(0, commaIdx).Trim();
                }

                resolved = Type.GetType(nameOnly);

                if (resolved != null)
                {
                    return resolved;
                }

                // 3) Scan loaded assemblies for matches by AQN or FullName
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();

                foreach (var asm in assemblies)
                {
                    try
                    {
                        // Fast path: assembly-local lookup by full name
                        var t = asm.GetType(nameOnly, false, false);

                        if (t != null)
                        {
                            return t;
                        }

                        // Fallback: enumerate types and match more strictly
                        foreach (var candidate in asm.GetTypes())
                        {
                            var candAqn = candidate.AssemblyQualifiedName;

                            if (!string.IsNullOrEmpty(candAqn) && string.Equals(candAqn, aqn, StringComparison.Ordinal))
                            {
                                return candidate;
                            }

                            if (string.Equals(candidate.FullName, aqn, StringComparison.Ordinal) ||
                                string.Equals(candidate.FullName, nameOnly, StringComparison.Ordinal))
                            {
                                return candidate;
                            }
                        }
                    }
                    catch
                    {
                        // Ignore reflection/type load exceptions from problematic assemblies
                    }
                }

                // If still not found, return null
                return null;
            }

            // Non-string property path: try boxedValue
            try
            {
                var obj = typeProp.boxedValue;

                return obj as Type;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public sealed class TypeItem
        {
            public string DisplayName { get; }
            public Type Type { get; }

            public TypeItem(string displayName, Type type)
            {
                DisplayName = displayName;
                Type = type;
            }

            public override string ToString()
            {
                return DisplayName;
            }
        }
    }
}