using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Utilities
{
    /// <summary>
    ///     Editor-only switcher for a single SerializedProperty that can be either a ManagedReference or ObjectReference.
    ///     Caches prior instances <b>per switcher instance</b> (i.e., per property) so user configuration survives
    ///     when switching back and forth between concrete types. Also supports clearing the property when no type is chosen.
    ///     Keep one instance of this class as a field in your ViewModel/Coordinator for the specific property.
    /// </summary>
    public sealed class SerializedInstanceSwitcher<TBase> where TBase : class
    {
        private readonly SerializedProperty _property;
        private readonly Func<string, Type> _resolveType;
        private readonly Dictionary<string, object> _cache = new(StringComparer.Ordinal);

        /// <param name="property">The SerializedProperty to switch (managed/object reference)</param>
        /// <param name="resolveType">Resolver mapping a key to a concrete Type (optional)</param>
        public SerializedInstanceSwitcher(SerializedProperty property, Func<string, Type> resolveType = null)
        {
            _property = property ?? throw new ArgumentNullException(nameof(property));
            _resolveType = resolveType ?? (_ => null);
        }

        /// <summary>
        ///     Switch using a resolver key (e.g., trigger type name). Falls back to <paramref name="fallbackType" /> if the key
        ///     cannot be resolved.
        ///     If key is null/empty and no fallback is provided, the property is cleared (set to null) and the cache is kept.
        /// </summary>
        public bool SwitchTo(string typeKey, Type fallbackType = null, bool createAssetIfSO = false,
            string assetFolder = "Assets")
        {
            if (string.IsNullOrWhiteSpace(typeKey) && fallbackType == null)
            {
                return ClearValue();
            }

            var desired = _resolveType(typeKey) ?? fallbackType;

            return desired == null ? ClearValue() : SwitchTo(desired, createAssetIfSO, assetFolder);
        }

        /// <summary>
        ///     Switch directly to a specific concrete type. If desiredType is null, the property is cleared (set to null) and the
        ///     cache is kept.
        /// </summary>
        public bool SwitchTo(Type desiredType, bool createAssetIfSO = false, string assetFolder = "Assets")
        {
            if (desiredType == null)
            {
                return ClearValue();
            }

            if (!typeof(TBase).IsAssignableFrom(desiredType))
            {
                Debug.LogWarning($"Type {desiredType.FullName} is not assignable to {typeof(TBase).FullName}.");

                return false;
            }

            var serializedObject = _property.serializedObject;

            if (serializedObject == null || serializedObject.targetObject == null)
            {
                return false;
            }

            serializedObject.Update();
            Undo.RecordObject(serializedObject.targetObject, $"Switch {typeof(TBase).Name} Type");

            var changed = false;
            var desiredKey = desiredType.AssemblyQualifiedName;

            switch (_property.propertyType)
            {
                case SerializedPropertyType.ManagedReference:
                {
                    var current = _property.managedReferenceValue;

                    if (current != null)
                    {
                        _cache[current.GetType().AssemblyQualifiedName] = current;
                    }

                    if (current == null || current.GetType() != desiredType)
                    {
                        if (_cache.TryGetValue(desiredKey, out var cached))
                        {
                            _property.managedReferenceValue = cached;
                            changed = true;
                        }
                        else
                        {
                            if (desiredType.IsAbstract || desiredType.IsGenericTypeDefinition)
                            {
                                Debug.LogError(
                                    $"Cannot instantiate abstract/open-generic type: {desiredType.FullName}");

                                break;
                            }

                            var ctor = desiredType.GetConstructor(Type.EmptyTypes);

                            if (ctor == null)
                            {
                                Debug.LogError(
                                    $"Type {desiredType.FullName} requires a public parameterless constructor.");

                                break;
                            }

                            _property.managedReferenceValue = Activator.CreateInstance(desiredType);
                            changed = true;
                        }
                    }

                    break;
                }

                case SerializedPropertyType.ObjectReference:
                {
                    if (!typeof(Object).IsAssignableFrom(desiredType))
                    {
                        Debug.LogWarning(
                            $"Property is ObjectReference but {desiredType.FullName} is not a UnityEngine.Object.");

                        break;
                    }

                    var currentObj = _property.objectReferenceValue;

                    if (currentObj != null)
                    {
                        _cache[currentObj.GetType().AssemblyQualifiedName] = currentObj;
                    }

                    if (currentObj == null || currentObj.GetType() != desiredType)
                    {
                        if (_cache.TryGetValue(desiredKey, out var cachedObj))
                        {
                            _property.objectReferenceValue = (Object)cachedObj; // restore prior
                            changed = true;
                        }
                        else
                        {
                            Object instance;

                            if (typeof(ScriptableObject).IsAssignableFrom(desiredType))
                            {
                                instance = ScriptableObject.CreateInstance(desiredType);

                                if (createAssetIfSO)
                                {
                                    var folder = string.IsNullOrWhiteSpace(assetFolder) ? "Assets" : assetFolder;
                                    var path = AssetDatabase.GenerateUniqueAssetPath(
                                        $"{folder}/{desiredType.Name}.asset");
                                    AssetDatabase.CreateAsset(instance, path);
                                    AssetDatabase.SaveAssets();
                                }
                            }
                            else
                            {
                                instance = (Object)Activator.CreateInstance(desiredType);
                            }

                            _property.objectReferenceValue = instance;
                            changed = true;
                        }
                    }

                    break;
                }

                default:
                    Debug.LogWarning(
                        $"Unsupported property type: {_property.propertyType}. Use [SerializeReference] or an Object reference.");

                    break;
            }

            if (changed)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(serializedObject.targetObject);
            }

            return changed;
        }

        /// <summary>
        ///     Clears the current value of the serialized property (sets to null). Optionally keeps or clears the per-type cache.
        ///     Returns true if a change was applied.
        /// </summary>
        public bool ClearValue(bool keepCache = true)
        {
            var so = _property.serializedObject;

            if (so == null || so.targetObject == null)
            {
                return false;
            }

            so.Update();
            Undo.RecordObject(so.targetObject, $"Clear {typeof(TBase).Name} Value");

            var changed = false;

            switch (_property.propertyType)
            {
                case SerializedPropertyType.ManagedReference:
                    if (_property.managedReferenceValue != null)
                    {
                        _property.managedReferenceValue = null;
                        changed = true;
                    }

                    break;

                case SerializedPropertyType.ObjectReference:
                    if (_property.objectReferenceValue != null)
                    {
                        _property.objectReferenceValue = null;
                        changed = true;
                    }

                    break;

                default:
                    Debug.LogWarning(
                        $"Unsupported property type: {_property.propertyType}. Use [SerializeReference] or an Object reference.");

                    break;
            }

            if (!keepCache)
            {
                _cache.Clear();
            }

            if (changed)
            {
                so.ApplyModifiedProperties();
                EditorUtility.SetDirty(so.targetObject);
            }

            return changed;
        }

        /// <summary>Clears the in-memory cache for this property (does not touch the current value).</summary>
        public void ClearCache()
        {
            _cache.Clear();
        }

        /// <summary>Removes a cached instance for a specific concrete type.</summary>
        public void RemoveFromCache(Type type)
        {
            if (type == null)
            {
                return;
            }

            _cache.Remove(type.AssemblyQualifiedName);
        }

        /// <summary>True if this switcher has a cached instance for the given type.</summary>
        public bool HasCached(Type type)
        {
            return type != null && _cache.ContainsKey(type.AssemblyQualifiedName);
        }
    }
}