using System;
using Dev.Cortez.StateMachines.Core.Data;
using NUnit.Framework;

namespace Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Unit.Core
{
    /// <summary>
    /// Unit tests for StateSettings data class.
    /// Tests constructor and property assignment.
    /// </summary>
    [TestFixture]
    public sealed class StateSettingsTests
    {
        #region Constructor Tests

        [Test]
        public void Constructor_SetsAllProperties()
        {
            // Arrange
            var id = "test-state";
            var name = "Test State";
            var description = "A test state description";
            var type = typeof(object);

            // Act
            var settings = new StateSettings(id, name, description, type);

            // Assert
            Assert.That(settings.Id, Is.EqualTo(id));
            Assert.That(settings.Name, Is.EqualTo(name));
            Assert.That(settings.Description, Is.EqualTo(description));
            Assert.That(settings.Type, Is.EqualTo(type));
        }

        [Test]
        public void Constructor_WithNullId_SetsNullId()
        {
            // Act
            var settings = new StateSettings(null, "Name", "Desc", typeof(object));

            // Assert
            Assert.That(settings.Id, Is.Null);
        }

        [Test]
        public void Constructor_WithNullType_SetsNullType()
        {
            // Act
            var settings = new StateSettings("id", "Name", "Desc", null);

            // Assert
            Assert.That(settings.Type, Is.Null);
        }

        [Test]
        public void Constructor_WithEmptyStrings_SetsEmptyStrings()
        {
            // Act
            var settings = new StateSettings("", "", "", typeof(int));

            // Assert
            Assert.That(settings.Id, Is.EqualTo(""));
            Assert.That(settings.Name, Is.EqualTo(""));
            Assert.That(settings.Description, Is.EqualTo(""));
        }

        #endregion
    }
}
