using System.Threading;
using System.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Data;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Common.Builders;
using Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Common.Mocks;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Unit.Core
{
    /// <summary>
    /// Unit tests for StateMachineBase class lifecycle and behavior.
    /// Tests initialization, activation/deactivation, state transitions, and disposal.
    /// </summary>
    [TestFixture]
    public sealed class StateMachineTests
    {
        private MockStateMachine _stateMachine;
        private MockTransitionSolver _transitionSolver;
        private MockState _initialState;
        private MockState _secondState;
        private CancellationTokenSource _cts;

        [SetUp]
        public void SetUp()
        {
            _stateMachine = new MockStateMachine();
            _transitionSolver = new MockTransitionSolver();
            _initialState = new MockState();
            _secondState = new MockState();
            _cts = new CancellationTokenSource();
        }

        [TearDown]
        public void TearDown()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _stateMachine?.DisposeAsync().AsTask().GetAwaiter().GetResult();
        }

        #region DisposeAsync Tests

        [Test]
        public async Task DisposeAsync_CleansUpAllResources()
        {
            // Arrange
            await CreateAndInitializeStateMachineAsync();
            await _stateMachine.ActivateAsync(_cts.Token);

            // Act
            await _stateMachine.DisposeAsync();

            // Assert
            Assert.That(_stateMachine.DoDisposeCallCount, Is.EqualTo(1));
            Assert.That(_stateMachine.IsActive, Is.False);
            Assert.That(_stateMachine.InitializationStatus, Is.EqualTo(InitializationStatus.NotInitialized));
        }

        #endregion

        #region Cancellation Tests

        [Test]
        public async Task ActivateAsync_WhenCancelled_ThrowsOperationCanceledException()
        {
            // Arrange
            await CreateAndInitializeStateMachineAsync();
            _cts.Cancel();

            // Act & Assert
            Assert.ThrowsAsync<TaskCanceledException>(async () =>
                await _stateMachine.ActivateAsync(_cts.Token));
        }

        #endregion

        #region TransitionRules Property Tests

        [Test]
        public async Task TransitionRules_ReturnsEmptyListWhenNoRules()
        {
            // Arrange
            await CreateAndInitializeStateMachineAsync();

            // Act
            var rules = ((IStateMachine)_stateMachine).TransitionRules;

            // Assert
            Assert.That(rules, Is.Not.Null);
            Assert.That(rules.Count, Is.EqualTo(0));
        }

        #endregion

        private async Task<StateMachineSettings> CreateAndInitializeStateMachineAsync()
        {
            var initialStateDefinition = StateDefinitionBuilder.Create().WithId("initial-state").
                WithName("Initial State").WithType<MockState>().Build();

            var secondStateDefinition = StateDefinitionBuilder.Create().WithId("second-state").WithName("Second State").
                WithType<MockState>().Build();

            await _initialState.InitializeAsync(initialStateDefinition, _cts.Token);
            await _secondState.InitializeAsync(secondStateDefinition, _cts.Token);

            var stateMachineDefinition = StateMachineDefinitionBuilder.Create().WithId("test-sm").
                WithName("Test State Machine").WithType<MockStateMachine>().WithInitialState(initialStateDefinition).
                WithState(secondStateDefinition).Build();

            var settings = StateMachineSettingsBuilder.Create().WithStateMachineDefinition(stateMachineDefinition).
                WithTransitionSolver(_transitionSolver).WithInitialState(_initialState).WithState(_initialState).
                WithState(_secondState).Build();

            await _stateMachine.InitializeAsync(settings, _cts.Token);

            return settings;
        }

        #region InitializeAsync Tests

        [Test]
        public async Task InitializeAsync_WithValidSettings_SetsStatusToInitialized()
        {
            // Arrange & Act
            await CreateAndInitializeStateMachineAsync();

            // Assert
            Assert.That(_stateMachine.InitializationStatus, Is.EqualTo(InitializationStatus.Initialized));
        }

        [Test]
        public async Task InitializeAsync_AppliesVariablesFromSettings()
        {
            // Arrange & Act
            await CreateAndInitializeStateMachineAsync();

            // Assert
            Assert.That(_stateMachine.Id, Is.EqualTo("test-sm"));
            Assert.That(_stateMachine.Name, Is.EqualTo("Test State Machine"));
        }

        [Test]
        public async Task InitializeAsync_InitializesTransitionSolver()
        {
            // Arrange & Act
            await CreateAndInitializeStateMachineAsync();

            // Assert
            Assert.That(_transitionSolver.ApplyTransitionRulesCallCount, Is.EqualTo(1));
        }

        [Test]
        public async Task InitializeAsync_PopulatesStatesCollection()
        {
            // Arrange & Act
            await CreateAndInitializeStateMachineAsync();

            // Assert
            Assert.That(_stateMachine.States, Is.Not.Null);
            Assert.That(_stateMachine.States.Count, Is.EqualTo(2));
        }

        #endregion

        #region ActivateAsync Tests

        [Test]
        public async Task ActivateAsync_WhenNotActive_ActivatesAndEntersDefaultState()
        {
            // Arrange
            await CreateAndInitializeStateMachineAsync();

            // Act
            var result = await _stateMachine.ActivateAsync(_cts.Token);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_stateMachine.IsActive, Is.True);
            Assert.That(_stateMachine.ActiveState, Is.EqualTo(_initialState));
            Assert.That(_initialState.IsActive, Is.True);
        }

        [Test]
        public async Task ActivateAsync_WhenAlreadyActive_ReturnsFalse()
        {
            // Arrange
            await CreateAndInitializeStateMachineAsync();
            await _stateMachine.ActivateAsync(_cts.Token);

            // Act
            var result = await _stateMachine.ActivateAsync(_cts.Token);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_stateMachine.DoActivateCallCount, Is.EqualTo(1)); // Should not re-activate
        }

        [Test]
        public async Task ActivateAsync_WhenCanActivateIsFalse_ReturnsFalse()
        {
            // Arrange
            LogAssert.ignoreFailingMessages = true;
            await CreateAndInitializeStateMachineAsync();
            _stateMachine.ConfigurableCanActivate = false;

            // Act
            var result = await _stateMachine.ActivateAsync(_cts.Token);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_stateMachine.IsActive, Is.False);
        }

        [Test]
        public async Task ActivateAsync_CallsDoActivateAsync()
        {
            // Arrange
            await CreateAndInitializeStateMachineAsync();

            // Act
            await _stateMachine.ActivateAsync(_cts.Token);

            // Assert
            Assert.That(_stateMachine.DoActivateCallCount, Is.EqualTo(1));
        }

        [Test]
        public async Task ActivateAsync_WhenDoActivateFails_ReturnsFalse()
        {
            // Arrange
            LogAssert.ignoreFailingMessages = true;
            await CreateAndInitializeStateMachineAsync();
            _stateMachine.DoActivateShouldSucceed = false;

            // Act
            var result = await _stateMachine.ActivateAsync(_cts.Token);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_stateMachine.IsActive, Is.False);
        }

        [Test]
        public async Task ActivateAsync_AppliesTransitionRulesForInitialState()
        {
            // Arrange
            await CreateAndInitializeStateMachineAsync();

            // Act
            await _stateMachine.ActivateAsync(_cts.Token);

            // Assert
            Assert.That(_transitionSolver.ApplyRulesForActiveStateCalls.Count, Is.EqualTo(1));
            Assert.That(_transitionSolver.ApplyRulesForActiveStateCalls[0], Is.EqualTo(_initialState));
        }

        #endregion

        #region DeactivateAsync Tests

        [Test]
        public async Task DeactivateAsync_WhenActive_DeactivatesSuccessfully()
        {
            // Arrange
            await CreateAndInitializeStateMachineAsync();
            await _stateMachine.ActivateAsync(_cts.Token);

            // Act
            var result = await _stateMachine.DeactivateAsync(_cts.Token);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_stateMachine.IsActive, Is.False);
        }

        [Test]
        public async Task DeactivateAsync_WhenNotActive_ReturnsFalse()
        {
            // Arrange
            await CreateAndInitializeStateMachineAsync();

            // Act
            var result = await _stateMachine.DeactivateAsync(_cts.Token);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task DeactivateAsync_ExitsCurrentState()
        {
            // Arrange
            await CreateAndInitializeStateMachineAsync();
            await _stateMachine.ActivateAsync(_cts.Token);

            // Act
            await _stateMachine.DeactivateAsync(_cts.Token);

            // Assert
            Assert.That(_initialState.IsActive, Is.False);
            Assert.That(_initialState.ExitCallCount, Is.EqualTo(1));
        }

        [Test]
        public async Task DeactivateAsync_CallsDoDeactivateAsync()
        {
            // Arrange
            await CreateAndInitializeStateMachineAsync();
            await _stateMachine.ActivateAsync(_cts.Token);

            // Act
            await _stateMachine.DeactivateAsync(_cts.Token);

            // Assert
            Assert.That(_stateMachine.DoDeactivateCallCount, Is.EqualTo(1));
        }

        [Test]
        public async Task DeactivateAsync_WhenDoDeactivateFails_ReturnsFalse()
        {
            // Arrange
            await CreateAndInitializeStateMachineAsync();
            await _stateMachine.ActivateAsync(_cts.Token);
            _stateMachine.DoDeactivateShouldSucceed = false;

            // Act
            var result = await _stateMachine.DeactivateAsync(_cts.Token);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region MoveToStateAsync Tests

        [Test]
        public async Task MoveToStateAsync_TransitionsFromCurrentToTarget()
        {
            // Arrange
            await CreateAndInitializeStateMachineAsync();
            await _stateMachine.ActivateAsync(_cts.Token);

            // Act
            var result = await _stateMachine.MoveToStateAsync(_secondState, _cts.Token);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_stateMachine.ActiveState, Is.EqualTo(_secondState));
            Assert.That(_initialState.IsActive, Is.False);
            Assert.That(_secondState.IsActive, Is.True);
        }

        [Test]
        public async Task MoveToStateAsync_ExitsPreviousState()
        {
            // Arrange
            await CreateAndInitializeStateMachineAsync();
            await _stateMachine.ActivateAsync(_cts.Token);

            // Act
            await _stateMachine.MoveToStateAsync(_secondState, _cts.Token);

            // Assert
            Assert.That(_initialState.ExitCallCount, Is.EqualTo(1));
        }

        [Test]
        public async Task MoveToStateAsync_EntersTargetState()
        {
            // Arrange
            await CreateAndInitializeStateMachineAsync();
            await _stateMachine.ActivateAsync(_cts.Token);

            // Act
            await _stateMachine.MoveToStateAsync(_secondState, _cts.Token);

            // Assert
            Assert.That(_secondState.EnterCallCount, Is.EqualTo(1));
        }

        [Test]
        public async Task MoveToStateAsync_AppliesTransitionRulesForNewState()
        {
            // Arrange
            await CreateAndInitializeStateMachineAsync();
            await _stateMachine.ActivateAsync(_cts.Token);
            _transitionSolver.ApplyRulesForActiveStateCalls.Clear();

            // Act
            await _stateMachine.MoveToStateAsync(_secondState, _cts.Token);

            // Assert
            Assert.That(_transitionSolver.ApplyRulesForActiveStateCalls.Count, Is.EqualTo(1));
            Assert.That(_transitionSolver.ApplyRulesForActiveStateCalls[0], Is.EqualTo(_secondState));
        }

        #endregion
    }
}