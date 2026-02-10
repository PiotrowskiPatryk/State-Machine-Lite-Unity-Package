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
    /// Unit tests for ConditionDefinitionViewModel.
    /// Tests construction, property binding, and disposal.
    /// </summary>
    [TestFixture]
    [Category("ViewModels")]
    public sealed class ConditionDefinitionViewModelTests
    {
        private TestPropertyContext<ConditionDefinition> _context;
        private ConditionDefinitionViewModel _viewModel;

        [SetUp]
        public void SetUp()
        {
            _context = SerializedPropertyTestHelper.CreateTestContext<ConditionDefinition>();
            _viewModel = new ConditionDefinitionViewModel(_context.Property);
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
            var standaloneVm = new ConditionDefinitionViewModel();

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
            var standaloneVm = new ConditionDefinitionViewModel();

            try
            {
                // Assert
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

        #region Property Binding Tests

        [Test]
        public void Id_Get_ReturnsSerializedValue()
        {
            // Arrange
            const string expectedValue = "condition-id-123";
            var idProp = _context.Property.FindPropertyRelative(ConditionDefinition.ID_PROPERTY_NAME);
            idProp.stringValue = expectedValue;
            _context.ApplyModifiedProperties();

            // Act & Assert
            Assert.That(_viewModel.Id, Is.EqualTo(expectedValue));
        }

        [Test]
        public void Id_Set_UpdatesSerializedValue()
        {
            // Arrange
            const string expectedValue = "new-condition-id";

            // Act
            _viewModel.Id = expectedValue;

            // Assert
            _context.Update();
            var idProp = _context.Property.FindPropertyRelative(ConditionDefinition.ID_PROPERTY_NAME);
            Assert.That(idProp.stringValue, Is.EqualTo(expectedValue));
        }

        [Test]
        public void Name_Get_ReturnsSerializedValue()
        {
            // Arrange
            const string expectedValue = "Test Condition";
            var nameProp = _context.Property.FindPropertyRelative(ConditionDefinition.NAME_PROPERTY_NAME);
            nameProp.stringValue = expectedValue;
            _context.ApplyModifiedProperties();

            // Act & Assert
            Assert.That(_viewModel.Name, Is.EqualTo(expectedValue));
        }

        [Test]
        public void Name_Set_UpdatesSerializedValue()
        {
            // Arrange
            const string expectedValue = "New Condition Name";

            // Act
            _viewModel.Name = expectedValue;

            // Assert
            _context.Update();
            var nameProp = _context.Property.FindPropertyRelative(ConditionDefinition.NAME_PROPERTY_NAME);
            Assert.That(nameProp.stringValue, Is.EqualTo(expectedValue));
        }

        [Test]
        public void Description_Get_ReturnsSerializedValue()
        {
            // Arrange
            const string expectedValue = "Condition description";
            var descProp = _context.Property.FindPropertyRelative(ConditionDefinition.DESCRIPTION_PROPERTY_NAME);
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
            var descProp = _context.Property.FindPropertyRelative(ConditionDefinition.DESCRIPTION_PROPERTY_NAME);
            Assert.That(descProp.stringValue, Is.EqualTo(expectedValue));
        }

        [Test]
        public void TypeName_Get_ReturnsSerializedValue()
        {
            // Arrange
            var expectedValue = typeof(MockCondition).AssemblyQualifiedName;
            var typeProp = _context.Property.FindPropertyRelative(ConditionDefinition.TYPE_NAME_PROPERTY_NAME);
            typeProp.stringValue = expectedValue;
            _context.ApplyModifiedProperties();

            // Act & Assert
            Assert.That(_viewModel.TypeName, Is.EqualTo(expectedValue));
        }

        [Test]
        public void TypeNameShort_ReturnsClassNameOnly()
        {
            // Arrange
            var fullTypeName = typeof(MockCondition).AssemblyQualifiedName;
            var typeProp = _context.Property.FindPropertyRelative(ConditionDefinition.TYPE_NAME_PROPERTY_NAME);
            typeProp.stringValue = fullTypeName;
            _context.ApplyModifiedProperties();

            // Act
            var shortName = _viewModel.TypeNameShort;

            // Assert
            Assert.That(shortName, Is.EqualTo("MockCondition"));
        }

        [Test]
        public void Payload_ReturnsSerializedProperty()
        {
            // Act
            var payload = _viewModel.Payload;

            // Assert
            Assert.That(payload, Is.Not.Null);
            Assert.That(payload.propertyPath, Does.Contain(ConditionDefinition.PAYLOAD_PROPERTY_NAME));
        }

        #endregion

        #region CopyFrom Tests

        [Test]
        public void CopyFrom_CopiesAllProperties()
        {
            // Arrange
            var sourceVm = new ConditionDefinitionViewModel();

            try
            {
                sourceVm.Id = "source-condition-id";
                sourceVm.Name = "Source Condition";
                sourceVm.Description = "Source Description";

                // Act
                _viewModel.CopyFrom(sourceVm);

                // Assert
                Assert.That(_viewModel.Id, Is.EqualTo("source-condition-id"));
                Assert.That(_viewModel.Name, Is.EqualTo("Source Condition"));
                Assert.That(_viewModel.Description, Is.EqualTo("Source Description"));
            }
            finally
            {
                sourceVm.Dispose();
            }
        }

        #endregion
    }
}
