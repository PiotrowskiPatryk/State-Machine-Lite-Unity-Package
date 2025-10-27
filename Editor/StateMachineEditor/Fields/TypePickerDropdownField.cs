using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Fields
{
    [UxmlElement]
    public partial class TypePickerDropdownField : DropdownField
    {
        private const string NoneLabel = "(None)";

        private readonly List<Type> _types = new();
        private readonly Dictionary<string, Type> _displayToType = new(StringComparer.Ordinal);
        private readonly Dictionary<Type, string> _typeToDisplay = new();

        [UxmlAttribute("base-type")]
        public string BaseTypeName { get; set; }

        public Type SelectedType => _displayToType.TryGetValue(value, out var t) ? t : null;

        public TypePickerDropdownField()
        {
            RegisterCallback<AttachToPanelEvent>(OnAttachedToPanel);
        }

        public void ChangeType(Type newType, Type initialSelection = null)
        {
            BaseTypeName = newType?.AssemblyQualifiedName;
            TryPopulate(initialSelection);
        }

        public void SetTypes(IEnumerable<Type> types, Type initialSelection = null)
        {
            _types.Clear();
            _displayToType.Clear();
            _typeToDisplay.Clear();
            choices.Clear();

            var concrete = types.Where(t => t is { IsAbstract: false, IsGenericTypeDefinition: false }).Distinct().
                ToList();

            _types.AddRange(concrete);

            // Build labels: short names, disambiguate duplicates with namespace in parentheses.
            var byShort = _types.GroupBy(type => type.Name);

            foreach (var group in byShort)
            {
                var hasCollision = group.Count() > 1;

                foreach (var t in group)
                {
                    var label = hasCollision
                        ? $"{t.AssemblyQualifiedName} ({(string.IsNullOrEmpty(t.Namespace) ? "global" : t.Namespace)})"
                        : t.AssemblyQualifiedName;

                    _typeToDisplay[t] = label;
                    _displayToType[label] = t;
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

            var assignables = TypeCache.GetTypesDerivedFrom(baseType).
                Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition);

            // Optionally include baseType itself if concrete
            if (!baseType.IsAbstract && !baseType.IsInterface && !baseType.IsGenericTypeDefinition)
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