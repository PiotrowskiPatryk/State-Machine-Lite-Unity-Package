using System;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.ViewModels
{
    public abstract class ViewModelBase : INotifyBindablePropertyChanged, IDisposable
    {
        private bool _disposed;

        public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;
        public abstract SerializedProperty SerializedProperty { get; }

        /// <summary>
        /// Disposes the ViewModel and cleans up any ScriptableObject backing the SerializedProperty.
        /// Call this method when the ViewModel is no longer needed to prevent memory leaks.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            if (SerializedProperty?.serializedObject?.targetObject != null)
            {
                var targetObject = SerializedProperty.serializedObject.targetObject;
                SerializedProperty.serializedObject.Dispose();

                if (targetObject is ScriptableObject)
                {
                    Object.DestroyImmediate(targetObject);
                }
            }
        }

        protected void ApplyPropertyValueString(string relativePropertyPath, string value,
            [CallerMemberName] string property = "")
        {
            var prop = SerializedProperty.FindPropertyRelative(relativePropertyPath);
            prop.serializedObject.Update();
            prop.stringValue = value;
            prop.serializedObject.ApplyModifiedProperties();

            Notify(property);
        }

        protected void ApplyPropertyValueVector2(string relativePropertyPath, Vector2 value,
            [CallerMemberName] string property = "")
        {
            var prop = SerializedProperty.FindPropertyRelative(relativePropertyPath);
            prop.serializedObject.Update();
            prop.vector2Value = value;
            prop.serializedObject.ApplyModifiedProperties();

            Notify(property);
        }

        protected void ApplyPropertyValueBool(string relativePropertyPath, bool value,
            [CallerMemberName] string property = "")
        {
            var prop = SerializedProperty.FindPropertyRelative(relativePropertyPath);
            prop.serializedObject.Update();
            prop.boolValue = value;
            prop.serializedObject.ApplyModifiedProperties();

            Notify(property);
        }

        protected void ApplyPropertyValueFloat(string relativePropertyPath, float value,
            [CallerMemberName] string property = "")
        {
            var prop = SerializedProperty.FindPropertyRelative(relativePropertyPath);
            prop.serializedObject.Update();
            prop.floatValue = value;
            prop.serializedObject.ApplyModifiedProperties();

            Notify(property);
        }

        protected void ApplyPropertyValueInt(string relativePropertyPath, int value,
            [CallerMemberName] string property = "")
        {
            var prop = SerializedProperty.FindPropertyRelative(relativePropertyPath);
            prop.serializedObject.Update();
            prop.intValue = value;
            prop.serializedObject.ApplyModifiedProperties();

            Notify(property);
        }

        protected void ApplyPropertyValueEnum(string relativePropertyPath, int value,
            [CallerMemberName] string property = "")
        {
            var prop = SerializedProperty.FindPropertyRelative(relativePropertyPath);
            prop.serializedObject.Update();
            prop.enumValueIndex = value;
            prop.serializedObject.ApplyModifiedProperties();

            Notify(property);
        }

        protected void ApplyPropertyValueObject(string relativePropertyPath, Object value,
            [CallerMemberName] string property = "")
        {
            var prop = SerializedProperty.FindPropertyRelative(relativePropertyPath);
            prop.serializedObject.Update();
            prop.objectReferenceValue = value;
            prop.serializedObject.ApplyModifiedProperties();

            Notify(property);
        }

        protected void ApplyPropertyValueVector3(string relativePropertyPath, Vector3 value,
            [CallerMemberName] string property = "")
        {
            var prop = SerializedProperty.FindPropertyRelative(relativePropertyPath);
            prop.serializedObject.Update();
            prop.vector3Value = value;
            prop.serializedObject.ApplyModifiedProperties();

            Notify(property);
        }

        protected void ApplyPropertyValueVector4(string relativePropertyPath, Vector4 value,
            [CallerMemberName] string property = "")
        {
            var prop = SerializedProperty.FindPropertyRelative(relativePropertyPath);
            prop.serializedObject.Update();
            prop.vector4Value = value;
            prop.serializedObject.ApplyModifiedProperties();

            Notify(property);
        }

        protected void ApplyPropertyValueVector2Int(string relativePropertyPath, Vector2Int value,
            [CallerMemberName] string property = "")
        {
            var prop = SerializedProperty.FindPropertyRelative(relativePropertyPath);
            prop.serializedObject.Update();
            prop.vector2IntValue = value;
            prop.serializedObject.ApplyModifiedProperties();

            Notify(property);
        }

        protected void ApplyPropertyValueVector3Int(string relativePropertyPath, Vector3Int value,
            [CallerMemberName] string property = "")
        {
            var prop = SerializedProperty.FindPropertyRelative(relativePropertyPath);
            prop.serializedObject.Update();
            prop.vector3IntValue = value;
            prop.serializedObject.ApplyModifiedProperties();

            Notify(property);
        }

        protected void ApplyPropertyValueColor(string relativePropertyPath, Color value,
            [CallerMemberName] string property = "")
        {
            var prop = SerializedProperty.FindPropertyRelative(relativePropertyPath);
            prop.serializedObject.Update();
            prop.colorValue = value;
            prop.serializedObject.ApplyModifiedProperties();

            Notify(property);
        }

        protected void ApplyPropertyValueRect(string relativePropertyPath, Rect value,
            [CallerMemberName] string property = "")
        {
            var prop = SerializedProperty.FindPropertyRelative(relativePropertyPath);
            prop.serializedObject.Update();
            prop.rectValue = value;
            prop.serializedObject.ApplyModifiedProperties();

            Notify(property);
        }

        protected void ApplyPropertyValueBounds(string relativePropertyPath, Bounds value,
            [CallerMemberName] string property = "")
        {
            var prop = SerializedProperty.FindPropertyRelative(relativePropertyPath);
            prop.serializedObject.Update();
            prop.boundsValue = value;
            prop.serializedObject.ApplyModifiedProperties();

            Notify(property);
        }

        protected void ApplyPropertyValueQuaternion(string relativePropertyPath, Quaternion value,
            [CallerMemberName] string property = "")
        {
            var prop = SerializedProperty.FindPropertyRelative(relativePropertyPath);
            prop.serializedObject.Update();
            prop.quaternionValue = value;
            prop.serializedObject.ApplyModifiedProperties();

            Notify(property);
        }

        protected void ApplyPropertyValueLong(string relativePropertyPath, long value,
            [CallerMemberName] string property = "")
        {
            var prop = SerializedProperty.FindPropertyRelative(relativePropertyPath);
            prop.serializedObject.Update();
            prop.longValue = value;
            prop.serializedObject.ApplyModifiedProperties();

            Notify(property);
        }

        protected void ApplyPropertyValueDouble(string relativePropertyPath, double value,
            [CallerMemberName] string property = "")
        {
            var prop = SerializedProperty.FindPropertyRelative(relativePropertyPath);
            prop.serializedObject.Update();
            prop.doubleValue = value;
            prop.serializedObject.ApplyModifiedProperties();

            Notify(property);
        }

        protected void ApplyPropertyValueAnimationCurve(string relativePropertyPath, AnimationCurve value,
            [CallerMemberName] string property = "")
        {
            var prop = SerializedProperty.FindPropertyRelative(relativePropertyPath);
            prop.serializedObject.Update();
            prop.animationCurveValue = value;
            prop.serializedObject.ApplyModifiedProperties();

            Notify(property);
        }

        protected void Notify([CallerMemberName] string property = "")
        {
            propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(property));
        }
    }
}