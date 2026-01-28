using System;
using Dev.Cortez.StateMachines.Core.Interfaces;
using UnityEngine;

namespace Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Common.Data
{
    /// <summary>
    /// Test data class containing all property types supported by ViewModelBase.
    /// Used for comprehensive testing of ApplyPropertyValue methods.
    /// </summary>
    [Serializable]
    public sealed class AllPropertyTypesData
    {
        public string StringValue;
        public int IntValue;
        public bool BoolValue;
        public float FloatValue;
        public double DoubleValue;
        public long LongValue;
        public Vector2 Vector2Value;
        public Vector3 Vector3Value;
        public Vector4 Vector4Value;
        public Vector2Int Vector2IntValue;
        public Vector3Int Vector3IntValue;
        public Color ColorValue;
        public Rect RectValue;
        public Bounds BoundsValue;
        public Quaternion QuaternionValue;
        public AnimationCurve AnimationCurveValue;
        public TestEnumType EnumValue;
        public UnityEngine.Object ObjectValue;

        // Property name constants for testing
        public const string STRING_VALUE_PROPERTY = nameof(StringValue);
        public const string INT_VALUE_PROPERTY = nameof(IntValue);
        public const string BOOL_VALUE_PROPERTY = nameof(BoolValue);
        public const string FLOAT_VALUE_PROPERTY = nameof(FloatValue);
        public const string DOUBLE_VALUE_PROPERTY = nameof(DoubleValue);
        public const string LONG_VALUE_PROPERTY = nameof(LongValue);
        public const string VECTOR2_VALUE_PROPERTY = nameof(Vector2Value);
        public const string VECTOR3_VALUE_PROPERTY = nameof(Vector3Value);
        public const string VECTOR4_VALUE_PROPERTY = nameof(Vector4Value);
        public const string VECTOR2INT_VALUE_PROPERTY = nameof(Vector2IntValue);
        public const string VECTOR3INT_VALUE_PROPERTY = nameof(Vector3IntValue);
        public const string COLOR_VALUE_PROPERTY = nameof(ColorValue);
        public const string RECT_VALUE_PROPERTY = nameof(RectValue);
        public const string BOUNDS_VALUE_PROPERTY = nameof(BoundsValue);
        public const string QUATERNION_VALUE_PROPERTY = nameof(QuaternionValue);
        public const string ANIMATION_CURVE_VALUE_PROPERTY = nameof(AnimationCurveValue);
        public const string ENUM_VALUE_PROPERTY = nameof(EnumValue);
        public const string OBJECT_VALUE_PROPERTY = nameof(ObjectValue);
    }

    /// <summary>
    /// Test enum for enum property testing.
    /// </summary>
    public enum TestEnumType
    {
        None = 0,
        First = 1,
        Second = 2,
        Third = 3
    }
}
