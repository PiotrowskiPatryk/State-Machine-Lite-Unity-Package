using Dev.Cortez.StateMachines.Core.Data;
using Dev.Cortez.StateMachines.Extensions;
using NUnit.Framework;

namespace Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Unit.Core
{
    /// <summary>
    /// Unit tests for InitializationStatusExtensions.
    /// Tests the CanInitialize extension method for all status values.
    /// </summary>
    [TestFixture]
    public sealed class InitializationStatusExtensionsTests
    {
        [Test]
        public void CanInitialize_WhenUndefined_ReturnsFalse()
        {
            // Arrange
            var status = InitializationStatus.Undefined;

            // Act
            var result = status.CanInitialize();

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void CanInitialize_WhenNotInitialized_ReturnsTrue()
        {
            // Arrange
            var status = InitializationStatus.NotInitialized;

            // Act
            var result = status.CanInitialize();

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void CanInitialize_WhenInitializing_ReturnsFalse()
        {
            // Arrange
            var status = InitializationStatus.Initializing;

            // Act
            var result = status.CanInitialize();

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void CanInitialize_WhenInitialized_ReturnsFalse()
        {
            // Arrange
            var status = InitializationStatus.Initialized;

            // Act
            var result = status.CanInitialize();

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void CanInitialize_WhenFailed_ReturnsTrue()
        {
            // Arrange
            var status = InitializationStatus.Failed;

            // Act
            var result = status.CanInitialize();

            // Assert
            Assert.That(result, Is.True);
        }
    }
}