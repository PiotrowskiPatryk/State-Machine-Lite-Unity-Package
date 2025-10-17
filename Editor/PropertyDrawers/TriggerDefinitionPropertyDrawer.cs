using System;
using System.Linq;
using System.Collections.Generic;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Data;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.UIToolkitUtilities;
using Dev.Cortez.StateMachines.Core;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Data;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Data.Definition;

namespace Dev.Cortez.StateMachines.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(TriggerDefinition))]
    public sealed class TriggerDefinitionPropertyDrawer : PropertyDrawer
    {
        [SerializeField]
        private VisualTreeAsset _visualTreeAsset;

        // In-memory cache to preserve payloads per type while the user experiments with different types in the editor.
        // Keyed by: target instance id + property path + type assembly-qualified name
        private static readonly Dictionary<string, IPayload> s_PayloadCache = new Dictionary<string, IPayload>();

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = _visualTreeAsset.CloneTree();

            var typeContainer = root.Q<VisualElement>("TypeContainer") ?? root;
            var payloadContainer = root.Q<VisualElement>("PayloadContainer") ?? root;

            var typeProp = property.FindPropertyRelative(TriggerDefinitionPropertyNames.TypeName);
            var payloadProp = property.FindPropertyRelative(TriggerDefinitionPropertyNames.Payload);
            var currentType = TypePickerUtility.TryGetTypeFromProperty(typeProp);

            // Seed cache with existing payload for the current type (so it can be restored if the user switches away and back)
            TryCacheCurrentPayload(property, typeProp, payloadProp);

            // Create a dropdown for types implementing ITrigger
            var dropdown = TypePickerUtility.CreateForBase<ITrigger>(currentType, selected =>
            {
                var so = property.serializedObject;
                so.Update();

                // Cache current payload under the previous type before changing
                var previousTypeName = typeProp.stringValue;
                CachePayload(property, previousTypeName, payloadProp);

                // Apply new type selection
                TypePickerUtility.TrySetTypeOnProperty(typeProp, selected);

                // Update payload UI when type changes (will attempt to restore cached payload for selected)
                RefreshPayloadUI(property, payloadProp, payloadContainer, selected);
                so.ApplyModifiedProperties();
            }, label: "Type:");

            dropdown.style.flexGrow = 0f;
            typeContainer.Add(dropdown);

            // Initial payload UI
            RefreshPayloadUI(property, payloadProp, payloadContainer, currentType);

            return root;
        }

        private static void RefreshPayloadUI(SerializedProperty rootProperty, SerializedProperty payloadProp, VisualElement container, Type triggerType)
        {
            if (container == null || payloadProp == null)
                return;

            // Clear previous UI
            container.Clear();

            // Determine payload type from ITrigger<TPayload>
            var payloadType = GetPayloadType(triggerType);
            if (payloadType == null)
            {
                // If no payload required, clear the managed reference but keep cached values for other types
                try
                {
                    payloadProp.managedReferenceValue = null;
                    rootProperty.serializedObject.ApplyModifiedProperties();
                }
                catch { /* ignore in case of older Unity versions */ }
                return;
            }

            // Try restore a cached payload for this selected type
            var cached = TryGetCachedPayload(rootProperty, triggerType);

            // Ensure managed reference has the correct concrete type
            var currentObj = payloadProp.managedReferenceValue;
            if (cached != null && payloadType.IsInstanceOfType(cached))
            {
                try
                {
                    payloadProp.managedReferenceValue = cached;
                    rootProperty.serializedObject.ApplyModifiedProperties();
                    currentObj = cached;
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Failed to assign cached payload for type {payloadType}: {e}");
                }
            }

            if (currentObj == null || currentObj.GetType() != payloadType)
            {
                try
                {
                    var instance = Activator.CreateInstance(payloadType);
                    payloadProp.managedReferenceValue = instance;
                    rootProperty.serializedObject.ApplyModifiedProperties();
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to create payload instance of type {payloadType}: {e}");
                }
            }

            // Add a PropertyField to edit the payload
            var field = new PropertyField(payloadProp, "Payload");
            field.style.marginTop = 4;
            field.Bind(rootProperty.serializedObject);
            container.Add(field);
        }

        private static string BuildCacheKey(SerializedProperty rootProperty, string typeName)
        {
            var target = rootProperty?.serializedObject?.targetObject;
            var id = target != null ? target.GetInstanceID().ToString() : "0";
            var path = rootProperty != null ? rootProperty.propertyPath : string.Empty;
            var typeKey = string.IsNullOrEmpty(typeName) ? "(none)" : typeName;
            return id + "|" + path + "|" + typeKey;
        }

        private static void CachePayload(SerializedProperty rootProperty, string typeName, SerializedProperty payloadProp)
        {
            if (payloadProp == null) return;
            try
            {
                var value = payloadProp.managedReferenceValue as IPayload;
                if (value != null)
                {
                    var key = BuildCacheKey(rootProperty, typeName);
                    s_PayloadCache[key] = value;
                }
            }
            catch { /* ignore */ }
        }

        private static void TryCacheCurrentPayload(SerializedProperty rootProperty, SerializedProperty typeProp, SerializedProperty payloadProp)
        {
            if (typeProp == null) return;
            CachePayload(rootProperty, typeProp.stringValue, payloadProp);
        }

        private static IPayload TryGetCachedPayload(SerializedProperty rootProperty, Type triggerType)
        {
            if (triggerType == null) return null;
            var typeName = triggerType.AssemblyQualifiedName;
            var key = BuildCacheKey(rootProperty, typeName);
            if (key != null && s_PayloadCache.TryGetValue(key, out var payload))
                return payload;
            return null;
        }

        private static Type GetPayloadType(Type triggerType)
        {
            if (triggerType == null)
                return null;

            if (triggerType.IsGenericType && triggerType.GetGenericTypeDefinition() == typeof(ITrigger<>))
            {
                var arg = triggerType.GetGenericArguments().FirstOrDefault();
                return typeof(IPayload).IsAssignableFrom(arg) ? arg : null;
            }

            var iface = triggerType.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ITrigger<>));
            if (iface == null)
                return null;

            var payloadArg = iface.GetGenericArguments().FirstOrDefault();
            return typeof(IPayload).IsAssignableFrom(payloadArg) ? payloadArg : null;
        }
    }
}
