using System.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Registry;
using Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Common.Builders;
using Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Common.Mocks;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Unit.Core
{
    /// <summary>
    /// Unit tests for StateMachineContainerEntry and related registry components.
    /// Tests trigger and state machine registration.
    /// </summary>
    [TestFixture]
    public sealed class RegistryTests
    {
        private StateMachineContainerEntry _entry;

        [SetUp]
        public void SetUp()
        {
            _entry = new StateMachineContainerEntry("test-container");
        }

        #region Constructor Tests

        [Test]
        public void Constructor_SetsId()
        {
            // Assert
            Assert.That(_entry.Id, Is.EqualTo("test-container"));
        }

        [Test]
        public void Constructor_InitializesEmptyCollections()
        {
            // Assert
            Assert.That(_entry.Triggers, Is.Not.Null);
            Assert.That(_entry.StateMachines, Is.Not.Null);
            Assert.That(_entry.Triggers.Count, Is.EqualTo(0));
            Assert.That(_entry.StateMachines.Count, Is.EqualTo(0));
        }

        #endregion

        #region TryRegisterTrigger Tests

        [Test]
        public void TryRegisterTrigger_WithValidTrigger_AddsToTriggers()
        {
            // Arrange
            var trigger = new MockTrigger("trigger-1", "Test Trigger", "Description");

            // Act
            var result = _entry.TryRegisterTrigger(trigger);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_entry.Triggers.ContainsKey("trigger-1"), Is.True);
            Assert.That(_entry.Triggers["trigger-1"], Is.SameAs(trigger));
        }

        [Test]
        public void TryRegisterTrigger_WithEmptyId_ReturnsFalse()
        {
            // Arrange
            LogAssert.ignoreFailingMessages = true;
            var trigger = new MockTrigger("", "Test Trigger", "Description");

            // Act
            var result = _entry.TryRegisterTrigger(trigger);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_entry.Triggers.Count, Is.EqualTo(0));
        }

        [Test]
        public void TryRegisterTrigger_WithWhitespaceId_ReturnsFalse()
        {
            // Arrange
            LogAssert.ignoreFailingMessages = true;
            var trigger = new MockTrigger("   ", "Test Trigger", "Description");

            // Act
            var result = _entry.TryRegisterTrigger(trigger);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void TryRegisterTrigger_WithDuplicateId_ReturnsFalse()
        {
            // Arrange
            var trigger1 = new MockTrigger("trigger-1", "Trigger 1", "Description");
            var trigger2 = new MockTrigger("trigger-1", "Trigger 2", "Description");
            _entry.TryRegisterTrigger(trigger1);

            // Act
            var result = _entry.TryRegisterTrigger(trigger2);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_entry.Triggers.Count, Is.EqualTo(1));
            Assert.That(_entry.Triggers["trigger-1"], Is.SameAs(trigger1));
        }

        [Test]
        public void TryRegisterTrigger_MultipleTriggers_AddsAll()
        {
            // Arrange
            var trigger1 = new MockTrigger("trigger-1", "Trigger 1", "Description");
            var trigger2 = new MockTrigger("trigger-2", "Trigger 2", "Description");
            var trigger3 = new MockTrigger("trigger-3", "Trigger 3", "Description");

            // Act
            _entry.TryRegisterTrigger(trigger1);
            _entry.TryRegisterTrigger(trigger2);
            _entry.TryRegisterTrigger(trigger3);

            // Assert
            Assert.That(_entry.Triggers.Count, Is.EqualTo(3));
        }

        #endregion

        #region TryRegisterStateMachine Tests

        [Test]
        public async Task TryRegisterStateMachine_WithValidMachine_AddsToStateMachines()
        {
            // Arrange
            var stateMachine = new MockStateMachine();
            var stateDefinition = new StateMachineDefinitionBuilder().WithId("sm-1").WithName("Test SM").Build();

            var settings = new StateMachineSettingsBuilder().WithStateMachineDefinition(stateDefinition).
                WithTransitionSolver(new MockTransitionSolver()).Build();

            await stateMachine.InitializeAsync(settings, default);

            // Act
            var result = _entry.TryRegisterStateMachine(stateMachine);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_entry.StateMachines.ContainsKey("sm-1"), Is.True);
        }

        [Test]
        public async Task TryRegisterStateMachine_WithDuplicateId_ReturnsFalse()
        {
            // Arrange
            var stateMachine1 = new MockStateMachine();
            var stateMachine2 = new MockStateMachine();

            var stateDefinition = new StateMachineDefinitionBuilder().WithId("sm-1").Build();

            var settings = new StateMachineSettingsBuilder().WithStateMachineDefinition(stateDefinition).
                WithTransitionSolver(new MockTransitionSolver()).Build();

            await stateMachine1.InitializeAsync(settings, default);
            await stateMachine2.InitializeAsync(settings, default);

            _entry.TryRegisterStateMachine(stateMachine1);

            // Act
            var result = _entry.TryRegisterStateMachine(stateMachine2);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_entry.StateMachines.Count, Is.EqualTo(1));
        }

        #endregion
    }
}