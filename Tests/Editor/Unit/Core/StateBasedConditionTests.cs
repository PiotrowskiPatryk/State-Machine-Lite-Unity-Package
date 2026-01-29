using System.Threading;
using System.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Condition;
using Dev.Cortez.StateMachines.Core.Condition.Payload;
using Dev.Cortez.StateMachines.Core.Data;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Core.ReferencePicker;
using Dev.Cortez.StateMachines.Core.ReferencePicker.State;
using Dev.Cortez.StateMachines.Core.Registry;
using Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Common.Builders;
using Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Common.Mocks;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Unit.Core
{
    /// <summary>
    /// Unit tests for StateBasedCondition.
    /// Tests initialization, cleanup, and condition satisfaction logic using the Registry singleton.
    /// </summary>
    [TestFixture]
    public sealed class StateBasedConditionTests
    {
        private MockState _mockState;
        private MockStateMachine _mockStateMachine;
        private StateMachineContainerEntry _containerEntry;
        private CancellationTokenSource _cts;

        [SetUp]
        public async Task SetUp()
        {
            _cts = new CancellationTokenSource();
            _mockState = new MockState();

            // Initialize state with definition so it has an ID
            var stateDefinition = StateDefinitionBuilder.Create().WithId("test-state-1").WithName("Test State").
                WithType<MockState>().Build();
            await _mockState.InitializeAsync(stateDefinition, _cts.Token);

            // Create state machine with the state
            _mockStateMachine = new MockStateMachine();
            var smDefinition = StateMachineDefinitionBuilder.Create().WithId("test-sm-1").WithName("Test SM").
                WithType<MockStateMachine>().WithInitialState(stateDefinition).Build();

            var settings = StateMachineSettingsBuilder.Create().WithStateMachineDefinition(smDefinition).
                WithTransitionSolver(new MockTransitionSolver()).WithInitialState(_mockState).WithState(_mockState).
                Build();

            await _mockStateMachine.InitializeAsync(settings, _cts.Token);

            // Register in container
            _containerEntry = new StateMachineContainerEntry("test-container-sbc");
            _containerEntry.TryRegisterStateMachine(_mockStateMachine);
            StateMachineContainerRegistry.Instance.RegisterStateMachineContainer(_containerEntry);
        }

        [TearDown]
        public void TearDown()
        {
            StateMachineContainerRegistry.Instance.UnregisterStateMachineContainer(_containerEntry);
            _cts?.Cancel();
            _cts?.Dispose();
        }

        #region Event Tests

        [Test]
        public async Task StateStatusChanged_FiresSatisfiedChanged()
        {
            // Arrange
            var condition = new StateBasedCondition();
            var payload = new StateBasedConditionPayload(
                new StateReferencePicker("test-state-1"),
                StateSatisfiedConditionType.WhenActivated);

            await condition.InitializeAsync(payload, CancellationToken.None);

            var eventFired = false;
            ICondition eventCondition = null;
            condition.SatisfiedChanged += (cond, _) =>
            {
                eventFired = true;
                eventCondition = cond;
            };

            // Act
            await _mockState.EnterAsync(new EmptyContext(), _cts.Token);

            // Assert
            Assert.That(eventFired, Is.True);
            Assert.That(eventCondition, Is.SameAs(condition));

            await condition.DisposeAsync();
        }

        #endregion

        #region Dispose Tests

        [Test]
        public async Task DisposeAsync_UnsubscribesFromState()
        {
            // Arrange
            var condition = new StateBasedCondition();
            var payload = new StateBasedConditionPayload(
                new StateReferencePicker("test-state-1"),
                StateSatisfiedConditionType.WhenActivated);

            await condition.InitializeAsync(payload, CancellationToken.None);

            var eventCount = 0;
            condition.SatisfiedChanged += (_, __) => eventCount++;

            // Act
            await condition.DisposeAsync();
            await _mockState.EnterAsync(new EmptyContext(), _cts.Token);

            // Assert - Event should NOT fire after disposal
            Assert.That(eventCount, Is.EqualTo(0));
        }

        #endregion

        #region Initialization Tests

        [Test]
        public async Task InitializeAsync_WhenReferenceSelected_ReturnsTrue()
        {
            // Arrange
            var condition = new StateBasedCondition();
            var picker = new StateReferencePicker("test-state-1");
            var payload = new StateBasedConditionPayload(picker, StateSatisfiedConditionType.WhenActivated);

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
            var condition = new StateBasedCondition();
            var picker = new StateReferencePicker(); // No ID selected
            var payload = new StateBasedConditionPayload(picker, StateSatisfiedConditionType.WhenActivated);

            // Act
            var result = await condition.InitializeAsync(payload, CancellationToken.None);

            // Assert
            Assert.That(result, Is.False);

            await condition.DisposeAsync();
        }

        #endregion

        #region IsSatisfied Tests

        [Test]
        public async Task IsSatisfied_WhenActive_AndTypeIsWhenActivated_ReturnsTrue()
        {
            // Arrange
            var condition = new StateBasedCondition();
            var payload = new StateBasedConditionPayload(
                new StateReferencePicker("test-state-1"),
                StateSatisfiedConditionType.WhenActivated);

            await condition.InitializeAsync(payload, CancellationToken.None);

            // Act - Simulate state becoming active using EnterAsync with EmptyContext
            await _mockState.EnterAsync(new EmptyContext(), _cts.Token);

            // Assert
            Assert.That(condition.IsSatisfied, Is.True);

            await condition.DisposeAsync();
        }

        [Test]
        public async Task IsSatisfied_WhenInactive_AndTypeIsWhenDeactivated_ReturnsTrue()
        {
            // Arrange
            var condition = new StateBasedCondition();
            var payload = new StateBasedConditionPayload(
                new StateReferencePicker("test-state-1"),
                StateSatisfiedConditionType.WhenDeactivated);

            await condition.InitializeAsync(payload, CancellationToken.None);

            // State is inactive by default after initialization

            // Assert
            Assert.That(condition.IsSatisfied, Is.True);

            await condition.DisposeAsync();
        }

        [Test]
        public async Task IsSatisfied_WithCombinedFlags_MatchesAny()
        {
            // Arrange
            var condition = new StateBasedCondition();
            var payload = new StateBasedConditionPayload(
                new StateReferencePicker("test-state-1"),
                StateSatisfiedConditionType.WhenActivated | StateSatisfiedConditionType.WhenDeactivated);

            await condition.InitializeAsync(payload, CancellationToken.None);

            // Both states should satisfy the condition
            Assert.That(condition.IsSatisfied, Is.True); // Currently inactive

            await _mockState.EnterAsync(new EmptyContext(), _cts.Token);
            Assert.That(condition.IsSatisfied, Is.True); // Now active

            await condition.DisposeAsync();
        }

        #endregion
    }
}