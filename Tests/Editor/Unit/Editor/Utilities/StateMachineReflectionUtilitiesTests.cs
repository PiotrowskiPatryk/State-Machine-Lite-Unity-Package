using System.Text.RegularExpressions;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Utilities;
using Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Common.Mocks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Unit.Editor.Utilities
{
    /// <summary>
    /// Unit tests for StateMachineReflectionUtilities.
    /// Tests type resolution and name formatting methods.
    /// </summary>
    [TestFixture, Category("Utilities")]
    public sealed class StateMachineReflectionUtilitiesTests
    {
        #region GetTriggerPayloadType Tests

        [Test]
        public void GetTriggerPayloadType_WithValidType_ReturnsPayloadType()
        {
            // Arrange
            var typeName = typeof(MockTrigger).AssemblyQualifiedName;

            // Act
            var result = StateMachineReflectionUtilities.GetTriggerPayloadType(typeName);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void GetTriggerPayloadType_WithInvalidType_LogsError()
        {
            // Arrange
            const string invalidTypeName = "NonExistent.Type, NonExistent.Assembly";
            LogAssert.Expect(LogType.Error, new Regex("Failed to find payload type"));

            // Act
            var result = StateMachineReflectionUtilities.GetTriggerPayloadType(invalidTypeName);

            // Assert
            Assert.That(result, Is.Null);
        }

        #endregion

        #region GetConditionPayloadType Tests

        [Test]
        public void GetConditionPayloadType_WithValidType_ReturnsPayloadType()
        {
            // Arrange
            var typeName = typeof(MockCondition).AssemblyQualifiedName;

            // Act
            var result = StateMachineReflectionUtilities.GetConditionPayloadType(typeName);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void GetConditionPayloadType_WithInvalidType_LogsError()
        {
            // Arrange
            const string invalidTypeName = "NonExistent.Type, NonExistent.Assembly";
            LogAssert.Expect(LogType.Error, new Regex("Failed to find payload type"));

            // Act
            var result = StateMachineReflectionUtilities.GetConditionPayloadType(invalidTypeName);

            // Assert
            Assert.That(result, Is.Null);
        }

        #endregion

        #region GetStateMachinePayloadType Tests

        [Test]
        public void GetStateMachinePayloadType_WithValidType_ReturnsPayloadType()
        {
            // Arrange
            var typeName = typeof(MockStateMachine).AssemblyQualifiedName;

            // Act
            var result = StateMachineReflectionUtilities.GetStateMachinePayloadType(typeName);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void GetStateMachinePayloadType_WithInvalidType_LogsError()
        {
            // Arrange
            const string invalidTypeName = "NonExistent.Type, NonExistent.Assembly";
            LogAssert.Expect(LogType.Error, new Regex("Failed to find state machine payload type"));

            // Act
            var result = StateMachineReflectionUtilities.GetStateMachinePayloadType(invalidTypeName);

            // Assert
            Assert.That(result, Is.Null);
        }

        #endregion

        #region GetStatePayloadType Tests

        [Test]
        public void GetStatePayloadType_WithValidType_ReturnsPayloadType()
        {
            // Arrange
            var typeName = typeof(MockState).AssemblyQualifiedName;

            // Act
            var result = StateMachineReflectionUtilities.GetStatePayloadType(typeName);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void GetStatePayloadType_WithInvalidType_LogsError()
        {
            // Arrange
            const string invalidTypeName = "NonExistent.Type, NonExistent.Assembly";
            LogAssert.Expect(LogType.Error, new Regex("Failed to find payload type"));

            // Act
            var result = StateMachineReflectionUtilities.GetStatePayloadType(invalidTypeName);

            // Assert
            Assert.That(result, Is.Null);
        }

        #endregion

        #region GetBaseStateFromStateMachine Tests

        [Test]
        public void GetBaseStateFromStateMachine_WithValidType_ReturnsBaseType()
        {
            // Arrange
            var typeName = typeof(MockStateMachine).AssemblyQualifiedName;

            // Act
            var result = StateMachineReflectionUtilities.GetBaseStateFromStateMachine(typeName);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void GetBaseStateFromStateMachine_WithInvalidType_LogsError()
        {
            // Arrange
            const string invalidTypeName = "NonExistent.Type, NonExistent.Assembly";
            LogAssert.Expect(LogType.Error, new Regex("Failed to find state type"));

            // Act
            var result = StateMachineReflectionUtilities.GetBaseStateFromStateMachine(invalidTypeName);

            // Assert
            Assert.That(result, Is.Null);
        }

        #endregion

        #region ToClassNameOnly (String) Tests

        [Test]
        public void ToClassNameOnly_String_ExtractsClassName()
        {
            // Arrange
            const string fullTypeName = "Dev.Cortez.StateMachines.MockState, Assembly-CSharp";

            // Act
            var result = StateMachineReflectionUtilities.ToClassNameOnly(fullTypeName);

            // Assert
            Assert.That(result, Is.EqualTo("MockState"));
        }

        [Test]
        public void ToClassNameOnly_String_RemovesAssemblyInfo()
        {
            // Arrange
            const string fullTypeName =
                "Dev.Cortez.StateMachines.TestClass, Assembly-CSharp, Version=1.0.0.0, Culture=neutral";

            // Act
            var result = StateMachineReflectionUtilities.ToClassNameOnly(fullTypeName);

            // Assert
            Assert.That(result, Is.EqualTo("TestClass"));
        }

        [Test]
        public void ToClassNameOnly_String_HandlesNestedTypes()
        {
            // Arrange
            const string nestedTypeName = "Dev.Cortez.StateMachines.OuterClass+InnerClass, Assembly-CSharp";

            // Act
            var result = StateMachineReflectionUtilities.ToClassNameOnly(nestedTypeName);

            // Assert
            Assert.That(result, Is.EqualTo("InnerClass"));
        }

        [Test]
        public void ToClassNameOnly_String_HandlesGenericTick()
        {
            // Arrange
            const string genericTypeName = "Dev.Cortez.StateMachines.GenericClass`1, Assembly-CSharp";

            // Act
            var result = StateMachineReflectionUtilities.ToClassNameOnly(genericTypeName);

            // Assert
            Assert.That(result, Is.EqualTo("GenericClass"));
        }

        [Test]
        public void ToClassNameOnly_NullString_ReturnsNull()
        {
            // Act
            var result = StateMachineReflectionUtilities.ToClassNameOnly((string)null);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void ToClassNameOnly_EmptyString_ReturnsEmpty()
        {
            // Act
            var result = StateMachineReflectionUtilities.ToClassNameOnly(string.Empty);

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void ToClassNameOnly_SimpleClassName_ReturnsSame()
        {
            // Arrange
            const string simpleClassName = "SimpleClass";

            // Act
            var result = StateMachineReflectionUtilities.ToClassNameOnly(simpleClassName);

            // Assert
            Assert.That(result, Is.EqualTo("SimpleClass"));
        }

        #endregion

        #region ToClassNameOnly (Type) Tests

        [Test]
        public void ToClassNameOnly_Type_ExtractsClassName()
        {
            // Arrange
            var type = typeof(MockState);

            // Act
            var result = StateMachineReflectionUtilities.ToClassNameOnly(type);

            // Assert
            Assert.That(result, Is.EqualTo("MockState"));
        }

        [Test]
        public void ToClassNameOnly_NullType_ReturnsNull()
        {
            // Act
            var result = StateMachineReflectionUtilities.ToClassNameOnly((System.Type)null);

            // Assert
            Assert.That(result, Is.Null);
        }

        #endregion
    }
}