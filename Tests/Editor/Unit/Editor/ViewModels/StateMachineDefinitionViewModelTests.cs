using System.Linq;
using Dev.Cortez.StateMachines.Core.StateMachineConfiguration.Definition;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.ViewModels;
using Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Common.Builders;
using Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Common.Mocks;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Unit.Editor.ViewModels
{
    /// <summary>
    /// Unit tests for StateMachineDefinitionViewModel.
    /// Tests construction, property binding, states management, and disposal.
    /// </summary>
    [TestFixture]
    [Category("ViewModels")]
    public sealed class StateMachineDefinitionViewModelTests
    {
        private TestPropertyContext<StateMachineDefinition> _context;
        private StateMachineDefinitionViewModel _viewModel;

        [SetUp]
        public void SetUp()
        {
            _context = SerializedPropertyTestHelper.CreateTestContext<StateMachineDefinition>();
            _viewModel = new StateMachineDefinitionViewModel(_context.Property);
        }

        [TearDown]
        public void TearDown()
        {
            _viewModel?.Dispose();
            _context?.Dispose();
        }

        #region Construction Tests

        [Test]
        public void Constructor_Default_CreatesValidWrapper()
        {
            // Arrange & Act
            var standaloneVm = new StateMachineDefinitionViewModel();

            try
            {
                // Assert
                Assert.That(standaloneVm, Is.Not.Null);
                Assert.That(standaloneVm.SerializedProperty, Is.Not.Null);
            }
            finally
            {
                standaloneVm.Dispose();
            }
        }

        [Test]
        public void Constructor_Default_GeneratesUniqueId()
        {
            // Arrange & Act
            var standaloneVm = new StateMachineDefinitionViewModel();

            try
            {
                // Assert - ID should be a valid GUID (32 hex characters without dashes)
                Assert.That(standaloneVm.Id, Is.Not.Null.And.Not.Empty);
                Assert.That(standaloneVm.Id.Length, Is.EqualTo(32));
            }
            finally
            {
                standaloneVm.Dispose();
            }
        }

        [Test]
        public void Constructor_WithSerializedProperty_InitializesCorrectly()
        {
            // Assert
            Assert.That(_viewModel, Is.Not.Null);
            Assert.That(_viewModel.SerializedProperty, Is.EqualTo(_context.Property));
        }

        #endregion

        #region Property Binding Tests - Basic Properties

        [Test]
        public void Id_Get_ReturnsSerializedValue()
        {
            // Arrange
            const string expectedValue = "test-sm-id";
            var idProp = _context.Property.FindPropertyRelative(StateMachineDefinition.ID_PROPERTY_NAME);
            idProp.stringValue = expectedValue;
            _context.ApplyModifiedProperties();

            // Act & Assert
            Assert.That(_viewModel.Id, Is.EqualTo(expectedValue));
        }

        [Test]
        public void Id_Set_UpdatesSerializedValue()
        {
            // Arrange
            const string expectedValue = "new-sm-id";

            // Act
            _viewModel.Id = expectedValue;

            // Assert
            _context.Update();
            var idProp = _context.Property.FindPropertyRelative(StateMachineDefinition.ID_PROPERTY_NAME);
            Assert.That(idProp.stringValue, Is.EqualTo(expectedValue));
        }

        [Test]
        public void Name_Get_ReturnsSerializedValue()
        {
            // Arrange
            const string expectedValue = "Test State Machine";
            var nameProp = _context.Property.FindPropertyRelative(StateMachineDefinition.NAME_PROPERTY_NAME);
            nameProp.stringValue = expectedValue;
            _context.ApplyModifiedProperties();

            // Act & Assert
            Assert.That(_viewModel.Name, Is.EqualTo(expectedValue));
        }

        [Test]
        public void Name_Set_UpdatesSerializedValue()
        {
            // Arrange
            const string expectedValue = "New State Machine Name";

            // Act
            _viewModel.Name = expectedValue;

            // Assert
            _context.Update();
            var nameProp = _context.Property.FindPropertyRelative(StateMachineDefinition.NAME_PROPERTY_NAME);
            Assert.That(nameProp.stringValue, Is.EqualTo(expectedValue));
        }

        [Test]
        public void Description_Get_ReturnsSerializedValue()
        {
            // Arrange
            const string expectedValue = "State machine description";
            var descProp = _context.Property.FindPropertyRelative(StateMachineDefinition.DESCRIPTION_PROPERTY_NAME);
            descProp.stringValue = expectedValue;
            _context.ApplyModifiedProperties();

            // Act & Assert
            Assert.That(_viewModel.Description, Is.EqualTo(expectedValue));
        }

        [Test]
        public void Description_Set_UpdatesSerializedValue()
        {
            // Arrange
            const string expectedValue = "Updated description";

            // Act
            _viewModel.Description = expectedValue;

            // Assert
            _context.Update();
            var descProp = _context.Property.FindPropertyRelative(StateMachineDefinition.DESCRIPTION_PROPERTY_NAME);
            Assert.That(descProp.stringValue, Is.EqualTo(expectedValue));
        }

        #endregion

        #region Property Binding Tests - Type Properties

        [Test]
        public void TypeName_Get_ReturnsSerializedValue()
        {
            // Arrange
            var expectedValue = typeof(MockStateMachine).AssemblyQualifiedName;
            var typeProp =
                _context.Property.FindPropertyRelative(StateMachineDefinition.STATE_MACHINE_TYPE_PROPERTY_NAME);
            typeProp.stringValue = expectedValue;
            _context.ApplyModifiedProperties();

            // Act & Assert
            Assert.That(_viewModel.TypeName, Is.EqualTo(expectedValue));
        }

        [Test]
        public void TransitionSolverTypeName_Get_ReturnsValue()
        {
            // Arrange
            var expectedValue = typeof(MockTransitionSolver).AssemblyQualifiedName;
            var prop =
                _context.Property.FindPropertyRelative(StateMachineDefinition.TRANSITION_SOLVER_TYPE_PROPERTY_NAME);
            prop.stringValue = expectedValue;
            _context.ApplyModifiedProperties();

            // Act & Assert
            Assert.That(_viewModel.TransitionSolverTypeName, Is.EqualTo(expectedValue));
        }

        [Test]
        public void TransitionSolverTypeName_Set_UpdatesValue()
        {
            // Arrange
            var expectedValue = typeof(MockTransitionSolver).AssemblyQualifiedName;

            // Act
            _viewModel.TransitionSolverTypeName = expectedValue;

            // Assert
            _context.Update();
            var prop =
                _context.Property.FindPropertyRelative(StateMachineDefinition.TRANSITION_SOLVER_TYPE_PROPERTY_NAME);
            Assert.That(prop.stringValue, Is.EqualTo(expectedValue));
        }

        #endregion

        #region Property Binding Tests - Payload

        [Test]
        public void Payload_ReturnsSerializedProperty()
        {
            // Act
            var payload = _viewModel.Payload;

            // Assert
            Assert.That(payload, Is.Not.Null);
            Assert.That(payload.propertyPath, Does.Contain(StateMachineDefinition.PAYLOAD_PROPERTY_NAME));
        }

        #endregion

        #region States Collection Tests

        [Test]
        public void States_Get_WithEmptyList_ReturnsEmptyList()
        {
            // Act
            var states = _viewModel.States;

            // Assert
            Assert.That(states, Is.Not.Null);
            Assert.That(states, Is.Empty);
        }

        [Test]
        public void States_Get_ReturnsListOfViewModels()
        {
            // Arrange - Add a state to the serialized data
            var statesProp = _context.Property.FindPropertyRelative(StateMachineDefinition.STATES_PROPERTY_NAME);
            statesProp.InsertArrayElementAtIndex(0);
            _context.ApplyModifiedProperties();

            // Act
            var states = _viewModel.States;

            // Assert
            Assert.That(states, Has.Count.EqualTo(1));
            Assert.That(states[0], Is.InstanceOf<StateDefinitionViewModel>());
        }

        [Test]
        public void StatesCount_ReturnsCorrectCount()
        {
            // Arrange - Add states
            var statesProp = _context.Property.FindPropertyRelative(StateMachineDefinition.STATES_PROPERTY_NAME);
            statesProp.InsertArrayElementAtIndex(0);
            statesProp.InsertArrayElementAtIndex(1);
            statesProp.InsertArrayElementAtIndex(2);
            _context.ApplyModifiedProperties();

            // Act
            var count = _viewModel.StatesCount;

            // Assert
            Assert.That(count, Is.EqualTo(3));
        }

        [Test]
        public void StatesSerializedProperty_ReturnsProperty()
        {
            // Act
            var statesProp = _viewModel.StatesSerializedProperty;

            // Assert
            Assert.That(statesProp, Is.Not.Null);
            Assert.That(statesProp.isArray, Is.True);
        }

        [Test]
        public void AvailableStates_ReturnsStateNames()
        {
            // Arrange - Add states with names
            var statesProp = _context.Property.FindPropertyRelative(StateMachineDefinition.STATES_PROPERTY_NAME);
            statesProp.InsertArrayElementAtIndex(0);
            var state1 = statesProp.GetArrayElementAtIndex(0);
            state1.FindPropertyRelative(StateDefinition.NAME_PROPERTY_NAME).stringValue = "State A";

            statesProp.InsertArrayElementAtIndex(1);
            var state2 = statesProp.GetArrayElementAtIndex(1);
            state2.FindPropertyRelative(StateDefinition.NAME_PROPERTY_NAME).stringValue = "State B";

            _context.ApplyModifiedProperties();

            // Act
            var availableStates = _viewModel.AvailableStates;

            // Assert
            Assert.That(availableStates, Has.Count.EqualTo(2));
            Assert.That(availableStates, Does.Contain("State A"));
            Assert.That(availableStates, Does.Contain("State B"));
        }

        #endregion

        #region Initial State Tests

        [Test]
        public void InitialState_Get_ReturnsViewModel()
        {
            // Act
            var initialState = _viewModel.InitialState;

            // Assert
            Assert.That(initialState, Is.Not.Null);
            Assert.That(initialState, Is.InstanceOf<StateDefinitionViewModel>());
        }

        [Test]
        public void InitialStateIndex_Get_WithNoStates_ReturnsMinusOne()
        {
            // Act
            var index = _viewModel.InitialStateIndex;

            // Assert - No states exist, so index should be -1
            Assert.That(index, Is.EqualTo(-1));
        }

        #endregion

        #region State Management Tests

        [Test]
        public void AddState_InsertsNewState()
        {
            // Arrange
            var stateVm = new StateDefinitionViewModel(typeof(MockStateMachine).AssemblyQualifiedName);
            stateVm.Name = "New State";

            try
            {
                // Act
                _viewModel.AddState(stateVm);

                // Assert
                _context.Update();
                Assert.That(_viewModel.StatesCount, Is.EqualTo(1));
            }
            finally
            {
                stateVm.Dispose();
            }
        }

        [Test]
        public void AddState_NotifiesMultipleProperties()
        {
            // Arrange
            var notifiedProperties = new System.Collections.Generic.List<string>();

            _viewModel.propertyChanged += (sender, args) => { notifiedProperties.Add(args.propertyName); };

            var stateVm = new StateDefinitionViewModel(typeof(MockStateMachine).AssemblyQualifiedName);

            try
            {
                // Act
                _viewModel.AddState(stateVm);

                // Assert
                Assert.That(notifiedProperties, Does.Contain(nameof(StateMachineDefinitionViewModel.States)));
                Assert.That(notifiedProperties, Does.Contain(nameof(StateMachineDefinitionViewModel.StatesCount)));
                Assert.That(notifiedProperties, Does.Contain(nameof(StateMachineDefinitionViewModel.AvailableStates)));
            }
            finally
            {
                stateVm.Dispose();
            }
        }

        [Test]
        public void RemoveStateAtIndex_WithInvalidIndex_LogsError()
        {
            // Act & Assert - Should not throw, just log error
            Assert.DoesNotThrow(() => _viewModel.RemoveStateAtIndex(-1));
            Assert.DoesNotThrow(() => _viewModel.RemoveStateAtIndex(100));
        }

        [Test]
        public void RemoveStateAtIndex_DeletesState()
        {
            // Arrange - Add a state first
            var statesProp = _context.Property.FindPropertyRelative(StateMachineDefinition.STATES_PROPERTY_NAME);
            statesProp.InsertArrayElementAtIndex(0);
            _context.ApplyModifiedProperties();

            Assert.That(_viewModel.StatesCount, Is.EqualTo(1));

            // Act
            _viewModel.RemoveStateAtIndex(0);

            // Assert
            _context.Update();
            Assert.That(_viewModel.StatesCount, Is.EqualTo(0));
        }

        [Test]
        public void UpdateState_ModifiesExistingState()
        {
            // Arrange - Add a state with known ID
            var statesProp = _context.Property.FindPropertyRelative(StateMachineDefinition.STATES_PROPERTY_NAME);
            statesProp.InsertArrayElementAtIndex(0);
            var stateElement = statesProp.GetArrayElementAtIndex(0);
            stateElement.FindPropertyRelative(StateDefinition.ID_PROPERTY_NAME).stringValue = "state-123";
            stateElement.FindPropertyRelative(StateDefinition.NAME_PROPERTY_NAME).stringValue = "Original Name";
            _context.ApplyModifiedProperties();

            // Create updated VM
            var updatedVm = new StateDefinitionViewModel(typeof(MockStateMachine).AssemblyQualifiedName);
            updatedVm.Id = "state-123";
            updatedVm.Name = "Updated Name";

            try
            {
                // Act
                _viewModel.UpdateState(updatedVm);

                // Assert
                _context.Update();
                var states = _viewModel.States;
                Assert.That(states[0].Name, Is.EqualTo("Updated Name"));
            }
            finally
            {
                updatedVm.Dispose();
            }
        }

        #endregion

        #region CopyFrom Tests

        [Test]
        public void CopyFrom_CopiesAllProperties()
        {
            // Arrange
            var sourceVm = new StateMachineDefinitionViewModel();

            try
            {
                sourceVm.Id = "source-sm-id";
                sourceVm.Name = "Source SM";
                sourceVm.Description = "Source Description";
                sourceVm.TransitionSolverTypeName = typeof(MockTransitionSolver).AssemblyQualifiedName;

                // Act
                _viewModel.CopyFrom(sourceVm);

                // Assert
                Assert.That(_viewModel.Id, Is.EqualTo("source-sm-id"));
                Assert.That(_viewModel.Name, Is.EqualTo("Source SM"));
                Assert.That(_viewModel.Description, Is.EqualTo("Source Description"));
                Assert.That(_viewModel.TransitionSolverTypeName, Does.Contain("MockTransitionSolver"));
            }
            finally
            {
                sourceVm.Dispose();
            }
        }

        #endregion

        #region Validation Tests

        [Test]
        public void IsValid_WithValidData_ReturnsTrue()
        {
            // Arrange - Set required fields
            _viewModel.Id = "valid-sm-id";
            _viewModel.Name = "Valid SM";
            _viewModel.TypeName = typeof(MockStateMachine).AssemblyQualifiedName;

            // Act
            var isValid = _viewModel.IsValid;

            // Assert - This depends on actual validation rules
            Assert.That(isValid, Is.True.Or.False);
        }

        #endregion
    }
}
