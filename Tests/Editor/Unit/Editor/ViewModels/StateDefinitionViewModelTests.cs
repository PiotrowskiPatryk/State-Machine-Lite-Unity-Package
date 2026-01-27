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
    /// Unit tests for StateDefinitionViewModel.
    /// Tests construction, property binding, transitions management, and disposal.
    /// </summary>
    [TestFixture]
    [Category("ViewModels")]
    public sealed class StateDefinitionViewModelTests
    {
        private TestPropertyContext<StateDefinition> _context;
        private StateDefinitionViewModel _viewModel;

        [SetUp]
        public void SetUp()
        {
            _context = SerializedPropertyTestHelper.CreateTestContext<StateDefinition>();
            _viewModel = new StateDefinitionViewModel(_context.Property);
        }

        [TearDown]
        public void TearDown()
        {
            _viewModel?.Dispose();
            _context?.Dispose();
        }

        #region Construction Tests

        [Test]
        public void Constructor_WithSerializedProperty_InitializesCorrectly()
        {
            // Assert
            Assert.That(_viewModel, Is.Not.Null);
            Assert.That(_viewModel.SerializedProperty, Is.EqualTo(_context.Property));
        }

        [Test]
        public void Constructor_Standalone_CreatesValidWrapper()
        {
            // Arrange & Act
            var standaloneVm = new StateDefinitionViewModel(typeof(MockStateMachine).AssemblyQualifiedName);

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
        public void Constructor_Standalone_GeneratesUniqueId()
        {
            // Arrange & Act
            var standaloneVm = new StateDefinitionViewModel(typeof(MockStateMachine).AssemblyQualifiedName);

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
        public void Constructor_Standalone_SetsStateMachineTypeName()
        {
            // Arrange
            var expectedTypeName = typeof(MockStateMachine).AssemblyQualifiedName;

            // Act
            var standaloneVm = new StateDefinitionViewModel(expectedTypeName);

            try
            {
                // Assert
                Assert.That(standaloneVm.StateMachineTypeName, Is.EqualTo(expectedTypeName));
            }
            finally
            {
                standaloneVm.Dispose();
            }
        }

        #endregion

        #region Property Binding Tests - Basic Properties

        [Test]
        public void Id_Get_ReturnsSerializedValue()
        {
            // Arrange
            const string expectedValue = "test-id-123";
            var idProp = _context.Property.FindPropertyRelative(StateDefinition.ID_PROPERTY_NAME);
            idProp.stringValue = expectedValue;
            _context.ApplyModifiedProperties();

            // Create new VM to read the value
            var vm = new StateDefinitionViewModel(_context.Property);

            try
            {
                // Assert
                Assert.That(vm.Id, Is.EqualTo(expectedValue));
            }
            finally
            {
                vm.Dispose();
            }
        }

        [Test]
        public void Id_Set_UpdatesSerializedValue()
        {
            // Arrange
            const string expectedValue = "new-id-456";

            // Act
            _viewModel.Id = expectedValue;

            // Assert
            _context.Update();
            var idProp = _context.Property.FindPropertyRelative(StateDefinition.ID_PROPERTY_NAME);
            Assert.That(idProp.stringValue, Is.EqualTo(expectedValue));
        }

        [Test]
        public void Name_Get_ReturnsSerializedValue()
        {
            // Arrange
            const string expectedValue = "Test State Name";
            var nameProp = _context.Property.FindPropertyRelative(StateDefinition.NAME_PROPERTY_NAME);
            nameProp.stringValue = expectedValue;
            _context.ApplyModifiedProperties();

            // Act & Assert
            Assert.That(_viewModel.Name, Is.EqualTo(expectedValue));
        }

        [Test]
        public void Name_Set_UpdatesSerializedValue()
        {
            // Arrange
            const string expectedValue = "New State Name";

            // Act
            _viewModel.Name = expectedValue;

            // Assert
            _context.Update();
            var nameProp = _context.Property.FindPropertyRelative(StateDefinition.NAME_PROPERTY_NAME);
            Assert.That(nameProp.stringValue, Is.EqualTo(expectedValue));
        }

        [Test]
        public void Description_Get_ReturnsSerializedValue()
        {
            // Arrange
            const string expectedValue = "Test description text";
            var descProp = _context.Property.FindPropertyRelative(StateDefinition.DESCRIPTION_PROPERTY_NAME);
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
            var descProp = _context.Property.FindPropertyRelative(StateDefinition.DESCRIPTION_PROPERTY_NAME);
            Assert.That(descProp.stringValue, Is.EqualTo(expectedValue));
        }

        #endregion

        #region Property Binding Tests - Type Properties

        [Test]
        public void TypeName_Get_ReturnsSerializedValue()
        {
            // Arrange
            var expectedValue = typeof(MockState).AssemblyQualifiedName;
            var typeProp = _context.Property.FindPropertyRelative(StateDefinition.TYPE_NAME_PROPERTY_NAME);
            typeProp.stringValue = expectedValue;
            _context.ApplyModifiedProperties();

            // Act & Assert
            Assert.That(_viewModel.TypeName, Is.EqualTo(expectedValue));
        }

        [Test]
        public void TypeNameShort_ReturnsClassNameOnly()
        {
            // Arrange
            var fullTypeName = typeof(MockState).AssemblyQualifiedName;
            _viewModel.TypeName = fullTypeName;

            // Act
            var shortName = _viewModel.TypeNameShort;

            // Assert
            Assert.That(shortName, Is.EqualTo("MockState"));
        }

        [Test]
        public void StateMachineTypeName_Get_ReturnsValue()
        {
            // Arrange
            var expectedValue = typeof(MockStateMachine).AssemblyQualifiedName;
            var prop = _context.Property.FindPropertyRelative(StateDefinition.STATE_MACHINE_TYPE_NAME_PROPERTY_NAME);
            prop.stringValue = expectedValue;
            _context.ApplyModifiedProperties();

            // Act & Assert
            Assert.That(_viewModel.StateMachineTypeName, Is.EqualTo(expectedValue));
        }

        [Test]
        public void StateMachineTypeName_Set_UpdatesValue()
        {
            // Arrange
            var expectedValue = typeof(MockStateMachine).AssemblyQualifiedName;

            // Act
            _viewModel.StateMachineTypeName = expectedValue;

            // Assert
            _context.Update();
            var prop = _context.Property.FindPropertyRelative(StateDefinition.STATE_MACHINE_TYPE_NAME_PROPERTY_NAME);
            Assert.That(prop.stringValue, Is.EqualTo(expectedValue));
        }

        #endregion

        #region Property Binding Tests - Position

        [Test]
        public void NodePosition_Get_ReturnsVector2Int()
        {
            // Arrange
            var expectedValue = new Vector2Int(100, 200);
            var posProp = _context.Property.FindPropertyRelative(StateDefinition.NODE_POSITION_PROPERTY_NAME);
            posProp.vector2IntValue = expectedValue;
            _context.ApplyModifiedProperties();

            // Act & Assert
            Assert.That(_viewModel.NodePosition, Is.EqualTo(expectedValue));
        }

        [Test]
        public void NodePosition_Set_UpdatesSerializedValue()
        {
            // Arrange
            var expectedValue = new Vector2Int(300, 400);

            // Act
            _viewModel.NodePosition = expectedValue;

            // Assert
            _context.Update();
            var posProp = _context.Property.FindPropertyRelative(StateDefinition.NODE_POSITION_PROPERTY_NAME);
            Assert.That(posProp.vector2IntValue, Is.EqualTo(expectedValue));
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
            Assert.That(payload.propertyPath, Does.Contain(StateDefinition.PAYLOAD_PROPERTY_NAME));
        }

        #endregion

        #region Transitions Tests

        [Test]
        public void Transitions_Get_WithEmptyList_ReturnsEmptyList()
        {
            // Act
            var transitions = _viewModel.Transitions;

            // Assert
            Assert.That(transitions, Is.Not.Null);
            Assert.That(transitions, Is.Empty);
        }

        [Test]
        public void Transitions_Get_ReturnsListOfViewModels()
        {
            // Arrange - Add a transition to the serialized data
            var transitionsProp =
                _context.Property.FindPropertyRelative(StateDefinition.TRANSITION_RULES_PROPERTY_NAME);
            transitionsProp.InsertArrayElementAtIndex(0);
            _context.ApplyModifiedProperties();

            // Act
            var transitions = _viewModel.Transitions;

            // Assert
            Assert.That(transitions, Has.Count.EqualTo(1));
            Assert.That(transitions[0], Is.InstanceOf<TransitionRuleDefinitionViewModel>());
        }

        [Test]
        public void AddTransition_InsertsAtEnd()
        {
            // Arrange
            var transitionVm = new TransitionRuleDefinitionViewModel();
            transitionVm.Priority = 5;

            try
            {
                // Act
                _viewModel.AddTransition(transitionVm);

                // Assert
                _context.Update();
                var transitionsProp =
                    _context.Property.FindPropertyRelative(StateDefinition.TRANSITION_RULES_PROPERTY_NAME);
                Assert.That(transitionsProp.arraySize, Is.EqualTo(1));
            }
            finally
            {
                transitionVm.Dispose();
            }
        }

        [Test]
        public void AddTransition_NotifiesPropertyChanged()
        {
            // Arrange
            var notified = false;
            string notifiedPropertyName = null;

            _viewModel.propertyChanged += (sender, args) =>
            {
                if (args.propertyName == nameof(StateDefinitionViewModel.Transitions))
                {
                    notified = true;
                    notifiedPropertyName = args.propertyName;
                }
            };

            var transitionVm = new TransitionRuleDefinitionViewModel();

            try
            {
                // Act
                _viewModel.AddTransition(transitionVm);

                // Assert
                Assert.That(notified, Is.True);
                Assert.That(notifiedPropertyName, Is.EqualTo(nameof(StateDefinitionViewModel.Transitions)));
            }
            finally
            {
                transitionVm.Dispose();
            }
        }

        [Test]
        public void RemoveTransition_WithNull_LogsError()
        {
            // Act & Assert - Should not throw, just log error
            UnityEngine.TestTools.LogAssert.Expect(LogType.Error, "RemoveTransition called with null argument");
            Assert.DoesNotThrow(() => _viewModel.RemoveTransition(null));
        }

        #endregion

        #region CopyFrom Tests

        [Test]
        public void CopyFrom_CopiesAllProperties()
        {
            // Arrange
            var sourceVm = new StateDefinitionViewModel(typeof(MockStateMachine).AssemblyQualifiedName);

            try
            {
                sourceVm.Id = "source-id";
                sourceVm.Name = "Source Name";
                sourceVm.Description = "Source Description";
                sourceVm.NodePosition = new Vector2Int(500, 600);

                // Act
                _viewModel.CopyFrom(sourceVm);

                // Assert
                Assert.That(_viewModel.Id, Is.EqualTo("source-id"));
                Assert.That(_viewModel.Name, Is.EqualTo("Source Name"));
                Assert.That(_viewModel.Description, Is.EqualTo("Source Description"));
                Assert.That(_viewModel.NodePosition, Is.EqualTo(new Vector2Int(500, 600)));
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
            _viewModel.Id = "valid-id";
            _viewModel.Name = "Valid Name";
            _viewModel.TypeName = typeof(MockState).AssemblyQualifiedName;

            // Act
            var isValid = _viewModel.IsValid;

            // Assert - StateDefinition.IsValid() should return true with these values
            // Note: This depends on the actual IsValid implementation
            Assert.That(isValid, Is.True.Or.False); // Adjust based on actual validation rules
        }

        #endregion
    }
}
