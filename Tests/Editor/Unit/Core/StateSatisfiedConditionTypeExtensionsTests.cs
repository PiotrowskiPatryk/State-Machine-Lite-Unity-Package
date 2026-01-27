using Dev.Cortez.StateMachines.Core.Condition.Payload;
using Dev.Cortez.StateMachines.Core.Data;
using NUnit.Framework;

namespace Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Unit.Core
{
    /// <summary>
    /// Unit tests for StateSatisfiedConditionTypeExtensions.
    /// Tests the IsValidStatement extension method for various combinations.
    /// </summary>
    [TestFixture]
    public sealed class StateSatisfiedConditionTypeExtensionsTests
    {
        #region WhenActivating Tests

        [Test]
        public void IsValidStatement_WhenActivating_AndStateIsActivating_ReturnsTrue()
        {
            // Arrange
            var conditionType = StateSatisfiedConditionType.WhenActivating;

            // Act
            var result = conditionType.IsValidStatement(StateStatus.Activating);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsValidStatement_WhenActivating_AndStateIsActive_ReturnsFalse()
        {
            // Arrange
            var conditionType = StateSatisfiedConditionType.WhenActivating;

            // Act
            var result = conditionType.IsValidStatement(StateStatus.Active);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region WhenActivated Tests

        [Test]
        public void IsValidStatement_WhenActivated_AndStateIsActive_ReturnsTrue()
        {
            // Arrange
            var conditionType = StateSatisfiedConditionType.WhenActivated;

            // Act
            var result = conditionType.IsValidStatement(StateStatus.Active);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsValidStatement_WhenActivated_AndStateIsActivating_ReturnsFalse()
        {
            // Arrange
            var conditionType = StateSatisfiedConditionType.WhenActivated;

            // Act
            var result = conditionType.IsValidStatement(StateStatus.Activating);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region WhenDeactivating Tests

        [Test]
        public void IsValidStatement_WhenDeactivating_AndStateIsDeactivating_ReturnsTrue()
        {
            // Arrange
            var conditionType = StateSatisfiedConditionType.WhenDeactivating;

            // Act
            var result = conditionType.IsValidStatement(StateStatus.Deactivating);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsValidStatement_WhenDeactivating_AndStateIsInactive_ReturnsFalse()
        {
            // Arrange
            var conditionType = StateSatisfiedConditionType.WhenDeactivating;

            // Act
            var result = conditionType.IsValidStatement(StateStatus.Inactive);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region WhenDeactivated Tests

        [Test]
        public void IsValidStatement_WhenDeactivated_AndStateIsInactive_ReturnsTrue()
        {
            // Arrange
            var conditionType = StateSatisfiedConditionType.WhenDeactivated;

            // Act
            var result = conditionType.IsValidStatement(StateStatus.Inactive);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsValidStatement_WhenDeactivated_AndStateIsDeactivating_ReturnsFalse()
        {
            // Arrange
            var conditionType = StateSatisfiedConditionType.WhenDeactivated;

            // Act
            var result = conditionType.IsValidStatement(StateStatus.Deactivating);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region Combined Flags Tests

        [Test]
        public void IsValidStatement_WhenActivatedOrDeactivated_AndStateIsActive_ReturnsTrue()
        {
            // Arrange - Using combined flags
            var conditionType = StateSatisfiedConditionType.WhenActivated |
                                StateSatisfiedConditionType.WhenDeactivated;

            // Act
            var result = conditionType.IsValidStatement(StateStatus.Active);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsValidStatement_WhenActivatedOrDeactivated_AndStateIsInactive_ReturnsTrue()
        {
            // Arrange
            var conditionType = StateSatisfiedConditionType.WhenActivated |
                                StateSatisfiedConditionType.WhenDeactivated;

            // Act
            var result = conditionType.IsValidStatement(StateStatus.Inactive);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsValidStatement_WhenActivatedOrDeactivated_AndStateIsActivating_ReturnsFalse()
        {
            // Arrange
            var conditionType = StateSatisfiedConditionType.WhenActivated |
                                StateSatisfiedConditionType.WhenDeactivated;

            // Act
            var result = conditionType.IsValidStatement(StateStatus.Activating);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region None Tests

        [Test]
        public void IsValidStatement_WhenNone_AndAnyState_ReturnsFalse()
        {
            // Arrange
            var conditionType = StateSatisfiedConditionType.None;

            // Act & Assert
            Assert.That(conditionType.IsValidStatement(StateStatus.Activating), Is.False);
            Assert.That(conditionType.IsValidStatement(StateStatus.Active), Is.False);
            Assert.That(conditionType.IsValidStatement(StateStatus.Deactivating), Is.False);
            Assert.That(conditionType.IsValidStatement(StateStatus.Inactive), Is.False);
            Assert.That(conditionType.IsValidStatement(StateStatus.Undefined), Is.False);
        }

        #endregion

        #region Undefined State Tests

        [Test]
        public void IsValidStatement_WhenAnyType_AndStateIsUndefined_ReturnsFalse()
        {
            // Arrange
            var allTypes = StateSatisfiedConditionType.WhenActivating |
                           StateSatisfiedConditionType.WhenActivated |
                           StateSatisfiedConditionType.WhenDeactivating |
                           StateSatisfiedConditionType.WhenDeactivated;

            // Act
            var result = allTypes.IsValidStatement(StateStatus.Undefined);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion
    }
}
