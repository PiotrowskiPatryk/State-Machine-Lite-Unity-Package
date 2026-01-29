using System.Threading;
using System.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Condition;
using Dev.Cortez.StateMachines.Core.Condition.Payload;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Core.ReferencePicker;
using Dev.Cortez.StateMachines.Core.ReferencePicker.Trigger;
using Dev.Cortez.StateMachines.Core.Registry;
using Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Common.Mocks;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Unit.Core
{
    /// <summary>
    /// Unit tests for TriggerBasedCondition.
    /// Tests initialization, cleanup, and condition satisfaction logic using the Registry singleton.
    /// </summary>
    [TestFixture]
    public sealed class TriggerBasedConditionTests
    {
        private MockTrigger _mockTrigger;
        private StateMachineContainerEntry _containerEntry;

        [SetUp]
        public void SetUp()
        {
            _mockTrigger = new MockTrigger("test-trigger-1", "Test Trigger", "Desc");
            _containerEntry = new StateMachineContainerEntry("test-container-tbc");
            _containerEntry.TryRegisterTrigger(_mockTrigger);
            StateMachineContainerRegistry.Instance.RegisterStateMachineContainer(_containerEntry);
        }

        [TearDown]
        public void TearDown()
        {
            StateMachineContainerRegistry.Instance.UnregisterStateMachineContainer(_containerEntry);
        }

        #region Event Tests

        [Test]
        public async Task TriggerValueChanged_FiresSatisfiedChanged()
        {
            // Arrange
            var condition = new TriggerBasedCondition();
            var payload = new TriggerBasedConditionPayload(
                new TriggerReferencePicker("test-trigger-1"),
                TriggerSatisfiedConditionType.WhenTriggered);

            await condition.InitializeAsync(payload, CancellationToken.None);

            bool? lastEventValue = null;
            ICondition eventCondition = null;
            condition.SatisfiedChanged += (cond, val) =>
            {
                eventCondition = cond;
                lastEventValue = val;
            };

            // Act
            _mockTrigger.IsTriggered = true;

            // Assert
            Assert.That(lastEventValue, Is.True);
            Assert.That(eventCondition, Is.SameAs(condition));

            await condition.DisposeAsync();
        }

        #endregion

        #region Dispose Tests

        [Test]
        public async Task DisposeAsync_UnsubscribesFromTrigger()
        {
            // Arrange
            var condition = new TriggerBasedCondition();
            var payload = new TriggerBasedConditionPayload(
                new TriggerReferencePicker("test-trigger-1"),
                TriggerSatisfiedConditionType.WhenTriggered);

            await condition.InitializeAsync(payload, CancellationToken.None);

            var eventFired = false;
            condition.SatisfiedChanged += (_, __) => eventFired = true;

            // Act
            await condition.DisposeAsync();
            _mockTrigger.IsTriggered = true;

            // Assert - Event should NOT fire after disposal
            Assert.That(eventFired, Is.False);
        }

        #endregion

        #region Initialization Tests

        [Test]
        public async Task InitializeAsync_WhenReferenceSelected_ReturnsTrue()
        {
            // Arrange
            var condition = new TriggerBasedCondition();
            var picker = new TriggerReferencePicker("test-trigger-1");
            var payload = new TriggerBasedConditionPayload(picker, TriggerSatisfiedConditionType.WhenTriggered);

            // Act
            var result = await condition.InitializeAsync(payload, CancellationToken.None);

            // Assert
            Assert.That(result, Is.True);

            await condition.DisposeAsync();
        }

        [Test]
        public async Task InitializeAsync_WhenReferenceNotSelected_ReturnsFalse()
        {
            // Arrange
            LogAssert.ignoreFailingMessages = true;
            var condition = new TriggerBasedCondition();
            var picker = new TriggerReferencePicker(); // No ID selected
            var payload = new TriggerBasedConditionPayload(picker, TriggerSatisfiedConditionType.WhenTriggered);

            // Act
            var result = await condition.InitializeAsync(payload, CancellationToken.None);

            // Assert
            Assert.That(result, Is.False);

            await condition.DisposeAsync();
        }

        #endregion

        #region IsSatisfied Tests

        [Test]
        public async Task IsSatisfied_WhenTriggered_AndTypeIsWhenTriggered_ReturnsTrue()
        {
            // Arrange
            var condition = new TriggerBasedCondition();
            var payload = new TriggerBasedConditionPayload(
                new TriggerReferencePicker("test-trigger-1"),
                TriggerSatisfiedConditionType.WhenTriggered);

            await condition.InitializeAsync(payload, CancellationToken.None);

            // Act
            _mockTrigger.IsTriggered = true;

            // Assert
            Assert.That(condition.IsSatisfied, Is.True);

            await condition.DisposeAsync();
        }

        [Test]
        public async Task IsSatisfied_WhenNotTriggered_AndTypeIsWhenTriggered_ReturnsFalse()
        {
            // Arrange
            var condition = new TriggerBasedCondition();
            var payload = new TriggerBasedConditionPayload(
                new TriggerReferencePicker("test-trigger-1"),
                TriggerSatisfiedConditionType.WhenTriggered);

            await condition.InitializeAsync(payload, CancellationToken.None);

            // Act
            _mockTrigger.IsTriggered = false;

            // Assert
            Assert.That(condition.IsSatisfied, Is.False);

            await condition.DisposeAsync();
        }

        [Test]
        public async Task IsSatisfied_WhenTriggered_AndTypeIsWhenNotTriggered_ReturnsFalse()
        {
            // Arrange
            var condition = new TriggerBasedCondition();
            var payload = new TriggerBasedConditionPayload(
                new TriggerReferencePicker("test-trigger-1"),
                TriggerSatisfiedConditionType.WhenNotTriggered);

            await condition.InitializeAsync(payload, CancellationToken.None);

            // Act
            _mockTrigger.IsTriggered = true;

            // Assert
            Assert.That(condition.IsSatisfied, Is.False);

            await condition.DisposeAsync();
        }

        [Test]
        public async Task IsSatisfied_WhenNotTriggered_AndTypeIsWhenNotTriggered_ReturnsTrue()
        {
            // Arrange
            var condition = new TriggerBasedCondition();
            var payload = new TriggerBasedConditionPayload(
                new TriggerReferencePicker("test-trigger-1"),
                TriggerSatisfiedConditionType.WhenNotTriggered);

            await condition.InitializeAsync(payload, CancellationToken.None);

            // Act
            _mockTrigger.IsTriggered = false;

            // Assert
            Assert.That(condition.IsSatisfied, Is.True);

            await condition.DisposeAsync();
        }

        [Test]
        public async Task IsSatisfied_WhenTypeIsUndefined_ReturnsFalse()
        {
            // Arrange
            var condition = new TriggerBasedCondition();
            var payload = new TriggerBasedConditionPayload(
                new TriggerReferencePicker("test-trigger-1"),
                TriggerSatisfiedConditionType.Undefined);

            await condition.InitializeAsync(payload, CancellationToken.None);

            // Act
            _mockTrigger.IsTriggered = true;

            // Assert
            Assert.That(condition.IsSatisfied, Is.False);

            await condition.DisposeAsync();
        }

        #endregion
    }
}