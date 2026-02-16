#region

using System;
using System.Collections.Generic;
using System.Linq;
using Dev.Cortez.StateMachines.Core.Attributes;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Utilities;
using UnityEngine.UIElements;

#endregion

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Views.VisualElements
{
    [UxmlElement]
    public partial class TypePickerDropdownField : DropdownField
    {
        private const string NoneLabel = "(None)";

        private readonly List<Type> _types = new();
        private readonly Dictionary<string, Type> _displayToType = new(StringComparer.Ordinal);
        private readonly Dictionary<Type, string> _typeToDisplay = new();
        private readonly Dictionary<string, Type> _assemblyQualifiedNameToType = new(StringComparer.Ordinal);

        [UxmlAttribute("base-type")]
        public string BaseTypeName { get; set; }

        /// <summary>
        ///     Gets the currently selected Type, or null if "(None)" is selected.
        /// </summary>
        public Type SelectedType => _displayToType.TryGetValue(value, out var t) ? t : null;

        /// <summary>
        ///     Gets the AssemblyQualifiedName of the currently selected type, for storage.
        ///     Returns null if no type is selected.
        /// </summary>
        public string SelectedTypeAssemblyQualifiedName => SelectedType?.AssemblyQualifiedName;

        public TypePickerDropdownField()
        {
            RegisterCallback<AttachToPanelEvent>(OnAttachedToPanel);
        }

        public void ChangeType(Type newType, Type initialSelection = null)
        {
            BaseTypeName = newType?.AssemblyQualifiedName;
            TryPopulate(initialSelection);
        }

        /// <summary>
        ///     Sets the selected type by its AssemblyQualifiedName.
        ///     Use this when loading a stored type name.
        /// </summary>
        /// <param name="assemblyQualifiedName">The AssemblyQualifiedName of the type to select.</param>
        internal void SetSelectionByAssemblyQualifiedName(string assemblyQualifiedName)
        {
            if (string.IsNullOrEmpty(assemblyQualifiedName))
            {
                SetValueWithoutNotify(NoneLabel);

                return;
            }

            if (_assemblyQualifiedNameToType.TryGetValue(assemblyQualifiedName, out var type) &&
                _typeToDisplay.TryGetValue(type, out var display))
            {
                SetValueWithoutNotify(display);
            }
            else
            {
                SetValueWithoutNotify(NoneLabel);
            }
        }

        public void SetTypes(IEnumerable<Type> types, Type initialSelection = null)
        {
            _types.Clear();
            _displayToType.Clear();
            _typeToDisplay.Clear();
            _assemblyQualifiedNameToType.Clear();
            choices.Clear();

            var concrete = types.Where(t => t is { IsAbstract: false, IsGenericTypeDefinition: false }).Distinct().
                ToList();

            _types.AddRange(concrete);

            // Build reverse lookup for AssemblyQualifiedName -> Type
            foreach (var t in _types)
            {
                if (!string.IsNullOrEmpty(t.AssemblyQualifiedName))
                {
                    _assemblyQualifiedNameToType[t.AssemblyQualifiedName] = t;
                }
            }

            // Build labels: short names, disambiguate duplicates with namespace in parentheses.
            var byShort = _types.GroupBy(type => type.Name);

            foreach (var group in byShort)
            {
                var hasCollision = group.Count() > 1;

                foreach (var type in group)
                {
                    var displayLabel = ResolveTypeNameToDisplay(type, hasCollision);

                    _typeToDisplay[type] = displayLabel;
                    _displayToType[displayLabel!] = type;
                }
            }

            // Sort by label for a clean list.
            var items = _types.Select(t => _typeToDisplay[t]).Distinct().OrderBy(s => s, StringComparer.Ordinal).
                ToList();

            var labels = new List<string> { NoneLabel };
            labels.AddRange(items);
            choices = labels;

            // Apply initial selection using the display label.
            if (initialSelection != null && _typeToDisplay.TryGetValue(initialSelection, out var display))
            {
                SetValueWithoutNotify(display);
            }
            else
            {
                SetValueWithoutNotify(NoneLabel);
            }
        }

        private static string ResolveTypeNameToDisplay(Type type, bool hasCollision)
        {
            if (type.IsDefined(typeof(DisplayAsAttribute), false))
            {
                var customAttribute =
                    type.GetCustomAttributes(typeof(DisplayAsAttribute), false).FirstOrDefault() as
                        DisplayAsAttribute;

                return customAttribute.CustomName;
            }

            var namespaceName = string.IsNullOrEmpty(type.Namespace) ? "global" : type.Namespace;

            var displayName = hasCollision
                ? $"{type.Name} ({namespaceName})"
                : type.Name;

            return displayName;
        }

        private void TryPopulate(Type initialSelection = null)
        {
            if (string.IsNullOrEmpty(BaseTypeName))
            {
                return;
            }

            var baseType = Type.GetType(BaseTypeName, false)
                           ?? AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType(BaseTypeName, false)).
                               FirstOrDefault(t => t != null)
                           ?? typeof(object);

            // Use StateMachineReflectionUtilities for proper generic type resolution
            var assignables = StateMachineReflectionUtilities.GetConcreteTypeCandidates(baseType);

            // Optionally include baseType itself if concrete and not excluded
            if (!baseType.IsAbstract && !baseType.IsInterface && !baseType.IsGenericTypeDefinition &&
                !baseType.IsDefined(typeof(ExcludeFromTypePickerAttribute), false))
            {
                assignables = assignables.Append(baseType);
            }

            SetTypes(assignables, initialSelection);
        }

        private void OnAttachedToPanel(AttachToPanelEvent attachToPanelEvent)
        {
            if (BaseTypeName != null && SelectedType == null)
            {
                TryPopulate();
            }
        }
    }
}