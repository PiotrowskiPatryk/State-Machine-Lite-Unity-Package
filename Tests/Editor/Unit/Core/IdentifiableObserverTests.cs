using Dev.Cortez.StateMachines.Core.Registry;
using NUnit.Framework;

namespace Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Unit.Core
{
    /// <summary>
    /// Unit tests for IdentifiableObserver class.
    /// Tests observer construction and callback behavior.
    /// </summary>
    [TestFixture]
    public sealed class IdentifiableObserverTests
    {
        #region Constructor Tests

        [Test]
        public void Constructor_SetsIdentifier()
        {
            // Arrange & Act
            var observer = new IdentifiableObserver<string>(
                "test-id",
                _ => { },
                () => { });

            // Assert
            Assert.That(observer.Identifier, Is.EqualTo("test-id"));
        }

        [Test]
        public void Constructor_WithNullIdentifier_SetsUnnamed()
        {
            // Arrange & Act
            var observer = new IdentifiableObserver<string>(
                null,
                _ => { },
                () => { });

            // Assert
            Assert.That(observer.Identifier, Is.EqualTo("Unnamed"));
        }

        [Test]
        public void Constructor_SetsOnRegisteredCallback()
        {
            // Arrange
            var callbackCalled = false;
            string receivedValue = null;

            var observer = new IdentifiableObserver<string>(
                "test-id",
                value =>
                {
                    callbackCalled = true;
                    receivedValue = value;
                },
                () => { });

            // Act
            observer.OnRegistered?.Invoke("test-value");

            // Assert
            Assert.That(callbackCalled, Is.True);
            Assert.That(receivedValue, Is.EqualTo("test-value"));

            // Cleanup
            observer.Dispose();
        }

        [Test]
        public void Constructor_SetsOnUnregisteredCallback()
        {
            // Arrange
            var callbackCalled = false;

            var observer = new IdentifiableObserver<string>(
                "test-id",
                _ => { },
                () => callbackCalled = true);

            // Act
            observer.OnUnregistered?.Invoke();

            // Assert
            Assert.That(callbackCalled, Is.True);

            // Cleanup
            observer.Dispose();
        }

        [Test]
        public void Constructor_WithNullCallbacks_DoesNotThrow()
        {
            IdentifiableObserver<string> observer = null;

            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                observer = new IdentifiableObserver<string>("test-id", null, null);
                Assert.That(observer.OnRegistered, Is.Null);
                Assert.That(observer.OnUnregistered, Is.Null);
            });

            // Cleanup
            observer.Dispose();
        }

        #endregion
    }
}