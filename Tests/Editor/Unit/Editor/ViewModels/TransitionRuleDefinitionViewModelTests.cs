using Dev.Cortez.StateMachines.Core.Condition;
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
    /// Unit tests for TransitionRuleDefinitionViewModel.
    /// Tests construction, property binding, conditions management, and disposal.
    /// </summary>
    [TestFixture]
    [Category("ViewModels")]
    public sealed class TransitionRuleDefinitionViewModelTests
    {
        private TestPropertyContext<TransitionRuleDefinition> _context;
        private TransitionRuleDefinitionViewModel _viewModel;

        [SetUp]
        public void SetUp()
        {
            _context = SerializedPropertyTestHelper.CreateTestContext<TransitionRuleDefinition>();
            _viewModel = new TransitionRuleDefinitionViewModel(_context.Property);
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
            var standaloneVm = new TransitionRuleDefinitionViewModel();

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
        public void Constructor_WithSerializedProperty_InitializesCorrectly()
        {
            // Assert
            Assert.That(_viewModel, Is.Not.Null);
            Assert.That(_viewModel.SerializedProperty, Is.EqualTo(_context.Property));
        }

        #endregion

        #region Property Binding Tests

        [Test]
        public void Priority_Get_ReturnsIntValue()
        {
            // Arrange
            const int expectedValue = 10;
            var priorityProp = _context.Property.FindPropertyRelative(TransitionRuleDefinition.PRIORITY_PROPERTY_NAME);
            priorityProp.intValue = expectedValue;
            _context.ApplyModifiedProperties();

            // Act & Assert
            Assert.That(_viewModel.Priority, Is.EqualTo(expectedValue));
        }

        [Test]
        public void Priority_Set_UpdatesSerializedValue()
        {
            // Arrange
            const int expectedValue = 25;

            // Act
            _viewModel.Priority = expectedValue;

            // Assert
            _context.Update();
            var priorityProp = _context.Property.FindPropertyRelative(TransitionRuleDefinition.PRIORITY_PROPERTY_NAME);
            Assert.That(priorityProp.intValue, Is.EqualTo(expectedValue));
        }

        [Test]
        public void ConditionFilterType_Get_ReturnsEnumValue()
        {
            // Arrange
            var expectedValue = ConditionFilterType.All;
            var filterProp =
                _context.Property.FindPropertyRelative(TransitionRuleDefinition.CONDITION_FILTER_TYPE_PROPERTY_NAME);
            filterProp.enumValueIndex = (int)expectedValue;
            _context.ApplyModifiedProperties();

            // Act & Assert
            Assert.That(_viewModel.ConditionFilterType, Is.EqualTo(expectedValue));
        }

        [Test]
        public void ConditionFilterType_Set_UpdatesSerializedValue()
        {
            // Arrange
            var expectedValue = ConditionFilterType.Any;

            // Act
            _viewModel.ConditionFilterType = expectedValue;

            // Assert
            _context.Update();
            var filterProp =
                _context.Property.FindPropertyRelative(TransitionRuleDefinition.CONDITION_FILTER_TYPE_PROPERTY_NAME);
            Assert.That((ConditionFilterType)filterProp.enumValueIndex, Is.EqualTo(expectedValue));
        }

        [Test]
        public void TargetState_Get_ReturnsStateViewModel()
        {
            // Act
            var targetState = _viewModel.TargetState;

            // Assert
            Assert.That(targetState, Is.Not.Null);
            Assert.That(targetState, Is.InstanceOf<StateDefinitionViewModel>());
        }

        #endregion

        #region Conditions Collection Tests

        [Test]
        public void Conditions_Get_WithEmptyList_ReturnsEmptyList()
        {
            // Act
            var conditions = _viewModel.Conditions;

            // Assert
            Assert.That(conditions, Is.Not.Null);
            Assert.That(conditions, Is.Empty);
        }

        [Test]
        public void Conditions_Get_ReturnsList()
        {
            // Arrange - Add a condition to the serialized data
            var conditionsProp =
                _context.Property.FindPropertyRelative(TransitionRuleDefinition.CONDITION_DEFINITIONS_PROPERTY_NAME);
            conditionsProp.InsertArrayElementAtIndex(0);
            _context.ApplyModifiedProperties();

            // Act
            var conditions = _viewModel.Conditions;

            // Assert
            Assert.That(conditions, Has.Count.EqualTo(1));
            Assert.That(conditions[0], Is.InstanceOf<ConditionDefinitionViewModel>());
        }

        [Test]
        public void AddCondition_InsertsCondition()
        {
            // Arrange
            var conditionVm = new ConditionDefinitionViewModel();
            conditionVm.Name = "Test Condition";

            try
            {
                // Act
                _viewModel.AddCondition(conditionVm);

                // Assert
                _context.Update();
                var conditionsProp =
                    _context.Property.FindPropertyRelative(TransitionRuleDefinition.CONDITION_DEFINITIONS_PROPERTY_NAME);
                Assert.That(conditionsProp.arraySize, Is.EqualTo(1));
            }
            finally
            {
                conditionVm.Dispose();
            }
        }

        [Test]
        public void AddCondition_NotifiesPropertyChanged()
        {
            // Arrange
            var notified = false;

            _viewModel.propertyChanged += (sender, args) =>
            {
                if (args.propertyName == nameof(TransitionRuleDefinitionViewModel.Conditions))
                {
                    notified = true;
                }
            };

            var conditionVm = new ConditionDefinitionViewModel();

            try
            {
                // Act
                _viewModel.AddCondition(conditionVm);

                // Assert
                Assert.That(notified, Is.True);
            }
            finally
            {
                conditionVm.Dispose();
            }
        }

        [Test]
        public void RemoveCondition_DeletesByIndex()
        {
            // Arrange - Add a condition first
            var conditionsProp =
                _context.Property.FindPropertyRelative(TransitionRuleDefinition.CONDITION_DEFINITIONS_PROPERTY_NAME);
            conditionsProp.InsertArrayElementAtIndex(0);
            _context.ApplyModifiedProperties();

            Assert.That(_viewModel.Conditions, Has.Count.EqualTo(1));

            // Act
            _viewModel.RemoveCondition(0);

            // Assert
            _context.Update();
            Assert.That(_viewModel.Conditions, Is.Empty);
        }

        #endregion

        #region CopyFrom Tests

        [Test]
        public void CopyFrom_CopiesPriority()
        {
            // Arrange
            var sourceVm = new TransitionRuleDefinitionViewModel();

            try
            {
                sourceVm.Priority = 99;

                // Act
                _viewModel.CopyFrom(sourceVm);

                // Assert
                Assert.That(_viewModel.Priority, Is.EqualTo(99));
            }
            finally
            {
                sourceVm.Dispose();
            }
        }

        [Test]
        public void CopyFrom_CopiesConditionFilterType()
        {
            // Arrange
            var sourceVm = new TransitionRuleDefinitionViewModel();

            try
            {
                sourceVm.ConditionFilterType = ConditionFilterType.Any;

                // Act
                _viewModel.CopyFrom(sourceVm);

                // Assert
                Assert.That(_viewModel.ConditionFilterType, Is.EqualTo(ConditionFilterType.Any));
            }
            finally
            {
                sourceVm.Dispose();
            }
        }

        #endregion
    }
}
