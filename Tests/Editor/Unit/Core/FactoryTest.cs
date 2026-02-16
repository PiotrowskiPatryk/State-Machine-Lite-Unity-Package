#region

using System;
using System.Threading;
using System.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Data;
using Dev.Cortez.StateMachines.Core.Factories;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Core.StateMachineConfiguration.Definition;
using Dev.Cortez.StateMachines.Core.TransitionSolver;
using Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Common.Builders;
using Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Common.Mocks;
using NUnit.Framework;
using UnityEngine.TestTools;

#endregion

namespace Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Unit.Core
{
    /// <summary>
    ///     Unit tests for StateMachineFactory.
    ///     Tests creation of state machines, states, triggers, and conditions including error paths.
    /// </summary>
    [TestFixture]
    public sealed class FactoryTest
    {
        private readonly IStateMachineFactory _stateMachineFactory = StateMachineFactory.Default();
        private CancellationTokenSource _cts;

        [SetUp]
        public void SetUp()
        {
            _cts = new CancellationTokenSource();
        }

        [TearDown]
        public void TearDown()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }

        #region CreateStateAsync Tests

        [Test]
        public async Task CreateStateAsync_WithValidDefinition_CreatesAndInitializes()
        {
            // Arrange
            var stateDefinition = StateDefinitionBuilder.Create().WithId("state-1").WithName("Test State").
                WithType<MockState>().Build();

            // Act
            var state = await _stateMachineFactory.CreateStateAsync(stateDefinition, _cts.Token);

            // Assert
            Assert.That(state, Is.Not.Null);
            Assert.That(state.Id, Is.EqualTo("state-1"));
            Assert.That(state.Name, Is.EqualTo("Test State"));
        }

        [Test]
        public async Task CreateStateAsync_WithInvalidTypeName_ReturnsNull()
        {
            // Arrange
            LogAssert.ignoreFailingMessages = true;

            var stateDefinition = StateDefinitionBuilder.Create().WithId("state-1").
                WithTypeName("Invalid.Type.Name, InvalidAssembly").Build();

            // Act
            var state = await _stateMachineFactory.CreateStateAsync(stateDefinition, _cts.Token);

            // Assert
            Assert.That(state, Is.Null);
        }

        [Test]
        public async Task CreateStateAsync_WithNullTypeName_ReturnsNull()
        {
            // Arrange
            LogAssert.ignoreFailingMessages = true;

            var stateDefinition = StateDefinitionBuilder.Create().WithId("state-1").WithTypeName(null).Build();

            // Act
            var state = await _stateMachineFactory.CreateStateAsync(stateDefinition, _cts.Token);

            // Assert
            Assert.That(state, Is.Null);
        }

        #endregion

        #region CreateTriggerAsync Tests

        [Test]
        public async Task CreateTriggerAsync_WithValidDefinition_CreatesTrigger()
        {
            // Arrange
            var triggerDefinition = new TriggerDefinition(
                "trigger-1",
                "Test Trigger",
                "A test trigger",
                typeof(MockTrigger).AssemblyQualifiedName,
                new EmptyPayload()
            );

            // Act
            var trigger = await _stateMachineFactory.CreateTriggerAsync(triggerDefinition, _cts.Token);

            // Assert
            Assert.That(trigger, Is.Not.Null);
            Assert.That(trigger.Id, Is.EqualTo("trigger-1"));
            Assert.That(trigger.Name, Is.EqualTo("Test Trigger"));
        }

