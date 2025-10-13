using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dev.Cortez.StateMachines.Attributes;
using UnityEditor;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Editor.Editor
{
    [CustomPropertyDrawer(typeof(SerializeReferenceSelectorAttribute))]
    public class SerializeReferenceSelectorDrawer : PropertyDrawer
    {
        private const string NoneOption = "<None>";
    
        private static readonly Dictionary<Type, Type[]> _typeCache = new();

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = EditorGUIUtility.singleLineHeight;
            
            if (property.hasVisibleChildren && property.managedReferenceValue != null)
            {
                height += EditorGUIUtility.standardVerticalSpacing;
                height += GetChildrenHeight(property);
            }

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Resolve the base type for this field
            var selector = (SerializeReferenceSelectorAttribute)attribute;
            var baseType = selector.BaseType ?? fieldInfo.FieldType;

            if (baseType == null)
            {
                EditorGUI.HelpBox(position, "SerializeReferenceSelector: Unable to resolve base type.", MessageType.Error);
                EditorGUI.EndProperty();
                return;
            }

            // Discover assignable concrete classes
            var types = GetAssignableTypes(baseType);

            // Current type from managedReferenceFullTypename
            var currentType = GetCurrentReferenceType(property);

            // Build popup data
            var displayNames = new List<string> { NoneOption };
            var typeList = new List<Type> { null }; // index aligned with displayNames
            foreach (var t in types)
            {
                displayNames.Add(GetNiceTypeName(t));
                typeList.Add(t);
            }

            // Determine current index
            int currentIndex = 0; // None
            if (currentType != null)
            {
                currentIndex = Mathf.Max(0, typeList.IndexOf(currentType));
            }

            // --- Draw ---
            var line = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            var guiOptions = displayNames.Select(s => new GUIContent(s)).ToArray();
            var newIndex = EditorGUI.Popup(line, label, currentIndex, guiOptions);
            
            // If selection changed, assign new instance (or null)
            if (newIndex != currentIndex)
            {
                var selectedType = typeList[newIndex];
                if (selectedType == null)
                {
                    property.managedReferenceValue = null;
                }
                else
                {
                    try
                    {
                        property.managedReferenceValue = Activator.CreateInstance(selectedType);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"SerializeReferenceSelector: Could not instantiate {selectedType.FullName}: {e.Message}");
                    }
                }
            }

            // Draw children of the managed reference instance
            if (property.managedReferenceValue != null)
            {
                var childRect = new Rect(position.x, line.yMax + EditorGUIUtility.standardVerticalSpacing, position.width, 0f);
                DrawChildren(childRect, property);
            }

            EditorGUI.EndProperty();
        }

        private static void DrawChildren(Rect rect, SerializedProperty root)
        {
            var copy = root.Copy();
            var end = copy.GetEndProperty();

            bool enterChildren = true;
            while (copy.NextVisible(enterChildren) && !SerializedProperty.EqualContents(copy, end))
            {
                float h = EditorGUI.GetPropertyHeight(copy, includeChildren: true);
                var line = new Rect(rect.x, rect.y, rect.width, h);
                EditorGUI.PropertyField(line, copy, includeChildren: true);
                rect.y += h + EditorGUIUtility.standardVerticalSpacing;
                enterChildren = false; // only enter children once; NextVisible will iterate subsequent props
            }
        }

        private static float GetChildrenHeight(SerializedProperty root)
        {
            float height = 0f;
            var copy = root.Copy();
            var end = copy.GetEndProperty();
            bool enterChildren = true;
            while (copy.NextVisible(enterChildren) && !SerializedProperty.EqualContents(copy, end))
            {
                height += EditorGUI.GetPropertyHeight(copy, includeChildren: true) + EditorGUIUtility.standardVerticalSpacing;
                enterChildren = false;
            }
            // subtract last spacing if any children were drawn
            if (height > 0f) height -= EditorGUIUtility.standardVerticalSpacing;
            return height;
        }

        private static Type GetCurrentReferenceType(SerializedProperty property)
        {
            var full = property.managedReferenceFullTypename; // format: "Assembly Name TypeFullName"
            if (string.IsNullOrEmpty(full)) return null;

            // managedReferenceFullTypename can be "AssemblyName TypeFullName" or "AssemblyName, Version=..., Culture=..., PublicKeyToken=...|TypeFullName"
            string[] parts = full.Split(' ');
            if (parts.Length == 2)
            {
                string asmName = parts[0];
                string typeName = parts[1];
                return Type.GetType($"{typeName}, {asmName}");
            }

            // Fallback robust parse (for older Unity variants):
            int pipeIdx = full.IndexOf('|');
            if (pipeIdx >= 0)
            {
                string asmQualified = full.Substring(0, pipeIdx);
                string typeName = full.Substring(pipeIdx + 1);
                return Type.GetType($"{typeName}, {asmQualified}");
            }

            return null;
        }

        private static Type[] GetAssignableTypes(Type baseType)
        {
            if (_typeCache.TryGetValue(baseType, out var cached))
                return cached;

            // Prefer fast UnityEditor.TypeCache lookup when possible
            var list = new List<Type>();
            try
            {
                foreach (var t in TypeCache.GetTypesDerivedFrom(baseType))
                {
                    if (IsConcreteSerializable(t)) list.Add(t);
                }
            }
            catch
            {
                // Fallback: manual scan
                foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    Type[] types;
                    try { types = asm.GetTypes(); }
                    catch (ReflectionTypeLoadException e) { types = e.Types.Where(x => x != null).ToArray(); }

                    foreach (var t in types)
                    {
                        if (t == null) continue;
                        if (baseType.IsAssignableFrom(t) && IsConcreteSerializable(t))
                            list.Add(t);
                    }
                }
            }

            // Sort by nice name for a clean dropdown
            var result = list.Distinct().OrderBy(GetNiceTypeName).ToArray();
            _typeCache[baseType] = result;
            return result;
        }

        private static bool IsConcreteSerializable(Type t)
        {
            return t.IsClass && !t.IsAbstract && t.GetConstructor(Type.EmptyTypes) != null &&
                (t.IsSerializable || t.GetCustomAttribute<SerializableAttribute>() != null);
        }

        private static string GetNiceTypeName(Type t)
        {
            // Display as "Namespace.TypeName" (without assembly)
            return string.IsNullOrEmpty(t.Namespace) ? t.Name : $"{t.Namespace}.{t.Name}";
        }
    }
}