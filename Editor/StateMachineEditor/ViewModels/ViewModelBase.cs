using System;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.ViewModels
{
    /// <summary>
    /// Base class for ViewModels that wrap SerializedProperty for UI binding.
    /// Provides generic property application methods and implements INotifyBindablePropertyChanged.
    /// </summary>
    public abstract class ViewModelBase : INotifyBindablePropertyChanged, IDisposable
    {
        public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;
        private bool _disposed;
        public abstract SerializedProperty SerializedProperty { get; }

        /// <summary>
        /// Disposes the ViewModel and cleans up any ScriptableObject backing the SerializedProperty.
        /// Call this method when the ViewModel is no longer needed to prevent memory leaks.
        /// Only destroys ScriptableObjects that were created in memory (not persisted to assets).
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing">True if disposing managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            if (disposing && SerializedProperty?.serializedObject?.targetObject != null)
            {
                var targetObject = SerializedProperty.serializedObject.targetObject;
                SerializedProperty.serializedObject.Dispose();

                // Only destroy ScriptableObjects that are not persisted to an asset
                if (targetObject is ScriptableObject && string.IsNullOrEmpty(AssetDatabase.GetAssetPath(targetObject)))
                {
                    Object.DestroyImmediate(targetObject);
                }
            }
        }

        /// <summary>
        /// Notifies listeners that a property has changed.
        /// </summary>
        /// <param name="property">The name of the property that changed.</param>
        protected void Notify([CallerMemberName] string property = "")
        {
            propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(property));
        }

        #region Generic Property Application

        /// <summary>
        /// Applies a value to a SerializedProperty at the given relative path.
        /// Automatically handles Update/ApplyModifiedProperties cycle and property change notification.
        /// </summary>
        /// <typeparam name="T">The type of value to apply.</typeparam>
        /// <param name="relativePropertyPath">Path to the property relative to SerializedProperty.</param>
        /// <param name="value">The value to apply.</param>
        /// <param name="property">The property name for notification (auto-filled by CallerMemberName).</param>
        protected void ApplyPropertyValue<T>(string relativePropertyPath, T value,
            [CallerMemberName] string property = "")
        {
            var prop = SerializedProperty.FindPropertyRelative(relativePropertyPath);

            if (prop == null)
            {
                Debug.LogError($"[ViewModelBase] Property not found: {relativePropertyPath}");

                return;
            }

            prop.serializedObject.Update();
            SetSerializedPropertyValue(prop, value);
            prop.serializedObject.ApplyModifiedProperties();

            Notify(property);
        }

        /// <summary>
        /// Sets the value of a SerializedProperty based on the type of value provided.
        /// Supports all Unity serializable types.
        /// </summary>
        /// <typeparam name="T">The type of value to set.</typeparam>
        /// <param name="prop">The SerializedProperty to modify.</param>
        /// <param name="value">The value to set.</param>
        private static void SetSerializedPropertyValue<T>(SerializedProperty prop, T value)
        {
            switch (value)
            {
                case string s:
                    prop.stringValue = s;

                    break;
                case int i:
                    prop.intValue = i;

                    break;
                case bool b:
                    prop.boolValue = b;

                    break;
                case float f:
                    prop.floatValue = f;

                    break;
                case double d:
                    prop.doubleValue = d;

                    break;
                case long l:
                    prop.longValue = l;

                    break;
                case Vector2 v2:
                    prop.vector2Value = v2;

                    break;
                case Vector3 v3:
                    prop.vector3Value = v3;

                    break;
                case Vector4 v4:
                    prop.vector4Value = v4;

                    break;
                case Vector2Int v2i:
                    prop.vector2IntValue = v2i;

                    break;
                case Vector3Int v3i:
                    prop.vector3IntValue = v3i;

                    break;
                case Color c:
                    prop.colorValue = c;

                    break;
                case Rect r:
                    prop.rectValue = r;

                    break;
                case Bounds bounds:
                    prop.boundsValue = bounds;

                    break;
                case Quaternion q:
                    prop.quaternionValue = q;

                    break;
                case AnimationCurve ac:
                    prop.animationCurveValue = ac;

                    break;
                case Object o:
                    prop.objectReferenceValue = o;

                    break;
                case null when typeof(T) == typeof(Object) || typeof(Object).IsAssignableFrom(typeof(T)):
                    prop.objectReferenceValue = null;

                    break;
                case Enum e:
                    prop.enumValueIndex = Convert.ToInt32(e);

                    break;
                default:
                    Debug.LogError($"[ViewModelBase] Unsupported property type: {typeof(T).Name}");

                    break;
            }
        }

        #endregion

        #region Type-Specific Methods (Backward Compatibility)

        // These methods provide backward compatibility and explicit type handling.
        // They delegate to the generic ApplyPropertyValue method.

        protected void ApplyPropertyValueString(string relativePropertyPath, string value,
            [CallerMemberName] string property = "")
        {
            ApplyPropertyValue(relativePropertyPath, value, property);
        }

        protected void ApplyPropertyValueInt(string relativePropertyPath, int value,
            [CallerMemberName] string property = "")
        {
            ApplyPropertyValue(relativePropertyPath, value, property);
        }

        protected void ApplyPropertyValueBool(string relativePropertyPath, bool value,
            [CallerMemberName] string property = "")
        {
            ApplyPropertyValue(relativePropertyPath, value, property);
        }

        protected void ApplyPropertyValueFloat(string relativePropertyPath, float value,
            [CallerMemberName] string property = "")
        {
            ApplyPropertyValue(relativePropertyPath, value, property);
        }

        protected void ApplyPropertyValueDouble(string relativePropertyPath, double value,
            [CallerMemberName] string property = "")
        {
            ApplyPropertyValue(relativePropertyPath, value, property);
        }

        protected void ApplyPropertyValueLong(string relativePropertyPath, long value,
            [CallerMemberName] string property = "")
        {
            ApplyPropertyValue(relativePropertyPath, value, property);
        }

        protected void ApplyPropertyValueVector2(string relativePropertyPath, Vector2 value,
            [CallerMemberName] string property = "")
        {
            ApplyPropertyValue(relativePropertyPath, value, property);
        }

        protected void ApplyPropertyValueVector3(string relativePropertyPath, Vector3 value,
            [CallerMemberName] string property = "")
        {
            ApplyPropertyValue(relativePropertyPath, value, property);
        }

        protected void ApplyPropertyValueVector4(string relativePropertyPath, Vector4 value,
            [CallerMemberName] string property = "")
        {
            ApplyPropertyValue(relativePropertyPath, value, property);
        }

        protected void ApplyPropertyValueVector2Int(string relativePropertyPath, Vector2Int value,
            [CallerMemberName] string property = "")
        {
            ApplyPropertyValue(relativePropertyPath, value, property);
        }

        protected void ApplyPropertyValueVector3Int(string relativePropertyPath, Vector3Int value,
            [CallerMemberName] string property = "")
        {
            ApplyPropertyValue(relativePropertyPath, value, property);
        }

        protected void ApplyPropertyValueColor(string relativePropertyPath, Color value,
            [CallerMemberName] string property = "")
        {
            ApplyPropertyValue(relativePropertyPath, value, property);
        }

        protected void ApplyPropertyValueRect(string relativePropertyPath, Rect value,
            [CallerMemberName] string property = "")
        {
            ApplyPropertyValue(relativePropertyPath, value, property);
        }

        protected void ApplyPropertyValueBounds(string relativePropertyPath, Bounds value,
            [CallerMemberName] string property = "")
        {
            ApplyPropertyValue(relativePropertyPath, value, property);
        }

        protected void ApplyPropertyValueQuaternion(string relativePropertyPath, Quaternion value,
            [CallerMemberName] string property = "")
        {
            ApplyPropertyValue(relativePropertyPath, value, property);
        }

        protected void ApplyPropertyValueAnimationCurve(string relativePropertyPath, AnimationCurve value,
            [CallerMemberName] string property = "")
        {
            ApplyPropertyValue(relativePropertyPath, value, property);
        }

        protected void ApplyPropertyValueEnum(string relativePropertyPath, int value,
            [CallerMemberName] string property = "")
        {
            var prop = SerializedProperty.FindPropertyRelative(relativePropertyPath);

            if (prop == null)
            {
                Debug.LogError($"[ViewModelBase] Property not found: {relativePropertyPath}");

                return;
            }

            prop.serializedObject.Update();
            prop.enumValueIndex = value;
            prop.serializedObject.ApplyModifiedProperties();

            Notify(property);
        }

        protected void ApplyPropertyValueObject(string relativePropertyPath, Object value,
            [CallerMemberName] string property = "")
        {
            ApplyPropertyValue(relativePropertyPath, value, property);
        }

        #endregion
    }
}