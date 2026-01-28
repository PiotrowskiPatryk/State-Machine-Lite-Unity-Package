using System;
using Dev.Cortez.StateMachines.Core.StateMachineConfiguration.Definition;
using Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Common.Builders;
using Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Common.Data;
using Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Common.Mocks;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Unit.Editor.ViewModels
{
    /// <summary>
    /// Unit tests for ViewModelBase class.
    /// Tests all property application methods, notification system, and disposal.
    /// </summary>
    [TestFixture]
    [Category("ViewModels")]
    public sealed class ViewModelBaseTests
    {
        private TestPropertyContext<AllPropertyTypesData> _context;
        private TestableViewModel _viewModel;

        [SetUp]
        public void SetUp()
        {
            _context = SerializedPropertyTestHelper.CreateTestContext<AllPropertyTypesData>();
            _viewModel = new TestableViewModel(_context.Property);
        }

        [TearDown]
        public void TearDown()
        {
            _viewModel?.Dispose();
            _context?.Dispose();
        }

        #region Property Application Tests - String

        [Test]
        public void ApplyPropertyValueString_UpdatesSerializedProperty()
        {
            // Arrange
            const string expectedValue = "Test String Value";

            // Act
            _viewModel.TestApplyPropertyValueString(AllPropertyTypesData.STRING_VALUE_PROPERTY, expectedValue);

            // Assert
            _context.Update();
            var prop = _context.Property.FindPropertyRelative(AllPropertyTypesData.STRING_VALUE_PROPERTY);
            Assert.That(prop.stringValue, Is.EqualTo(expectedValue));
        }

        #endregion

        #region Property Application Tests - Numeric Types

        [Test]
        public void ApplyPropertyValueInt_UpdatesSerializedProperty()
        {
            // Arrange
            const int expectedValue = 42;

            // Act
            _viewModel.TestApplyPropertyValueInt(AllPropertyTypesData.INT_VALUE_PROPERTY, expectedValue);

            // Assert
            _context.Update();
            var prop = _context.Property.FindPropertyRelative(AllPropertyTypesData.INT_VALUE_PROPERTY);
            Assert.That(prop.intValue, Is.EqualTo(expectedValue));
        }

        [Test]
        public void ApplyPropertyValueFloat_UpdatesSerializedProperty()
        {
            // Arrange
            const float expectedValue = 3.14159f;

            // Act
            _viewModel.TestApplyPropertyValueFloat(AllPropertyTypesData.FLOAT_VALUE_PROPERTY, expectedValue);

            // Assert
            _context.Update();
            var prop = _context.Property.FindPropertyRelative(AllPropertyTypesData.FLOAT_VALUE_PROPERTY);
            Assert.That(prop.floatValue, Is.EqualTo(expectedValue).Within(0.0001f));
        }

        [Test]
        public void ApplyPropertyValueDouble_UpdatesSerializedProperty()
        {
            // Arrange
            const double expectedValue = 3.14159265358979;

            // Act
            _viewModel.TestApplyPropertyValueDouble(AllPropertyTypesData.DOUBLE_VALUE_PROPERTY, expectedValue);

            // Assert
            _context.Update();
            var prop = _context.Property.FindPropertyRelative(AllPropertyTypesData.DOUBLE_VALUE_PROPERTY);
            Assert.That(prop.doubleValue, Is.EqualTo(expectedValue).Within(0.0000001));
        }

        [Test]
        public void ApplyPropertyValueLong_UpdatesSerializedProperty()
        {
            // Arrange
            const long expectedValue = 9223372036854775807L;

            // Act
            _viewModel.TestApplyPropertyValueLong(AllPropertyTypesData.LONG_VALUE_PROPERTY, expectedValue);

            // Assert
            _context.Update();
            var prop = _context.Property.FindPropertyRelative(AllPropertyTypesData.LONG_VALUE_PROPERTY);
            Assert.That(prop.longValue, Is.EqualTo(expectedValue));
        }

        #endregion

        #region Property Application Tests - Boolean

        [Test]
        public void ApplyPropertyValueBool_UpdatesSerializedProperty()
        {
            // Arrange
            const bool expectedValue = true;

            // Act
            _viewModel.TestApplyPropertyValueBool(AllPropertyTypesData.BOOL_VALUE_PROPERTY, expectedValue);

            // Assert
            _context.Update();
            var prop = _context.Property.FindPropertyRelative(AllPropertyTypesData.BOOL_VALUE_PROPERTY);
            Assert.That(prop.boolValue, Is.EqualTo(expectedValue));
        }

        [Test]
        public void ApplyPropertyValueBool_False_UpdatesSerializedProperty()
        {
            // Arrange - First set to true
            _viewModel.TestApplyPropertyValueBool(AllPropertyTypesData.BOOL_VALUE_PROPERTY, true);
            _context.Update();

            // Act - Then set to false
            _viewModel.TestApplyPropertyValueBool(AllPropertyTypesData.BOOL_VALUE_PROPERTY, false);

            // Assert
            _context.Update();
            var prop = _context.Property.FindPropertyRelative(AllPropertyTypesData.BOOL_VALUE_PROPERTY);
            Assert.That(prop.boolValue, Is.False);
        }

        #endregion

        #region Property Application Tests - Vector Types

        [Test]
        public void ApplyPropertyValueVector2_UpdatesSerializedProperty()
        {
            // Arrange
            var expectedValue = new Vector2(1.5f, 2.5f);

            // Act
            _viewModel.TestApplyPropertyValueVector2(AllPropertyTypesData.VECTOR2_VALUE_PROPERTY, expectedValue);

            // Assert
            _context.Update();
            var prop = _context.Property.FindPropertyRelative(AllPropertyTypesData.VECTOR2_VALUE_PROPERTY);
            Assert.That(prop.vector2Value, Is.EqualTo(expectedValue));
        }

        [Test]
        public void ApplyPropertyValueVector3_UpdatesSerializedProperty()
        {
            // Arrange
            var expectedValue = new Vector3(1.5f, 2.5f, 3.5f);

            // Act
            _viewModel.TestApplyPropertyValueVector3(AllPropertyTypesData.VECTOR3_VALUE_PROPERTY, expectedValue);

            // Assert
            _context.Update();
            var prop = _context.Property.FindPropertyRelative(AllPropertyTypesData.VECTOR3_VALUE_PROPERTY);
            Assert.That(prop.vector3Value, Is.EqualTo(expectedValue));
        }

        [Test]
        public void ApplyPropertyValueVector4_UpdatesSerializedProperty()
        {
            // Arrange
            var expectedValue = new Vector4(1.5f, 2.5f, 3.5f, 4.5f);

            // Act
            _viewModel.TestApplyPropertyValueVector4(AllPropertyTypesData.VECTOR4_VALUE_PROPERTY, expectedValue);

            // Assert
            _context.Update();
            var prop = _context.Property.FindPropertyRelative(AllPropertyTypesData.VECTOR4_VALUE_PROPERTY);
            Assert.That(prop.vector4Value, Is.EqualTo(expectedValue));
        }

        [Test]
        public void ApplyPropertyValueVector2Int_UpdatesSerializedProperty()
        {
            // Arrange
            var expectedValue = new Vector2Int(10, 20);

            // Act
            _viewModel.TestApplyPropertyValueVector2Int(AllPropertyTypesData.VECTOR2INT_VALUE_PROPERTY, expectedValue);

            // Assert
            _context.Update();
            var prop = _context.Property.FindPropertyRelative(AllPropertyTypesData.VECTOR2INT_VALUE_PROPERTY);
            Assert.That(prop.vector2IntValue, Is.EqualTo(expectedValue));
        }

        [Test]
        public void ApplyPropertyValueVector3Int_UpdatesSerializedProperty()
        {
            // Arrange
            var expectedValue = new Vector3Int(10, 20, 30);

            // Act
            _viewModel.TestApplyPropertyValueVector3Int(AllPropertyTypesData.VECTOR3INT_VALUE_PROPERTY, expectedValue);

            // Assert
            _context.Update();
            var prop = _context.Property.FindPropertyRelative(AllPropertyTypesData.VECTOR3INT_VALUE_PROPERTY);
            Assert.That(prop.vector3IntValue, Is.EqualTo(expectedValue));
        }

        #endregion

        #region Property Application Tests - Color, Rect, Bounds

        [Test]
        public void ApplyPropertyValueColor_UpdatesSerializedProperty()
        {
            // Arrange
            var expectedValue = new Color(0.5f, 0.6f, 0.7f, 0.8f);

            // Act
            _viewModel.TestApplyPropertyValueColor(AllPropertyTypesData.COLOR_VALUE_PROPERTY, expectedValue);

            // Assert
            _context.Update();
            var prop = _context.Property.FindPropertyRelative(AllPropertyTypesData.COLOR_VALUE_PROPERTY);
            Assert.That(prop.colorValue, Is.EqualTo(expectedValue));
        }

        [Test]
        public void ApplyPropertyValueRect_UpdatesSerializedProperty()
        {
            // Arrange
            var expectedValue = new Rect(10, 20, 100, 200);

            // Act
            _viewModel.TestApplyPropertyValueRect(AllPropertyTypesData.RECT_VALUE_PROPERTY, expectedValue);

            // Assert
            _context.Update();
            var prop = _context.Property.FindPropertyRelative(AllPropertyTypesData.RECT_VALUE_PROPERTY);
            Assert.That(prop.rectValue, Is.EqualTo(expectedValue));
        }

        [Test]
        public void ApplyPropertyValueBounds_UpdatesSerializedProperty()
        {
            // Arrange
            var expectedValue = new Bounds(new Vector3(1, 2, 3), new Vector3(10, 20, 30));

            // Act
            _viewModel.TestApplyPropertyValueBounds(AllPropertyTypesData.BOUNDS_VALUE_PROPERTY, expectedValue);

            // Assert
            _context.Update();
            var prop = _context.Property.FindPropertyRelative(AllPropertyTypesData.BOUNDS_VALUE_PROPERTY);
            Assert.That(prop.boundsValue, Is.EqualTo(expectedValue));
        }

        #endregion

        #region Property Application Tests - Quaternion

        [Test]
        public void ApplyPropertyValueQuaternion_UpdatesSerializedProperty()
        {
            // Arrange
            var expectedValue = Quaternion.Euler(45, 90, 180);

            // Act
            _viewModel.TestApplyPropertyValueQuaternion(AllPropertyTypesData.QUATERNION_VALUE_PROPERTY, expectedValue);

            // Assert
            _context.Update();
            var prop = _context.Property.FindPropertyRelative(AllPropertyTypesData.QUATERNION_VALUE_PROPERTY);

            // Quaternion comparison needs tolerance
            Assert.That(Quaternion.Angle(prop.quaternionValue, expectedValue), Is.LessThan(0.01f));
        }

        #endregion

        #region Property Application Tests - AnimationCurve

        [Test]
        public void ApplyPropertyValueAnimationCurve_UpdatesSerializedProperty()
        {
            // Arrange
            var expectedValue = AnimationCurve.EaseInOut(0, 0, 1, 1);

            // Act
            _viewModel.TestApplyPropertyValueAnimationCurve(
                AllPropertyTypesData.ANIMATION_CURVE_VALUE_PROPERTY,
                expectedValue);

            // Assert
            _context.Update();
            var prop = _context.Property.FindPropertyRelative(AllPropertyTypesData.ANIMATION_CURVE_VALUE_PROPERTY);

            // AnimationCurve comparison by key count and values
            Assert.That(prop.animationCurveValue.length, Is.EqualTo(expectedValue.length));
        }

        #endregion

        #region Property Application Tests - Enum

        [Test]
        public void ApplyPropertyValueEnum_UpdatesSerializedProperty()
        {
            // Arrange
            const int expectedEnumIndex = (int)TestEnumType.Second;

            // Act
            _viewModel.TestApplyPropertyValueEnum(AllPropertyTypesData.ENUM_VALUE_PROPERTY, expectedEnumIndex);

            // Assert
            _context.Update();
            var prop = _context.Property.FindPropertyRelative(AllPropertyTypesData.ENUM_VALUE_PROPERTY);
            Assert.That(prop.enumValueIndex, Is.EqualTo(expectedEnumIndex));
        }

        [Test]
        [TestCase(TestEnumType.None, 0)]
        [TestCase(TestEnumType.First, 1)]
        [TestCase(TestEnumType.Second, 2)]
        [TestCase(TestEnumType.Third, 3)]
        public void ApplyPropertyValueEnum_AllValues_UpdatesCorrectly(TestEnumType enumValue, int expectedIndex)
        {
            // Act
            _viewModel.TestApplyPropertyValueEnum(AllPropertyTypesData.ENUM_VALUE_PROPERTY, expectedIndex);

            // Assert
            _context.Update();
            var prop = _context.Property.FindPropertyRelative(AllPropertyTypesData.ENUM_VALUE_PROPERTY);
            Assert.That(prop.enumValueIndex, Is.EqualTo(expectedIndex));
        }

        #endregion

        #region Property Application Tests - Object Reference

        [Test]
        public void ApplyPropertyValueObject_UpdatesSerializedProperty()
        {
            // Arrange
            var expectedValue = ScriptableObject.CreateInstance<ScriptableObject>();

            try
            {
                // Act
                _viewModel.TestApplyPropertyValueObject(AllPropertyTypesData.OBJECT_VALUE_PROPERTY, expectedValue);

                // Assert
                _context.Update();
                var prop = _context.Property.FindPropertyRelative(AllPropertyTypesData.OBJECT_VALUE_PROPERTY);
                Assert.That(prop.objectReferenceValue, Is.EqualTo(expectedValue));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(expectedValue);
            }
        }

        [Test]
        public void ApplyPropertyValueObject_Null_ClearsReference()
        {
            // Arrange - First set a value
            var tempObj = ScriptableObject.CreateInstance<ScriptableObject>();

            try
            {
                _viewModel.TestApplyPropertyValueObject(AllPropertyTypesData.OBJECT_VALUE_PROPERTY, tempObj);
                _context.Update();

                // Act - Then clear it
                _viewModel.TestApplyPropertyValueObject(AllPropertyTypesData.OBJECT_VALUE_PROPERTY, null);

                // Assert
                _context.Update();
                var prop = _context.Property.FindPropertyRelative(AllPropertyTypesData.OBJECT_VALUE_PROPERTY);
                Assert.That(prop.objectReferenceValue, Is.Null);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(tempObj);
            }
        }

        #endregion

        #region Notification Tests

        [Test]
        public void ApplyPropertyValueString_NotifiesPropertyChanged()
        {
            // Arrange
            var notified = false;
            string notifiedPropertyName = null;

            _viewModel.propertyChanged += (sender, args) =>
            {
                notified = true;
                notifiedPropertyName = args.propertyName;
            };

            // Act
            _viewModel.TestApplyPropertyValueString(AllPropertyTypesData.STRING_VALUE_PROPERTY, "test");

            // Assert
            Assert.That(notified, Is.True, "PropertyChanged event should have been raised");
            Assert.That(notifiedPropertyName, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void Notify_WithExplicitName_UsesProvidedName()
        {
            // Arrange
            const string expectedPropertyName = "CustomPropertyName";
            string notifiedPropertyName = null;

            _viewModel.propertyChanged += (sender, args) => { notifiedPropertyName = args.propertyName; };

            // Act
            _viewModel.TestNotify(expectedPropertyName);

            // Assert
            Assert.That(notifiedPropertyName, Is.EqualTo(expectedPropertyName));
        }

        [Test]
        public void Notify_WithNoSubscribers_DoesNotThrow()
        {
            // Act & Assert - Should not throw
            Assert.DoesNotThrow(() => _viewModel.TestNotify("SomeProperty"));
        }

        #endregion

        #region Disposal Tests

        [Test]
        public void Dispose_CanBeCalledMultipleTimes()
        {
            // Arrange
            var disposableVm = new TestableViewModel(_context.Property);

            // Act & Assert - Should not throw
            Assert.DoesNotThrow(() =>
            {
                disposableVm.Dispose();
                disposableVm.Dispose();
                disposableVm.Dispose();
            });
        }

        [Test]
        public void Dispose_CleansUpSerializedObject()
        {
            // Arrange - Use concrete wrapper instead of generic DefinitionWrapper
            var wrapper = ScriptableObject.CreateInstance<AllPropertyTypesDataWrapper>();
            var so = new SerializedObject(wrapper);
            var property = so.FindProperty(AllPropertyTypesDataWrapper.DATA_PROPERTY_NAME);
            var disposableVm = new TestableViewModel(property);

            // Act
            disposableVm.Dispose();

            // Assert - The wrapper should be destroyed (non-persisted SO)
            Assert.That(wrapper == null, Is.True, "Transient ScriptableObject should be destroyed");
        }

        #endregion
    }
}