        [Test]
        public void CreateTriggerAsync_WithInvalidTypeName_ThrowsArgumentException()
        {
            // Arrange
            LogAssert.ignoreFailingMessages = true;

            var triggerDefinition = new TriggerDefinition(
                "trigger-1",
                "Test Trigger",
                "A test trigger",
                "Invalid.Type.Name, InvalidAssembly",
                new EmptyPayload()
            );

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _stateMachineFactory.CreateTriggerAsync(triggerDefinition, _cts.Token));
        }

        [Test]
        public void CreateTriggerAsync_WithNullId_ThrowsArgumentException()
        {
            // Arrange
            LogAssert.ignoreFailingMessages = true;

            var triggerDefinition = new TriggerDefinition(
                null,
                "Test Trigger",
                "A test trigger",
                typeof(MockTrigger).AssemblyQualifiedName,
                new EmptyPayload()
            );

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _stateMachineFactory.CreateTriggerAsync(triggerDefinition, _cts.Token));
        }

        #endregion

        #region CreateConditionAsync Tests

        [Test]
        public async Task CreateConditionAsync_WithValidDefinition_CreatesCondition()
        {
            // Arrange
            var conditionDefinition = new ConditionDefinition(
                "condition-1",
                "Test Condition",
                "A test condition",
                typeof(MockCondition).AssemblyQualifiedName,
                new EmptyPayload()
            );

            // Act
            var condition = await _stateMachineFactory.CreateConditionAsync(conditionDefinition, _cts.Token);

            // Assert
            Assert.That(condition, Is.Not.Null);
            Assert.That(condition, Is.InstanceOf<ICondition>());
        }

        [Test]
        public async Task CreateConditionAsync_WithInvalidTypeName_ReturnsNull()
        {
            // Arrange
            LogAssert.ignoreFailingMessages = true;

            var conditionDefinition = new ConditionDefinition(
                "condition-1",
                "Test Condition",
                "A test condition",
                "Invalid.Type.Name, InvalidAssembly",
                new EmptyPayload()
            );

            // Act
            var condition = await _stateMachineFactory.CreateConditionAsync(conditionDefinition, _cts.Token);

            // Assert
            Assert.That(condition, Is.Null);
        }

        [Test]
        public async Task CreateConditionAsync_WithNonConditionType_ReturnsNull()
        {
            LogAssert.ignoreFailingMessages = true;

            // Arrange - Using a type that doesn't implement ICondition
            var conditionDefinition = new ConditionDefinition(
                "condition-1",
                "Test Condition",
                "A test condition",
                typeof(string).AssemblyQualifiedName,
                new EmptyPayload()
            );

            // Act
            var condition = await _stateMachineFactory.CreateConditionAsync(conditionDefinition, _cts.Token);

            // Assert
            Assert.That(condition, Is.Null);
        }

        #endregion

        #region CreateStateMachineAsync Tests

        [Test]
        public async Task CreateStateMachineAsync_WithValidDefinition_CreatesAndInitializes()
        {
            // Arrange
            var initialStateDefinition = StateDefinitionBuilder.Create().WithId("initial-state").
                WithName("Initial State").WithType<MockState>().Build();

            var stateMachineDefinition = StateMachineDefinitionBuilder.Create().WithId("sm-1").
                WithName("Test State Machine").WithType<MockStateMachine>().
                WithTransitionSolverType<DefaultTransitionSolver>().WithInitialState(initialStateDefinition).Build();

            // Act
            var stateMachine = await _stateMachineFactory.CreateStateMachineAsync(stateMachineDefinition, _cts.Token);

            // Assert
            Assert.That(stateMachine, Is.Not.Null);
            Assert.That(stateMachine.Id, Is.EqualTo("sm-1"));
            Assert.That(stateMachine.Name, Is.EqualTo("Test State Machine"));
        }

        [Test]
        public async Task CreateStateMachineAsync_WithInvalidTypeName_ReturnsNull()
        {
            // Arrange
            LogAssert.ignoreFailingMessages = true;

            var initialStateDefinition =
                StateDefinitionBuilder.Create().WithId("initial-state").WithType<MockState>().Build();

            var stateMachineDefinition = StateMachineDefinitionBuilder.Create().WithId("sm-1").
                WithTypeName("Invalid.Type.Name, InvalidAssembly").WithInitialState(initialStateDefinition).Build();

            // Act
            var stateMachine = await _stateMachineFactory.CreateStateMachineAsync(stateMachineDefinition, _cts.Token);

            // Assert
            Assert.That(stateMachine, Is.Null);
        }

        [Test]
        public async Task CreateStateMachineAsync_WithNullInitialState_ReturnsNull()
        {
            // Arrange
            LogAssert.ignoreFailingMessages = true;

            var stateMachineDefinition = StateMachineDefinitionBuilder.Create().WithId("sm-1").
                WithType<MockStateMachine>().Build(); // No initial state

            // Act
            var stateMachine = await _stateMachineFactory.CreateStateMachineAsync(stateMachineDefinition, _cts.Token);

            // Assert
            Assert.That(stateMachine, Is.Null);
        }

        [Test]
        public async Task CreateStateMachineAsync_WithNonStateMachineType_ReturnsNull()
        {
            // Arrange
            LogAssert.ignoreFailingMessages = true;

            var initialStateDefinition =
                StateDefinitionBuilder.Create().WithId("initial-state").WithType<MockState>().Build();

            var stateMachineDefinition = StateMachineDefinitionBuilder.Create().WithId("sm-1").
                WithTypeName(typeof(string).AssemblyQualifiedName) // Not IStateMachine
                .WithInitialState(initialStateDefinition).Build();

            // Act
            var stateMachine = await _stateMachineFactory.CreateStateMachineAsync(stateMachineDefinition, _cts.Token);

            // Assert
            Assert.That(stateMachine, Is.Null);
        }

        [Test]
        public async Task CreateStateMachineAsync_WithInvalidTransitionSolverType_ReturnsNull()
        {
            // Arrange
            LogAssert.ignoreFailingMessages = true;

            var initialStateDefinition =
                StateDefinitionBuilder.Create().WithId("initial-state").WithType<MockState>().Build();

            var stateMachineDefinition = StateMachineDefinitionBuilder.Create().WithId("sm-1").
                WithType<MockStateMachine>().WithTransitionSolverTypeName("Invalid.Solver.Type, InvalidAssembly").
                WithInitialState(initialStateDefinition).Build();

            // Act
            var stateMachine = await _stateMachineFactory.CreateStateMachineAsync(stateMachineDefinition, _cts.Token);

            // Assert
            Assert.That(stateMachine, Is.Null);
        }

        #endregion
    }
}