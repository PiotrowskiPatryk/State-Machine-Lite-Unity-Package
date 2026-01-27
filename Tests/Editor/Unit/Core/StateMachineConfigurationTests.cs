using Dev.Cortez.StateMachines.Core.Data;
using Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Common.Builders;
using Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Common.Mocks;
using NUnit.Framework;

namespace Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Unit.Core
{
    /// <summary>
    /// Unit tests for configuration data classes.
    /// Tests TransitionRule and StateMachineSettings construction.
    /// </summary>
    [TestFixture]
    public sealed class StateMachineConfigurationTests
    {
        #region TransitionRuleBuilder Tests

        [Test]
        public void TransitionRuleBuilder_Build_CreatesValidRule()
        {
            // Arrange
            var currentState = new MockState();
            var targetState = new MockState();
            var condition = new MockCondition();

            // Act
            var rule = TransitionRuleBuilder.Create().From(currentState).To(targetState).WithCondition(condition).
                WithPriority(5).Build();

            // Assert
            Assert.That(rule.CurrentState, Is.SameAs(currentState));
            Assert.That(rule.TargetState, Is.SameAs(targetState));
            Assert.That(rule.Condition, Is.SameAs(condition));
            Assert.That(rule.Priority, Is.EqualTo(5));
        }

        #endregion

        #region StateMachineSettingsBuilder Tests

        [Test]
        public void StateMachineSettingsBuilder_Build_CreatesValidSettings()
        {
            // Arrange
            var stateMachineDefinition = StateMachineDefinitionBuilder.Create().WithId("sm-1").Build();
            var transitionSolver = new MockTransitionSolver();
            var initialState = new MockState();
            var secondState = new MockState();

            // Act
            var settings = StateMachineSettingsBuilder.Create().WithStateMachineDefinition(stateMachineDefinition).
                WithTransitionSolver(transitionSolver).WithInitialState(initialState).WithState(initialState).
                WithState(secondState).Build();

            // Assert
            Assert.That(settings.StateMachineDefinition, Is.SameAs(stateMachineDefinition));
            Assert.That(settings.TransitionSolver, Is.SameAs(transitionSolver));
            Assert.That(settings.InitialState, Is.SameAs(initialState));
            Assert.That(settings.States.Count, Is.EqualTo(2));
        }

        #endregion

        #region InitializationStatus Tests

        [Test]
        public void InitializationStatus_HasExpectedValues()
        {
            // Assert
            Assert.That((int)InitializationStatus.Undefined, Is.EqualTo(0));
            Assert.That((int)InitializationStatus.NotInitialized, Is.EqualTo(1));
            Assert.That((int)InitializationStatus.Initializing, Is.EqualTo(2));
            Assert.That((int)InitializationStatus.Initialized, Is.EqualTo(3));
            Assert.That((int)InitializationStatus.Failed, Is.EqualTo(4));
        }

        #endregion

        #region StateStatus Tests

        [Test]
        public void StateStatus_HasExpectedValues()
        {
            // Assert
            Assert.That((int)StateStatus.Undefined, Is.EqualTo(0));
            Assert.That((int)StateStatus.Activating, Is.EqualTo(1));
            Assert.That((int)StateStatus.Active, Is.EqualTo(2));
            Assert.That((int)StateStatus.Deactivating, Is.EqualTo(3));
            Assert.That((int)StateStatus.Inactive, Is.EqualTo(4));
        }

        #endregion

        #region TransitionRule Tests

        [Test]
        public void TransitionRule_Constructor_SetsAllProperties()
        {
            // Arrange
            var currentState = new MockState();
            var targetState = new MockState();
            var condition = new MockCondition();
            var priority = 10;

            // Act
            var rule = new TransitionRule(currentState, targetState, condition, priority);

            // Assert
            Assert.That(rule.CurrentState, Is.SameAs(currentState));
            Assert.That(rule.TargetState, Is.SameAs(targetState));
            Assert.That(rule.Condition, Is.SameAs(condition));
            Assert.That(rule.Priority, Is.EqualTo(10));
        }

        [Test]
        public void TransitionRule_Constructor_WithDefaultPriority_SetsZero()
        {
            // Arrange
            var currentState = new MockState();
            var targetState = new MockState();
            var condition = new MockCondition();

            // Act
            var rule = new TransitionRule(currentState, targetState, condition);

            // Assert
            Assert.That(rule.Priority, Is.EqualTo(0));
        }

        #endregion

        #region StateDefinitionBuilder Tests

        [Test]
        public void StateDefinitionBuilder_Build_CreatesValidDefinition()
        {
            // Act
            var emptyPayload = new EmptyPayload();
            var definition = StateDefinitionBuilder.Create().WithId("state-1").WithName("Test State").
                WithDescription("A test state description").WithType<MockState>().WithPayload(emptyPayload).
                Build();

            // Assert
            Assert.That(definition.Id, Is.EqualTo("state-1"));
            Assert.That(definition.Name, Is.EqualTo("Test State"));
            Assert.That(definition.Description, Is.EqualTo("A test state description"));
            Assert.That(definition.TypeName, Does.Contain("MockState"));
            Assert.That(definition.Payload, Is.EqualTo(emptyPayload));
        }

        [Test]
        public void StateDefinitionBuilder_CreateForType_SetsTypeName()
        {
            // Act
            var definition = StateDefinitionBuilder.CreateForType<MockState>().WithId("state-1").Build();

            // Assert
            Assert.That(definition.TypeName, Does.Contain("MockState"));
        }

        #endregion

        #region StateMachineDefinitionBuilder Tests

        [Test]
        public void StateMachineDefinitionBuilder_Build_CreatesValidDefinition()
        {
            // Arrange
            var stateDefinition = StateDefinitionBuilder.Create().WithId("state-1").WithType<MockState>().Build();

            // Act
            var definition = StateMachineDefinitionBuilder.Create().WithId("sm-1").WithName("Test SM").
                WithDescription("A test state machine").WithType<MockStateMachine>().WithInitialState(stateDefinition).
                Build();

            // Assert
            Assert.That(definition.Id, Is.EqualTo("sm-1"));
            Assert.That(definition.Name, Is.EqualTo("Test SM"));
            Assert.That(definition.Description, Is.EqualTo("A test state machine"));
            Assert.That(definition.StateMachineTypeName, Does.Contain("MockStateMachine"));
            Assert.That(definition.InitialState, Is.SameAs(stateDefinition));
            Assert.That(definition.States, Does.Contain(stateDefinition));
        }

        [Test]
        public void StateMachineDefinitionBuilder_WithInitialState_AddsToStatesCollection()
        {
            // Arrange
            var stateDefinition = StateDefinitionBuilder.Create().WithId("state-1").WithType<MockState>().Build();

            // Act
            var definition = StateMachineDefinitionBuilder.Create().WithId("sm-1").WithType<MockStateMachine>().
                WithInitialState(stateDefinition).Build();

            // Assert
            Assert.That(definition.States.Count, Is.EqualTo(1));
            Assert.That(definition.States[0], Is.SameAs(stateDefinition));
        }

        #endregion
    }
}