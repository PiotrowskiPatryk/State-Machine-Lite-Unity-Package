using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Core.Registry;
using Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Common.Builders;
using Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Common.Mocks;
using NUnit.Framework;

namespace Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Unit.Core
{
    /// <summary>
    /// Unit tests for StateMachineContainerRegistry.
    /// Tests entry registration, subscription, and notification behavior.
    /// </summary>
    [TestFixture]
    public sealed class StateMachineContainerRegistryTests
    {
        // Note: We cannot test the singleton Instance directly as it would pollute global state.
        // Instead, we test the registration/unregistration through StateMachineContainerEntry.

        #region RegisterStateMachineContainer Tests

        [Test]
        public void RegisterStateMachineContainer_AddsEntryAndNotifiesObservers()
        {
            // Arrange
            var registry = StateMachineContainerRegistry.Instance;
            var entry = new StateMachineContainerEntry("test-container-1");
            var trigger = new MockTrigger("trigger-1", "Test Trigger", "Description");
            entry.TryRegisterTrigger(trigger);

            ITrigger receivedTrigger = null;
            var observer = new IdentifiableObserver<ITrigger>(
                "trigger-1",
                t => receivedTrigger = t,
                () => { });

            registry.SubscribeTrigger(observer);

            // Act
            registry.RegisterStateMachineContainer(entry);

            // Assert
            Assert.That(receivedTrigger, Is.SameAs(trigger));

            // Cleanup
            registry.UnsubscribeTrigger(observer);
            registry.UnregisterStateMachineContainer(entry);
        }

        [Test]
        public void RegisterStateMachineContainer_DuplicateEntry_DoesNotAddTwice()
        {
            // Arrange
            var registry = StateMachineContainerRegistry.Instance;
            var entry = new StateMachineContainerEntry("test-container-2");
            var trigger = new MockTrigger("trigger-2", "Test Trigger", "Description");
            entry.TryRegisterTrigger(trigger);

            var notificationCount = 0;
            var observer = new IdentifiableObserver<ITrigger>(
                "trigger-2",
                _ => notificationCount++,
                () => { });

            registry.SubscribeTrigger(observer);
            registry.RegisterStateMachineContainer(entry);
            notificationCount = 0; // Reset after first registration

            // Act - Register same entry again
            registry.RegisterStateMachineContainer(entry);

            // Assert
            Assert.That(notificationCount, Is.EqualTo(0)); // Should not notify again

            // Cleanup
            registry.UnsubscribeTrigger(observer);
            registry.UnregisterStateMachineContainer(entry);
        }

        #endregion

        #region UnregisterStateMachineContainer Tests

        [Test]
        public void UnregisterStateMachineContainer_RemovesEntryAndNotifiesUnregistered()
        {
            // Arrange
            var registry = StateMachineContainerRegistry.Instance;
            var entry = new StateMachineContainerEntry("test-container-3");
            var trigger = new MockTrigger("trigger-3", "Test Trigger", "Description");
            entry.TryRegisterTrigger(trigger);

            var unregisteredCalled = false;
            var observer = new IdentifiableObserver<ITrigger>(
                "trigger-3",
                _ => { },
                () => unregisteredCalled = true);

            registry.SubscribeTrigger(observer);
            registry.RegisterStateMachineContainer(entry);

            // Act
            registry.UnregisterStateMachineContainer(entry);

            // Assert
            Assert.That(unregisteredCalled, Is.True);

            // Cleanup
            registry.UnsubscribeTrigger(observer);
        }

        [Test]
        public void UnregisterStateMachineContainer_NonExistentEntry_DoesNotThrow()
        {
            // Arrange
            var registry = StateMachineContainerRegistry.Instance;
            var entry = new StateMachineContainerEntry("test-container-nonexistent");

            // Act & Assert
            Assert.DoesNotThrow(() => registry.UnregisterStateMachineContainer(entry));
        }

        #endregion

        #region SubscribeStateMachine Tests

        [Test]
        public void SubscribeStateMachine_WhenMachineExists_NotifiesObserver()
        {
            // Arrange
            var registry = StateMachineContainerRegistry.Instance;
            var entry = new StateMachineContainerEntry("test-container-4");
            var stateMachine = new MockStateMachine();

            // Initialize state machine with an ID
            var settings = new StateMachineSettingsBuilder()
                .WithStateMachineDefinition(new StateMachineDefinitionBuilder().WithId("sm-4").Build())
                .WithTransitionSolver(new MockTransitionSolver())
                .Build();
            stateMachine.InitializeAsync(settings, default).GetAwaiter().GetResult();

            entry.TryRegisterStateMachine(stateMachine);

            IStateMachine receivedMachine = null;
            var observer = new IdentifiableObserver<IStateMachine>(
                "sm-4",
                m => receivedMachine = m,
                () => { });

            // Act
            registry.RegisterStateMachineContainer(entry);
            registry.SubscribeStateMachine(observer);

            // Assert
            Assert.That(receivedMachine, Is.SameAs(stateMachine));

            // Cleanup
            registry.UnsubscribeStateMachine(observer);
            registry.UnregisterStateMachineContainer(entry);
        }

        #endregion

        #region SubscribeTrigger Tests

        [Test]
        public void SubscribeTrigger_WhenTriggerExists_NotifiesObserver()
        {
            // Arrange
            var registry = StateMachineContainerRegistry.Instance;
            var entry = new StateMachineContainerEntry("test-container-5");
            var trigger = new MockTrigger("trigger-5", "Test Trigger", "Description");
            entry.TryRegisterTrigger(trigger);

            registry.RegisterStateMachineContainer(entry);

            ITrigger receivedTrigger = null;
            var observer = new IdentifiableObserver<ITrigger>(
                "trigger-5",
                t => receivedTrigger = t,
                () => { });

            // Act
            registry.SubscribeTrigger(observer);

            // Assert
            Assert.That(receivedTrigger, Is.SameAs(trigger));

            // Cleanup
            registry.UnsubscribeTrigger(observer);
            registry.UnregisterStateMachineContainer(entry);
        }

        [Test]
        public void SubscribeTrigger_WhenTriggerNotExists_DoesNotNotify()
        {
            // Arrange
            var registry = StateMachineContainerRegistry.Instance;
            ITrigger receivedTrigger = null;
            var observer = new IdentifiableObserver<ITrigger>(
                "trigger-nonexistent",
                t => receivedTrigger = t,
                () => { });

            // Act
            registry.SubscribeTrigger(observer);

            // Assert
            Assert.That(receivedTrigger, Is.Null);

            // Cleanup
            registry.UnsubscribeTrigger(observer);
        }

        #endregion
    }
}
