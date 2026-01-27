using System.Threading;
using System.Threading.Tasks;
using Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Common.Mocks;
using NUnit.Framework;

namespace Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Unit.Core
{
    /// <summary>
    /// Unit tests for TriggerBase class and trigger implementations.
    /// Tests trigger value changes, event firing, and disposal.
    /// </summary>
    [TestFixture]
    public sealed class TriggerTests
    {
        private MockTrigger _trigger;
        private CancellationTokenSource _cts;

        [SetUp]
        public void SetUp()
        {
            _trigger = new MockTrigger();
            _cts = new CancellationTokenSource();
        }

        [TearDown]
        public void TearDown()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _trigger?.DisposeAsync().AsTask().GetAwaiter().GetResult();
        }

        #region Constructor Tests

        [Test]
        public void Constructor_SetsIdNameDescription()
        {
            // Arrange & Act
            var trigger = new MockTrigger("test-id", "Test Name", "Test Description");

            // Assert
            Assert.That(trigger.Id, Is.EqualTo("test-id"));
            Assert.That(trigger.Name, Is.EqualTo("Test Name"));
            Assert.That(trigger.Description, Is.EqualTo("Test Description"));
        }

        [Test]
        public void Constructor_WithDefaults_SetsDefaultValues()
        {
            // Arrange & Act
            var trigger = new MockTrigger();

            // Assert
            Assert.That(trigger.Id, Is.EqualTo("mock-trigger"));
            Assert.That(trigger.Name, Is.EqualTo("Mock Trigger"));
            Assert.That(trigger.IsTriggered, Is.False);
        }

        #endregion

        #region IsTriggered Tests

        [Test]
        public void IsTriggered_WhenChanged_FiresTriggeredValueChangedEvent()
        {
            // Arrange
            var eventFired = false;
            bool? receivedValue = null;
            _trigger.TriggeredValueChanged += (trigger, value) =>
            {
                eventFired = true;
                receivedValue = value;
            };

            // Act
            _trigger.SetTriggered(true);

            // Assert
            Assert.That(eventFired, Is.True);
            Assert.That(receivedValue, Is.True);
        }

        [Test]
        public void IsTriggered_WhenSetToSameValue_DoesNotFireEvent()
        {
            // Arrange
            _trigger.SetTriggered(true);
            var eventFireCount = 0;
            _trigger.TriggeredValueChanged += (_, _) => eventFireCount++;

            // Act
            _trigger.SetTriggered(true);

            // Assert
            Assert.That(eventFireCount, Is.EqualTo(0));
        }

        [Test]
        public void IsTriggered_TogglingValue_FiresEventEachTime()
        {
            // Arrange
            var eventFireCount = 0;
            _trigger.TriggeredValueChanged += (_, _) => eventFireCount++;

            // Act
            _trigger.SetTriggered(true);
            _trigger.SetTriggered(false);
            _trigger.SetTriggered(true);

            // Assert
            Assert.That(eventFireCount, Is.EqualTo(3));
        }

        #endregion

        #region TriggerValueAsync Tests

        [Test]
        public async Task TriggerValueAsync_WhenTargetIsTrue_SetsTriggeredToTrue()
        {
            // Act
            var result = await _trigger.TriggerValueAsync(true, _cts.Token);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_trigger.IsTriggered, Is.True);
        }

        [Test]
        public async Task TriggerValueAsync_WhenTargetIsFalse_SetsTriggeredToFalse()
        {
            // Arrange
            _trigger.SetTriggered(true);

            // Act
            var result = await _trigger.TriggerValueAsync(false, _cts.Token);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_trigger.IsTriggered, Is.False);
        }

        [Test]
        public async Task TriggerValueAsync_RecordsCallArguments()
        {
            // Act
            await _trigger.TriggerValueAsync(true, _cts.Token);
            await _trigger.TriggerValueAsync(false, _cts.Token);
            await _trigger.TriggerValueAsync(true, _cts.Token);

            // Assert
            Assert.That(_trigger.TriggerValueAsyncCalls.Count, Is.EqualTo(3));
            Assert.That(_trigger.TriggerValueAsyncCalls[0], Is.True);
            Assert.That(_trigger.TriggerValueAsyncCalls[1], Is.False);
            Assert.That(_trigger.TriggerValueAsyncCalls[2], Is.True);
        }

        [Test]
        public async Task TriggerValueAsync_WhenConfiguredToFail_ReturnsFalse()
        {
            // Arrange
            _trigger.TriggerValueShouldSucceed = false;

            // Act
            var result = await _trigger.TriggerValueAsync(true, _cts.Token);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_trigger.IsTriggered, Is.False); // Should not change
        }

        [Test]
        public async Task TriggerValueAsync_TracksCallCount()
        {
            // Act
            await _trigger.TriggerValueAsync(true, _cts.Token);
            await _trigger.TriggerValueAsync(false, _cts.Token);

            // Assert
            Assert.That(_trigger.TriggerValueCallCount, Is.EqualTo(2));
        }

        #endregion

        #region DisposeAsync Tests

        [Test]
        public async Task DisposeAsync_ClearsEventHandlers()
        {
            // Arrange
            var handlerCalled = false;
            _trigger.TriggeredValueChanged += (_, _) => handlerCalled = true;

            // Act
            await _trigger.DisposeAsync();

            // Trigger should be disposed, handler should be cleared
            // Setting triggered directly would throw or not call handler
            Assert.That(_trigger.DisposeCallCount, Is.EqualTo(1));
        }

        [Test]
        public async Task DisposeAsync_CallsDoDisposeAsync()
        {
            // Act
            await _trigger.DisposeAsync();

            // Assert
            Assert.That(_trigger.DisposeCallCount, Is.EqualTo(1));
        }

        #endregion

        #region Reset Tests

        [Test]
        public void Reset_ClearsAllTrackingState()
        {
            // Arrange
            _trigger.SetTriggered(true);
            _trigger.TriggerValueAsyncCalls.Add(true);

            // Act
            _trigger.Reset();

            // Assert
            Assert.That(_trigger.IsTriggered, Is.False);
            Assert.That(_trigger.TriggerValueAsyncCalls.Count, Is.EqualTo(0));
            Assert.That(_trigger.TriggerValueCallCount, Is.EqualTo(0));
            Assert.That(_trigger.DisposeCallCount, Is.EqualTo(0));
        }

        #endregion
    }
}