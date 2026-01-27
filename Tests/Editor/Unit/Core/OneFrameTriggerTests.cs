using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Trigger;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Unit.Core
{
    /// <summary>
    /// Unit tests for OneFrameTrigger.
    /// Tests that the trigger sets IsTriggered to true for one frame/yield and then resets.
    /// </summary>
    [TestFixture]
    public sealed class OneFrameTriggerTests
    {
        #region TriggerValueAsync Tests

        [UnityTest]
        public IEnumerator TriggerValueAsync_WhenTargetTrue_IsTrueForOneFrame_ThenFalse()
        {
            // Arrange
            var trigger = new OneFrameTrigger("trigger-1", "Test Trigger", "Desc");
            using var cts = new CancellationTokenSource();
            var wasTrueAtSomePoint = false;

            // Act - Start the trigger but don't wait for completion yet
            var task = trigger.TriggerValueAsync(true, cts.Token);

            // Check if trigger is true during the frame (before yield completes)
            if (trigger.IsTriggered)
            {
                wasTrueAtSomePoint = true;
            }

            // Wait for the task to complete (which includes the yield)
            yield return task.ToCoroutine();

            // Assert - After yield, trigger should be reset to false
            Assert.That(wasTrueAtSomePoint, Is.True, "Trigger should have been true at some point");
            Assert.That(trigger.IsTriggered, Is.False, "Trigger should be false after yield");
        }

        [Test]
        public async Task TriggerValueAsync_WhenTargetTrue_SetsTriggeredAndResetsAfterYield()
        {
            // Arrange
            var trigger = new OneFrameTrigger("trigger-1", "Test Trigger", "Desc");
            using var cts = new CancellationTokenSource();

            // Act
            await trigger.TriggerValueAsync(true, cts.Token);

            // Assert - After yield, trigger should be reset to false
            Assert.That(trigger.IsTriggered, Is.False);
        }

        [Test]
        public async Task TriggerValueAsync_WhenTargetFalse_ResetsTrigger()
        {
            // Arrange
            var trigger = new OneFrameTrigger("trigger-1", "Test Trigger", "Desc");
            trigger.IsTriggered = true; // Manually set to true
            using var cts = new CancellationTokenSource();

            // Act
            await trigger.TriggerValueAsync(false, cts.Token);

            // Assert
            Assert.That(trigger.IsTriggered, Is.False);
        }

        [Test]
        public async Task TriggerValueAsync_WhenAlreadyTargetValue_ReturnsTrue()
        {
            // Arrange
            var trigger = new OneFrameTrigger("trigger-1", "Test Trigger", "Desc");
            trigger.IsTriggered = true;
            using var cts = new CancellationTokenSource();

            // Act
            var result = await trigger.TriggerValueAsync(true, cts.Token);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(trigger.IsTriggered, Is.True); // Should stay true (early return)
        }

        [Test]
        public async Task TriggerValueAsync_ReturnsTrue()
        {
            // Arrange
            var trigger = new OneFrameTrigger("trigger-1", "Test Trigger", "Desc");
            using var cts = new CancellationTokenSource();

            // Act
            var result = await trigger.TriggerValueAsync(true, cts.Token);

            // Assert
            Assert.That(result, Is.True);
        }

        #endregion
    }
}