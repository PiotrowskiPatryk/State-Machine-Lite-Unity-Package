using Dev.Cortez.StateMachines.Core.Data;
using NUnit.Framework;

namespace Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Unit.Core
{
    /// <summary>
    /// Unit tests for EmptyPayload data class.
    /// Tests the IsValid method.
    /// </summary>
    [TestFixture]
    public sealed class EmptyPayloadTests
    {
        #region IsValid Tests

        [Test]
        public void IsValid_AlwaysReturnsTrue()
        {
            // Arrange
            var payload = new EmptyPayload();

            // Act
            var result = payload.IsValid();

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsValid_MultipleInstances_AllReturnTrue()
        {
            // Arrange
            var payload1 = new EmptyPayload();
            var payload2 = new EmptyPayload();

            // Assert
            Assert.That(payload1.IsValid(), Is.True);
            Assert.That(payload2.IsValid(), Is.True);
        }

        #endregion
    }
}
