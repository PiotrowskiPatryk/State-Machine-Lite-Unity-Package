using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Data;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Core.Trigger;
using Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Common.Builders;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Unit.Core
{
    /// <summary>
    /// Comprehensive unit tests for DefaultTrigger covering initialization,
    /// all activation/deactivation rule variants, cancellation, disposal, and events.
    /// </summary>
    [TestFixture]
    public sealed class DefaultTriggerTests
    {
        private DefaultTrigger _trigger;
        private CancellationTokenSource _cts;

        [SetUp]
        public void SetUp()
        {
            _trigger = new DefaultTrigger("trigger-1", "Test Trigger", "Test Description");
            _cts = new CancellationTokenSource();
        }

        [TearDown]
        public void TearDown()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _trigger?.DisposeAsync().AsTask().GetAwaiter().GetResult();
        }

        #region Activation - AfterFixedFrame

        [UnityTest]
        public IEnumerator TriggerValueAsync_ActivationAfterFixedFrame_ActivatesAfterFrameDelay()
        {
            // Arrange
            var payload = DefaultTriggerPayloadBuilder.Create().
                WithActivationRule(TriggerActivationRule.AfterFixedFrame).WithActivationFrameDelay(1).
                WithDeactivationRule(TriggerDeactivationRule.Never).Build();

            yield return _trigger.InitializeAsync(payload, _cts.Token).ToCoroutine();

            // Act - start the trigger but don't await completion immediately
            var task = _trigger.TriggerValueAsync(true, _cts.Token);

            // Assert - should not be triggered yet (delay pending)
            Assert.That(_trigger.IsTriggered, Is.False,
                "Trigger should not be active before frame delay completes");

            // Wait for task to complete
            yield return task.ToCoroutine();

            // Assert - after delay, should be triggered
            Assert.That(_trigger.IsTriggered, Is.True,
                "Trigger should be active after frame delay");
        }

        #endregion

        #region Deactivation - Never

        [Test]
        public async Task TriggerValueAsync_DeactivationNever_StaysTriggeredIndefinitely()
        {
            // Arrange
            var payload = DefaultTriggerPayloadBuilder.Create().WithActivationRule(TriggerActivationRule.Immediately).
                WithDeactivationRule(TriggerDeactivationRule.Never).Build();
            await InitializeTrigger(payload);

            // Act
            await _trigger.TriggerValueAsync(true, _cts.Token);

            // Assert
            Assert.That(_trigger.IsTriggered, Is.True,
                "Trigger should remain active with DeactivationRule.Never");
        }

        #endregion

        #region Deactivation - NextFrame

        [UnityTest]
        public IEnumerator TriggerValueAsync_DeactivationNextFrame_ActivatesThenDeactivatesAfterYield()
        {
            // Arrange
            var payload = DefaultTriggerPayloadBuilder.Create().WithActivationRule(TriggerActivationRule.Immediately).
                WithDeactivationRule(TriggerDeactivationRule.NextFrame).Build();

            yield return _trigger.InitializeAsync(payload, _cts.Token).ToCoroutine();

            // Act
            var task = _trigger.TriggerValueAsync(true, _cts.Token);

            // Assert - Should be triggered immediately after activation
            Assert.That(_trigger.IsTriggered, Is.True,
                "Trigger should be active before deactivation yield");

            // Wait for the deactivation yield to complete
            yield return task.ToCoroutine();

            // Assert - Should be deactivated after yield
            Assert.That(_trigger.IsTriggered, Is.False,
                "Trigger should be deactivated after NextFrame yield");
        }

        #endregion

        #region Deactivation - AfterFixedFrame

        [UnityTest]
        public IEnumerator TriggerValueAsync_DeactivationAfterFixedFrame_DeactivatesAfterFrameDelay()
        {
            // Arrange
            var payload = DefaultTriggerPayloadBuilder.Create().WithActivationRule(TriggerActivationRule.Immediately).
                WithDeactivationRule(TriggerDeactivationRule.AfterFixedFrame).WithDeactivationFrameDelay(1).Build();

            yield return _trigger.InitializeAsync(payload, _cts.Token).ToCoroutine();

            // Act
            var task = _trigger.TriggerValueAsync(true, _cts.Token);

            // Assert - Should be activated immediately
            Assert.That(_trigger.IsTriggered, Is.True,
                "Trigger should be active before deactivation frame delay");

            // Wait for deactivation
            yield return task.ToCoroutine();

            // Assert - Should be deactivated after frame delay
            Assert.That(_trigger.IsTriggered, Is.False,
                "Trigger should be deactivated after frame delay");
        }

        #endregion

        #region Disposal Tests

        [UnityTest]
        public IEnumerator DisposeAsync_CancelsActiveLifecycleToken()
        {
            // Arrange - trigger with delayed deactivation
            var payload = DefaultTriggerPayloadBuilder.Create().WithActivationRule(TriggerActivationRule.Immediately).
                WithDeactivationRule(TriggerDeactivationRule.AfterFixedFrame).WithDeactivationFrameDelay(100).Build();

            yield return _trigger.InitializeAsync(payload, _cts.Token).ToCoroutine();

            // Start trigger
            var task = _trigger.TriggerValueAsync(true, _cts.Token);
            Assert.That(_trigger.IsTriggered, Is.True);

            // Act - dispose the trigger while lifecycle is active
            yield return _trigger.DisposeAsync().AsTask().AsUniTask().ToCoroutine();

            var result = false;

            yield return UniTask.ToCoroutine(async () => { result = await task; });

            // Assert - the lifecycle should have been cancelled
            Assert.That(result, Is.False, "Active lifecycle should be cancelled on dispose");

            // Prevent TearDown from double-disposing
            _trigger = null;
        }

        #endregion

        #region Helper Methods

        private async Task InitializeTrigger(DefaultTriggerPayload payload)
        {
            var result = await _trigger.InitializeAsync(payload, _cts.Token);

            Assert.That(result, Is.True, "Trigger initialization should succeed");
        }

        private async Task InitializeTriggerWithDefaults()
        {
            var payload = DefaultTriggerPayloadBuilder.Create().Build();

            await InitializeTrigger(payload);
        }

        #endregion

        #region Initialization Tests

        [Test]
        public async Task InitializeAsync_WithValidPayload_SetsStatusToInitialized()
        {
            // Arrange
            var payload = DefaultTriggerPayloadBuilder.Create().Build();

            // Act
            var result = await _trigger.InitializeAsync(payload, _cts.Token);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_trigger.InitializationStatus, Is.EqualTo(InitializationStatus.Initialized));
        }

        [Test]
        public async Task InitializeAsync_WithInvalidPayloadType_SetsStatusToFailed()
        {
            // Arrange
            var wrongPayload = new EmptyPayload();

            // Act
            var result = await _trigger.InitializeAsync(wrongPayload, _cts.Token);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_trigger.InitializationStatus, Is.EqualTo(InitializationStatus.Failed));
        }

        [Test]
        public async Task InitializeAsync_WhenAlreadyInitialized_ReturnsFalse()
        {
            // Arrange
            var payload = DefaultTriggerPayloadBuilder.Create().Build();
            await _trigger.InitializeAsync(payload, _cts.Token);

            LogAssert.Expect(UnityEngine.LogType.Error,
                "[StateMachines]: Unable to initialize trigger: [id:trigger-1, name:Test Trigger]");

            // Act
            var result = await _trigger.InitializeAsync(payload, _cts.Token);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region Activation - Immediately

        [Test]
        public async Task TriggerValueAsync_ActivationImmediately_DeactivationNever_ActivatesAndStaysTriggered()
        {
            // Arrange
            var payload = DefaultTriggerPayloadBuilder.Create().WithActivationRule(TriggerActivationRule.Immediately).
                WithDeactivationRule(TriggerDeactivationRule.Never).Build();
            await InitializeTrigger(payload);

            // Act
            var result = await _trigger.TriggerValueAsync(true, _cts.Token);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_trigger.IsTriggered, Is.True);
        }

        [Test]
        public async Task TriggerValueAsync_ActivationImmediately_ReturnsTrue()
        {
            // Arrange
            var payload = DefaultTriggerPayloadBuilder.Create().WithActivationRule(TriggerActivationRule.Immediately).
                WithDeactivationRule(TriggerDeactivationRule.Never).Build();
            await InitializeTrigger(payload);

            // Act
            var result = await _trigger.TriggerValueAsync(true, _cts.Token);

            // Assert
            Assert.That(result, Is.True);
        }

        #endregion

        #region Early Return / Short-Circuit Tests

        [Test]
        public async Task TriggerValueAsync_WhenAlreadyMatchingTargetValue_ReturnsTrueImmediately()
        {
            // Arrange
            await InitializeTriggerWithDefaults();
            _trigger.IsTriggered = true;

            // Act
            var result = await _trigger.TriggerValueAsync(true, _cts.Token);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_trigger.IsTriggered, Is.True, "Should stay in the same state (early return)");
        }

        [Test]
        public async Task TriggerValueAsync_WhenNotTriggeredAndTargetFalse_ReturnsTrueImmediately()
        {
            // Arrange
            await InitializeTriggerWithDefaults();

            // Act - target is false and IsTriggered is already false
            var result = await _trigger.TriggerValueAsync(false, _cts.Token);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_trigger.IsTriggered, Is.False);
        }

        [Test]
        public async Task TriggerValueAsync_WhenTargetFalse_ResetsTriggeredToFalse()
        {
            // Arrange
            await InitializeTriggerWithDefaults();
            await _trigger.TriggerValueAsync(true, _cts.Token);

            Assert.That(_trigger.IsTriggered, Is.True, "Precondition: should be triggered");

            // Act
            var result = await _trigger.TriggerValueAsync(false, _cts.Token);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_trigger.IsTriggered, Is.False);
        }

        #endregion

        #region Cancellation Tests

        [UnityTest]
        public IEnumerator TriggerValueAsync_WhenCancelledDuringActivation_ReturnsFalse()
        {
            // Arrange - use a frame-delayed activation so we can cancel during it
            var payload = DefaultTriggerPayloadBuilder.Create().
                WithActivationRule(TriggerActivationRule.AfterFixedFrame).WithActivationFrameDelay(10).
                WithDeactivationRule(TriggerDeactivationRule.Never).Build();

            yield return _trigger.InitializeAsync(payload, _cts.Token).ToCoroutine();

            // Act - start the trigger and cancel immediately
            var task = _trigger.TriggerValueAsync(true, _cts.Token);
            _cts.Cancel();

            // Wait for task to complete
            var result = false;

            yield return UniTask.ToCoroutine(async () => { result = await task; });

            // Assert
            Assert.That(result, Is.False, "Should return false when cancelled during activation delay");
            Assert.That(_trigger.IsTriggered, Is.False, "Should not be triggered when cancelled");
        }

        [UnityTest]
        public IEnumerator TriggerValueAsync_WhenCancelledDuringDeactivation_ReturnsFalse()
        {
            // Arrange - immediate activation, frame-delayed deactivation
            var payload = DefaultTriggerPayloadBuilder.Create().WithActivationRule(TriggerActivationRule.Immediately).
                WithDeactivationRule(TriggerDeactivationRule.AfterFixedFrame).WithDeactivationFrameDelay(10).Build();

            yield return _trigger.InitializeAsync(payload, _cts.Token).ToCoroutine();

            // Act - start trigger, then cancel during deactivation
            var task = _trigger.TriggerValueAsync(true, _cts.Token);

            // Trigger should be active at this point (immediate activation)
            Assert.That(_trigger.IsTriggered, Is.True, "Should be triggered before cancel");

            _cts.Cancel();

            var result = false;

            yield return UniTask.ToCoroutine(async () => { result = await task; });

            // Assert
            Assert.That(result, Is.False, "Should return false when cancelled during deactivation delay");
        }

        [UnityTest]
        public IEnumerator TriggerValueAsync_CallingAgainCancelsPreviousLifecycle()
        {
            // Arrange - trigger with long frame delay deactivation
            var payload = DefaultTriggerPayloadBuilder.Create().WithActivationRule(TriggerActivationRule.Immediately).
                WithDeactivationRule(TriggerDeactivationRule.AfterFixedFrame).WithDeactivationFrameDelay(100).Build();

            yield return _trigger.InitializeAsync(payload, _cts.Token).ToCoroutine();

            // Act - start a first trigger call
            var firstTask = _trigger.TriggerValueAsync(true, _cts.Token);
            Assert.That(_trigger.IsTriggered, Is.True);

            // Call TriggerValueAsync again with false - should cancel the in-flight lifecycle
            var secondTask = _trigger.TriggerValueAsync(false, _cts.Token);

            var firstResult = false;
            var secondResult = false;

            yield return UniTask.ToCoroutine(async () =>
            {
                firstResult = await firstTask;
                secondResult = await secondTask;
            });

            // Assert - first task was cancelled by re-entry (returns false), second completed normally
            Assert.That(firstResult, Is.False, "First lifecycle should be cancelled");
            Assert.That(secondResult, Is.True, "Second call should succeed");
            Assert.That(_trigger.IsTriggered, Is.False, "Should be deactivated after second call");
        }

        #endregion

        #region TriggeredValueChanged Event Tests

        [Test]
        public async Task TriggerValueAsync_WhenActivated_FiresTriggeredValueChangedEvent()
        {
            // Arrange
            var payload = DefaultTriggerPayloadBuilder.Create().WithActivationRule(TriggerActivationRule.Immediately).
                WithDeactivationRule(TriggerDeactivationRule.Never).Build();
            await InitializeTrigger(payload);

            var eventFired = false;
            bool? receivedValue = null;
            _trigger.TriggeredValueChanged += (_, value) =>
            {
                eventFired = true;
                receivedValue = value;
            };

            // Act
            await _trigger.TriggerValueAsync(true, _cts.Token);

            // Assert
            Assert.That(eventFired, Is.True, "TriggeredValueChanged should fire on activation");
            Assert.That(receivedValue, Is.True);
        }

        [UnityTest]
        public IEnumerator TriggerValueAsync_WhenDeactivated_FiresTriggeredValueChangedEventTwice()
        {
            // Arrange - activate immediately, deactivate next frame
            var payload = DefaultTriggerPayloadBuilder.Create().WithActivationRule(TriggerActivationRule.Immediately).
                WithDeactivationRule(TriggerDeactivationRule.NextFrame).Build();

            yield return _trigger.InitializeAsync(payload, _cts.Token).ToCoroutine();

            var fireCount = 0;
            _trigger.TriggeredValueChanged += (_, _) => fireCount++;

            // Act
            yield return _trigger.TriggerValueAsync(true, _cts.Token).ToCoroutine();

            // Assert - once for true (activation), once for false (deactivation)
            Assert.That(fireCount, Is.EqualTo(2),
                "Event should fire twice: once on activation, once on deactivation");
        }

        #endregion

        #region DefaultTriggerPayload.IsValid() Tests

        [Test]
        public void IsValid_Immediately_Never_ReturnsTrue()
        {
            var payload = DefaultTriggerPayloadBuilder.Create().WithActivationRule(TriggerActivationRule.Immediately).
                WithDeactivationRule(TriggerDeactivationRule.Never).Build();

            Assert.That(payload.IsValid(), Is.True);
        }

        [Test]
        public void IsValid_AfterFixedFrame_WithValidFrameDelay_ReturnsTrue()
        {
            var payload = DefaultTriggerPayloadBuilder.Create().
                WithActivationRule(TriggerActivationRule.AfterFixedFrame).WithActivationFrameDelay(5).
                WithDeactivationRule(TriggerDeactivationRule.Never).Build();

            Assert.That(payload.IsValid(), Is.True);
        }

        [Test]
        public void IsValid_AfterFixedFrame_WithZeroFrameDelay_ReturnsFalse()
        {
            var payload = DefaultTriggerPayloadBuilder.Create().
                WithActivationRule(TriggerActivationRule.AfterFixedFrame).WithActivationFrameDelay(0).
                WithDeactivationRule(TriggerDeactivationRule.Never).Build();

            Assert.That(payload.IsValid(), Is.False);
        }

        [Test]
        public void IsValid_AfterTime_WithValidTimeDelay_ReturnsTrue()
        {
            var payload = DefaultTriggerPayloadBuilder.Create().WithActivationRule(TriggerActivationRule.AfterTime).
                WithActivationTimeDelay(1.5f).WithDeactivationRule(TriggerDeactivationRule.Never).Build();

            Assert.That(payload.IsValid(), Is.True);
        }

        [Test]
        public void IsValid_AfterTime_WithZeroTimeDelay_ReturnsTrue()
        {
            var payload = DefaultTriggerPayloadBuilder.Create().WithActivationRule(TriggerActivationRule.AfterTime).
                WithActivationTimeDelay(0f).WithDeactivationRule(TriggerDeactivationRule.Never).Build();

            Assert.That(payload.IsValid(), Is.True);
        }

        [Test]
        public void IsValid_AfterTime_WithNegativeTimeDelay_ReturnsFalse()
        {
            var payload = DefaultTriggerPayloadBuilder.Create().WithActivationRule(TriggerActivationRule.AfterTime).
                WithActivationTimeDelay(-1f).WithDeactivationRule(TriggerDeactivationRule.Never).Build();

            Assert.That(payload.IsValid(), Is.False);
        }

        [Test]
        public void IsValid_AfterTimeUnscaled_WithValidTimeDelay_ReturnsTrue()
        {
            var payload = DefaultTriggerPayloadBuilder.Create().
                WithActivationRule(TriggerActivationRule.AfterTimeUnscaled).WithActivationTimeDelay(2.0f).
                WithDeactivationRule(TriggerDeactivationRule.Never).Build();

            Assert.That(payload.IsValid(), Is.True);
        }

        [Test]
        public void IsValid_DeactivationNextFrame_AlwaysValid()
        {
            var payload = DefaultTriggerPayloadBuilder.Create().WithActivationRule(TriggerActivationRule.Immediately).
                WithDeactivationRule(TriggerDeactivationRule.NextFrame).Build();

            Assert.That(payload.IsValid(), Is.True);
        }

        [Test]
        public void IsValid_DeactivationAfterFixedFrame_WithValidDelay_ReturnsTrue()
        {
            var payload = DefaultTriggerPayloadBuilder.Create().WithActivationRule(TriggerActivationRule.Immediately).
                WithDeactivationRule(TriggerDeactivationRule.AfterFixedFrame).WithDeactivationFrameDelay(3).Build();

            Assert.That(payload.IsValid(), Is.True);
        }

        [Test]
        public void IsValid_DeactivationAfterFixedFrame_WithZeroDelay_ReturnsFalse()
        {
            var payload = DefaultTriggerPayloadBuilder.Create().WithActivationRule(TriggerActivationRule.Immediately).
                WithDeactivationRule(TriggerDeactivationRule.AfterFixedFrame).WithDeactivationFrameDelay(0).Build();

            Assert.That(payload.IsValid(), Is.False);
        }

        [Test]
        public void IsValid_DeactivationAfterTime_WithValidDelay_ReturnsTrue()
        {
            var payload = DefaultTriggerPayloadBuilder.Create().WithActivationRule(TriggerActivationRule.Immediately).
                WithDeactivationRule(TriggerDeactivationRule.AfterTime).WithDeactivationTimeDelay(0.5f).Build();

            Assert.That(payload.IsValid(), Is.True);
        }

        [Test]
        public void IsValid_DeactivationAfterTime_WithNegativeDelay_ReturnsFalse()
        {
            var payload = DefaultTriggerPayloadBuilder.Create().WithActivationRule(TriggerActivationRule.Immediately).
                WithDeactivationRule(TriggerDeactivationRule.AfterTime).WithDeactivationTimeDelay(-0.5f).Build();

            Assert.That(payload.IsValid(), Is.False);
        }

        [Test]
        public void IsValid_DeactivationAfterTimeUnscaled_WithValidDelay_ReturnsTrue()
        {
            var payload = DefaultTriggerPayloadBuilder.Create().WithActivationRule(TriggerActivationRule.Immediately).
                WithDeactivationRule(TriggerDeactivationRule.AfterTimeUnscaled).WithDeactivationTimeDelay(1.0f).Build();

            Assert.That(payload.IsValid(), Is.True);
        }

        [Test]
        public void IsValid_UndefinedActivationRule_ReturnsFalse()
        {
            var payload = DefaultTriggerPayloadBuilder.Create().WithActivationRule(TriggerActivationRule.Undefined).
                WithDeactivationRule(TriggerDeactivationRule.Never).Build();

            Assert.That(payload.IsValid(), Is.False);
        }

        [Test]
        public void IsValid_UndefinedDeactivationRule_ReturnsFalse()
        {
            var payload = DefaultTriggerPayloadBuilder.Create().WithActivationRule(TriggerActivationRule.Immediately).
                WithDeactivationRule(TriggerDeactivationRule.Undefined).Build();

            Assert.That(payload.IsValid(), Is.False);
        }

        #endregion
    }
}