using System.Collections.Generic;
using Dev.Cortez.StateMachines.Core.Condition;
using Dev.Cortez.StateMachines.Core.StateMachineConfiguration.Definition;
using NUnit.Framework;

namespace Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Unit.Core
{
    /// <summary>
    /// Unit tests for TransitionRuleDefinition.
    /// Tests property access for serializable transition rule data.
    /// </summary>
    [TestFixture]
    public sealed class TransitionRuleDefinitionTests
    {
        #region Constructor and Property Tests

        [Test]
        public void PropertyNames_AreNotNull()
        {
            // Assert - Static property names for serialization are defined
            Assert.That(TransitionRuleDefinition.PRIORITY_PROPERTY_NAME, Is.Not.Null);
            Assert.That(TransitionRuleDefinition.CONDITION_DEFINITIONS_PROPERTY_NAME, Is.Not.Null);
            Assert.That(TransitionRuleDefinition.TARGET_STATE_PROPERTY_NAME, Is.Not.Null);
            Assert.That(TransitionRuleDefinition.CONDITION_FILTER_TYPE_PROPERTY_NAME, Is.Not.Null);
        }

        [Test]
        public void PropertyNames_AreCorrectFieldNames()
        {
            // Assert - Property names match expected field names
            Assert.That(TransitionRuleDefinition.PRIORITY_PROPERTY_NAME, Is.EqualTo("_priority"));
            Assert.That(TransitionRuleDefinition.TARGET_STATE_PROPERTY_NAME, Is.EqualTo("_targetState"));
            Assert.That(TransitionRuleDefinition.CONDITION_DEFINITIONS_PROPERTY_NAME, Is.EqualTo("_conditionDefinitions"));
            Assert.That(TransitionRuleDefinition.CONDITION_FILTER_TYPE_PROPERTY_NAME, Is.EqualTo("_conditionFilterType"));
        }

        [Test]
        public void DefaultInstance_HasDefaultValues()
        {
            // Arrange & Act
            var definition = new TransitionRuleDefinition();

            // Assert - Default values for uninitialized fields
            Assert.That(definition.TargetState, Is.Null);
            Assert.That(definition.Priority, Is.EqualTo(0));
            Assert.That(definition.ConditionDefinitions, Is.Null);
            Assert.That(definition.ConditionFilterType, Is.EqualTo(ConditionFilterType.Undefined));
        }

        #endregion
    }
}
