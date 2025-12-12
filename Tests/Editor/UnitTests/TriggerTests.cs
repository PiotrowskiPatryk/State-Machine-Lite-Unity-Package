using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Trigger;
using UnityEngine.Assertions;
using UnityEngine.TestTools;

namespace Dev.Cortez.StateMachines.EditorTests.Tests.Editor.UnitTests
{
    public class TriggerTests
    {
        [UnityTest]
        public IEnumerator OneFrameTrigger_IsTriggeredProperly()
        {
            // Arrange
            var trigger = new OneFrameTrigger(Guid.NewGuid().ToString(), "Trigger", "Description");
            var wasTriggered = false;

            // Act
            yield return trigger.TriggerValueAsync(true, CancellationToken.None).
                ToCoroutine(output => wasTriggered = output);

            // Assert
            Assert.IsTrue(wasTriggered);
            Assert.IsTrue(trigger.IsTriggered);

            // Cleanup
            yield return trigger.DisposeAsync().AsUniTask().ToCoroutine();
        }

        [UnityTest]
        public IEnumerator OneFrameTrigger_IsTriggeredOnlyForOneFrame()
        {
            // Arrange
            var trigger = new OneFrameTrigger(Guid.NewGuid().ToString(), "Trigger", "Description");
            var triggeredFalseValue = false;

            trigger.TriggeredValueChanged += (_, value) =>
            {
                if (!value)
                {
                    triggeredFalseValue = true;
                }
            };

            // Act
            yield return trigger.TriggerValueAsync(true, CancellationToken.None).ToCoroutine();
            yield return null;

            // Assert
            Assert.IsFalse(trigger.IsTriggered);
            Assert.IsTrue(triggeredFalseValue);
        }
    }
}