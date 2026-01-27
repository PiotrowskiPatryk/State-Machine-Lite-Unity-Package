using Dev.Cortez.StateMachines.Core.StateMachineConfiguration;
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
    /// Unit tests for StateMachineConfigurationViewModel.
    /// Tests collection management and property binding.
    /// </summary>
    [TestFixture]
    [Category("ViewModels")]
    public sealed class StateMachineConfigurationViewModelTests
    {
        private StateMachineConfigurationWrapper _wrapper;
        private SerializedObject _serializedObject;
        private SerializedProperty _property;
        private StateMachineConfigurationViewModel _viewModel;

        [SetUp]
        public void SetUp()
        {
            _wrapper = ScriptableObject.CreateInstance<StateMachineConfigurationWrapper>();
            _serializedObject = new SerializedObject(_wrapper);
            var dataProperty = _serializedObject.FindProperty(StateMachineConfigurationWrapper.DATA_PROPERTY_NAME);
            _property = dataProperty.FindPropertyRelative(StateMachineConfiguration.STATE_MACHINES_PROPERTY_NAME);
            _viewModel = new StateMachineConfigurationViewModel(dataProperty);
        }

        [TearDown]
        public void TearDown()
        {
            _viewModel = null;
            _serializedObject?.Dispose();

            if (_wrapper != null)
            {
                Object.DestroyImmediate(_wrapper);
            }
        }

        #region Construction Tests

        [Test]
        public void Constructor_InitializesCorrectly()
        {
            // Assert
            Assert.That(_viewModel, Is.Not.Null);
        }

        #endregion

        #region Collection Tests

        [Test]
        public void StateMachines_Get_ReturnsListOfViewModels()
        {
            // Arrange - Add a state machine
            _property.InsertArrayElementAtIndex(0);
            _serializedObject.ApplyModifiedProperties();

            // Act
            var stateMachines = _viewModel.StateMachines;

            // Assert
            Assert.That(stateMachines, Is.Not.Null);
            Assert.That(stateMachines, Has.Count.EqualTo(1));
        }

        [Test]
        public void StateMachinesCount_ReturnsCorrectCount()
        {
            // Arrange - Add state machines
            _property.InsertArrayElementAtIndex(0);
            _property.InsertArrayElementAtIndex(1);
            _serializedObject.ApplyModifiedProperties();

            // Act
            var count = _viewModel.StateMachinesCount;

            // Assert
            Assert.That(count, Is.EqualTo(2));
        }

        [Test]
        public void GetStateMachineByIndex_ReturnsCorrectVM()
        {
            // Arrange - Add a state machine with known ID
            _property.InsertArrayElementAtIndex(0);
            var element = _property.GetArrayElementAtIndex(0);
            element.FindPropertyRelative(StateMachineDefinition.ID_PROPERTY_NAME).stringValue = "sm-test-id";
            _serializedObject.ApplyModifiedProperties();

            // Act
            var vm = _viewModel.GetStateMachineByIndex(0);

            // Assert
            Assert.That(vm, Is.Not.Null);
            Assert.That(vm.Id, Is.EqualTo("sm-test-id"));
        }

        [Test]
        public void GetStateMachineByIndex_WithInvalidIndex_ReturnsNull()
        {
            // Act
            var vm = _viewModel.GetStateMachineByIndex(-1);
            var vm2 = _viewModel.GetStateMachineByIndex(100);

            // Assert
            Assert.That(vm, Is.Null);
            Assert.That(vm2, Is.Null);
        }

        #endregion

        #region Management Tests

        [Test]
        public void AddStateMachine_InsertsNewMachine()
        {
            // Arrange
            var smVm = new StateMachineDefinitionViewModel();
            smVm.Name = "Test SM";

            try
            {
                // Act
                _viewModel.AddStateMachine(smVm);

                // Assert
                _serializedObject.Update();
                Assert.That(_viewModel.StateMachinesCount, Is.EqualTo(1));
            }
            finally
            {
                smVm.Dispose();
            }
        }

        [Test]
        public void RemoveStateMachineAtIndex_DeletesMachine()
        {
            // Arrange - Add a state machine first
            _property.InsertArrayElementAtIndex(0);
            _serializedObject.ApplyModifiedProperties();

            Assert.That(_viewModel.StateMachinesCount, Is.EqualTo(1));

            // Act
            _viewModel.RemoveStateMachineAtIndex(0);

            // Assert
            _serializedObject.Update();
            Assert.That(_viewModel.StateMachinesCount, Is.EqualTo(0));
        }

        [Test]
        public void RemoveStateMachineAtIndex_WithInvalidIndex_LogsError()
        {
            // Act & Assert - Should not throw
            UnityEngine.TestTools.LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex("Invalid index -1 for state machine count \\d+"));
            Assert.DoesNotThrow(() => _viewModel.RemoveStateMachineAtIndex(-1));
            
            UnityEngine.TestTools.LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex("Invalid index 100 for state machine count \\d+"));
            Assert.DoesNotThrow(() => _viewModel.RemoveStateMachineAtIndex(100));
        }

        #endregion
    }
}
