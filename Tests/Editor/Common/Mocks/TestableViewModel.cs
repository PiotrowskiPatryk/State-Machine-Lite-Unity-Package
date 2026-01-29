using Dev.Cortez.StateMachines.Editor.StateMachineEditor.ViewModels;
using UnityEditor;
using UnityEngine;

namespace Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Common.Mocks
{
    /// <summary>
    /// Concrete implementation of ViewModelBase for testing purposes.
    /// Exposes all protected ApplyPropertyValue methods as public for testing.
    /// </summary>
    public sealed class TestableViewModel : ViewModelBase
    {
        public override SerializedProperty SerializedProperty { get; }

        public TestableViewModel(SerializedProperty serializedProperty)
        {
            SerializedProperty = serializedProperty;
        }

        // Expose protected methods for testing

        public void TestApplyPropertyValueString(string relativePropertyPath, string value)
        {
            ApplyPropertyValueString(relativePropertyPath, value);
        }

        public void TestApplyPropertyValueInt(string relativePropertyPath, int value)
        {
            ApplyPropertyValueInt(relativePropertyPath, value);
        }

        public void TestApplyPropertyValueBool(string relativePropertyPath, bool value)
        {
            ApplyPropertyValueBool(relativePropertyPath, value);
        }

        public void TestApplyPropertyValueFloat(string relativePropertyPath, float value)
        {
            ApplyPropertyValueFloat(relativePropertyPath, value);
        }

        public void TestApplyPropertyValueDouble(string relativePropertyPath, double value)
        {
            ApplyPropertyValueDouble(relativePropertyPath, value);
        }

        public void TestApplyPropertyValueLong(string relativePropertyPath, long value)
        {
            ApplyPropertyValueLong(relativePropertyPath, value);
        }

        public void TestApplyPropertyValueVector2(string relativePropertyPath, Vector2 value)
        {
            ApplyPropertyValueVector2(relativePropertyPath, value);
        }

        public void TestApplyPropertyValueVector3(string relativePropertyPath, Vector3 value)
        {
            ApplyPropertyValueVector3(relativePropertyPath, value);
        }

        public void TestApplyPropertyValueVector4(string relativePropertyPath, Vector4 value)
        {
            ApplyPropertyValueVector4(relativePropertyPath, value);
        }

        public void TestApplyPropertyValueVector2Int(string relativePropertyPath, Vector2Int value)
        {
            ApplyPropertyValueVector2Int(relativePropertyPath, value);
        }

        public void TestApplyPropertyValueVector3Int(string relativePropertyPath, Vector3Int value)
        {
            ApplyPropertyValueVector3Int(relativePropertyPath, value);
        }

        public void TestApplyPropertyValueColor(string relativePropertyPath, Color value)
        {
            ApplyPropertyValueColor(relativePropertyPath, value);
        }

        public void TestApplyPropertyValueRect(string relativePropertyPath, Rect value)
        {
            ApplyPropertyValueRect(relativePropertyPath, value);
        }

        public void TestApplyPropertyValueBounds(string relativePropertyPath, Bounds value)
        {
            ApplyPropertyValueBounds(relativePropertyPath, value);
        }

        public void TestApplyPropertyValueQuaternion(string relativePropertyPath, Quaternion value)
        {
            ApplyPropertyValueQuaternion(relativePropertyPath, value);
        }

        public void TestApplyPropertyValueAnimationCurve(string relativePropertyPath, AnimationCurve value)
        {
            ApplyPropertyValueAnimationCurve(relativePropertyPath, value);
        }

        public void TestApplyPropertyValueEnum(string relativePropertyPath, int value)
        {
            ApplyPropertyValueEnum(relativePropertyPath, value);
        }

        public void TestApplyPropertyValueObject(string relativePropertyPath, Object value)
        {
            ApplyPropertyValueObject(relativePropertyPath, value);
        }

        public void TestApplyPropertyValue<T>(string relativePropertyPath, T value)
        {
            ApplyPropertyValue(relativePropertyPath, value);
        }

        public void TestNotify(string propertyName)
        {
            Notify(propertyName);
        }
    }
}