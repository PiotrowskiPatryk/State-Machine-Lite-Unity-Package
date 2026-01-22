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
    }
}