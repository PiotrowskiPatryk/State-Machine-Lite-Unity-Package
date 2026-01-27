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
    /// Unit tests for TriggerConfigurationViewModel.
    /// Tests collection management and property binding.
    /// </summary>
    [TestFixture]
    [Category("ViewModels")]
    public sealed class TriggerConfigurationViewModelTests
    {
        private DefinitionWrapper<TriggerConfiguration> _wrapper;
        private SerializedObject _serializedObject;
        private SerializedProperty _property;
        private TriggerConfigurationViewModel _viewModel;

        [SetUp]
        public void SetUp()
        {
            _wrapper = ScriptableObject.CreateInstance<DefinitionWrapper<TriggerConfiguration>>();
            _serializedObject = new SerializedObject(_wrapper);
            _property = _serializedObject.FindProperty(DefinitionWrapper<TriggerConfiguration>.DATA_PROPERTY_NAME)
                .FindPropertyRelative(TriggerConfiguration.TRIGGERS_PROPERTY_NAME);
            _viewModel = new TriggerConfigurationViewModel(_property);
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
        public void Triggers_Get_ReturnsListOfViewModels()
        {
            // Arrange - Add a trigger
            _property.InsertArrayElementAtIndex(0);
            _serializedObject.ApplyModifiedProperties();

            // Act
            var triggers = _viewModel.Triggers;

            // Assert
            Assert.That(triggers, Is.Not.Null);
            Assert.That(triggers, Has.Count.EqualTo(1));
        }

        [Test]
        public void TriggersCount_ReturnsCorrectCount()
        {
            // Arrange - Add triggers
            _property.InsertArrayElementAtIndex(0);
            _property.InsertArrayElementAtIndex(1);
            _property.InsertArrayElementAtIndex(2);
            _serializedObject.ApplyModifiedProperties();

            // Act
            var count = _viewModel.TriggersCount;

            // Assert
            Assert.That(count, Is.EqualTo(3));
        }

        [Test]
        public void GetTriggerByIndex_ReturnsCorrectVM()
        {
            // Arrange - Add a trigger with known ID
            _property.InsertArrayElementAtIndex(0);
            var element = _property.GetArrayElementAtIndex(0);
            element.FindPropertyRelative(TriggerDefinition.ID_PROPERTY_NAME).stringValue = "trigger-test-id";
            _serializedObject.ApplyModifiedProperties();

            // Act
            var vm = _viewModel.GetTriggerByIndex(0);

            // Assert
            Assert.That(vm, Is.Not.Null);
            Assert.That(vm.Id, Is.EqualTo("trigger-test-id"));
        }

        [Test]
        public void GetTriggerByIndex_WithInvalidIndex_ReturnsNull()
        {
            // Act
            var vm = _viewModel.GetTriggerByIndex(-1);
            var vm2 = _viewModel.GetTriggerByIndex(100);

            // Assert
            Assert.That(vm, Is.Null);
            Assert.That(vm2, Is.Null);
        }

        #endregion

        #region Management Tests

        [Test]
        public void AddTrigger_InsertsTrigger()
        {
            // Arrange
            var triggerVm = new TriggerDefinitionViewModel();
            triggerVm.Name = "Test Trigger";

            try
            {
                // Act
                _viewModel.AddTrigger(triggerVm);

                // Assert
                _serializedObject.Update();
                Assert.That(_viewModel.TriggersCount, Is.EqualTo(1));
            }
            finally
            {
                triggerVm.Dispose();
            }
        }

        [Test]
        public void RemoveTriggerAtIndex_DeletesTrigger()
        {
            // Arrange - Add a trigger first
            _property.InsertArrayElementAtIndex(0);
            _serializedObject.ApplyModifiedProperties();

            Assert.That(_viewModel.TriggersCount, Is.EqualTo(1));

            // Act
            _viewModel.RemoveTriggerAtIndex(0);

            // Assert
            _serializedObject.Update();
            Assert.That(_viewModel.TriggersCount, Is.EqualTo(0));
        }

        [Test]
        public void RemoveTriggerAtIndex_WithInvalidIndex_LogsError()
        {
            // Act & Assert - Should not throw
            Assert.DoesNotThrow(() => _viewModel.RemoveTriggerAtIndex(-1));
            Assert.DoesNotThrow(() => _viewModel.RemoveTriggerAtIndex(100));
        }

        [Test]
        public void UpdateTrigger_ModifiesExistingTrigger()
        {
            // Arrange - Add a trigger with known ID
            _property.InsertArrayElementAtIndex(0);
            var element = _property.GetArrayElementAtIndex(0);
            element.FindPropertyRelative(TriggerDefinition.ID_PROPERTY_NAME).stringValue = "trigger-123";
            element.FindPropertyRelative(TriggerDefinition.NAME_PROPERTY_NAME).stringValue = "Original Name";
            _serializedObject.ApplyModifiedProperties();

            // Create updated VM
            var updatedVm = new TriggerDefinitionViewModel();
            updatedVm.Id = "trigger-123";
            updatedVm.Name = "Updated Name";

            try
            {
                // Act
                _viewModel.UpdateTrigger(updatedVm);

                // Assert
                _serializedObject.Update();
                var triggers = _viewModel.Triggers;
                Assert.That(triggers[0].Name, Is.EqualTo("Updated Name"));
            }
            finally
            {
                updatedVm.Dispose();
            }
        }

        #endregion
    }
}
