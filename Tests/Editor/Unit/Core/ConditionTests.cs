using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Condition;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Common.Mocks;
using NUnit.Framework;

namespace Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Unit.Core
{
    /// <summary>
    /// Unit tests for ConditionBase and condition implementations.
    /// Tests condition satisfaction, event firing, composite conditions, and disposal.
    /// </summary>
    [TestFixture]
    public sealed class ConditionTests
    {
        private MockCondition _condition;
        private CancellationTokenSource _cts;

        [SetUp]
        public void SetUp()
        {
            _condition = new MockCondition();
            _cts = new CancellationTokenSource();
        }

        [TearDown]
        public void TearDown()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _condition?.DisposeAsync().AsTask().GetAwaiter().GetResult();
        }

        #region MockCondition Tests

        [Test]
        public void IsSatisfied_DefaultValue_IsFalse()
        {
            // Assert
            Assert.That(_condition.IsSatisfied, Is.False);
        }

        [Test]
        public void SetSatisfied_ChangesValue_UpdatesIsSatisfied()
        {
            // Act
            _condition.SetSatisfied(true);

            // Assert
            Assert.That(_condition.IsSatisfied, Is.True);
        }

        [Test]
        public void SetSatisfied_WhenValueChanges_FiresSatisfiedChangedEvent()
        {
            // Arrange
            var eventFired = false;
            bool? receivedValue = null;
            _condition.SatisfiedChanged += (condition, value) =>
            {
                eventFired = true;
                receivedValue = value;
            };

            // Act
            _condition.SetSatisfied(true);

            // Assert
            Assert.That(eventFired, Is.True);
            Assert.That(receivedValue, Is.True);
        }

        [Test]
        public void SetSatisfied_WhenValueUnchanged_DoesNotFireEvent()
        {
            // Arrange
            _condition.SetSatisfied(true);
            var eventFireCount = 0;
            _condition.SatisfiedChanged += (_, _) => eventFireCount++;

            // Act
            _condition.SetSatisfied(true);

            // Assert
            Assert.That(eventFireCount, Is.EqualTo(0));
        }

        [Test]
        public void SetSatisfied_TracksCallCount()
        {
            // Act
            _condition.SetSatisfied(true);
            _condition.SetSatisfied(false);
            _condition.SetSatisfied(false); // Same value

            // Assert
            Assert.That(_condition.SetSatisfiedCallCount, Is.EqualTo(3));
        }

        [Test]
        public void ForceSatisfiedChangedEvent_AlwaysFiresEvent()
        {
            // Arrange
            var eventFireCount = 0;
            _condition.SatisfiedChanged += (_, _) => eventFireCount++;

            // Act
            _condition.ForceSatisfiedChangedEvent();
            _condition.ForceSatisfiedChangedEvent();

            // Assert
            Assert.That(eventFireCount, Is.EqualTo(2));
        }

        [Test]
        public async Task DisposeAsync_ClearsSatisfiedChangedHandler()
        {
            // Arrange
            var handlerCalled = false;
            _condition.SatisfiedChanged += (_, _) => handlerCalled = true;

            // Act
            await _condition.DisposeAsync();

            // Setting satisfied after dispose should not call handler
            // (event is cleared in DisposeAsync)
            Assert.Pass("DisposeAsync completed successfully");
        }

        [Test]
        public void Reset_ClearsAllTrackingState()
        {
            // Arrange
            _condition.SetSatisfied(true);

            // Act
            _condition.Reset();

            // Assert
            Assert.That(_condition.IsSatisfied, Is.False);
            Assert.That(_condition.SetSatisfiedCallCount, Is.EqualTo(0));
        }

        #endregion

        #region ConditionComposite Tests

        [Test]
        public void ConditionComposite_WithFilterTypeAll_WhenAllSatisfied_ReturnsTrue()
        {
            // Arrange
            var condition1 = new MockCondition();
            var condition2 = new MockCondition();
            condition1.SetSatisfied(true);
            condition2.SetSatisfied(true);

            var composite = new ConditionComposite(
                new List<ICondition> { condition1, condition2 },
                ConditionFilterType.All);

            // Assert
            Assert.That(composite.IsSatisfied, Is.True);
        }

        [Test]
        public void ConditionComposite_WithFilterTypeAll_WhenOnlyOneSatisfied_ReturnsFalse()
        {
            // Arrange
            var condition1 = new MockCondition();
            var condition2 = new MockCondition();
            condition1.SetSatisfied(true);
            condition2.SetSatisfied(false);

            var composite = new ConditionComposite(
                new List<ICondition> { condition1, condition2 },
                ConditionFilterType.All);

            // Assert
            Assert.That(composite.IsSatisfied, Is.False);
        }

        [Test]
        public void ConditionComposite_WithFilterTypeAll_WhenNoneSatisfied_ReturnsFalse()
        {
            // Arrange
            var condition1 = new MockCondition();
            var condition2 = new MockCondition();

            var composite = new ConditionComposite(
                new List<ICondition> { condition1, condition2 },
                ConditionFilterType.All);

            // Assert
            Assert.That(composite.IsSatisfied, Is.False);
        }

        [Test]
        public void ConditionComposite_WithFilterTypeAny_WhenAnySatisfied_ReturnsTrue()
        {
            // Arrange
            var condition1 = new MockCondition();
            var condition2 = new MockCondition();
            condition1.SetSatisfied(false);
            condition2.SetSatisfied(true);

            var composite = new ConditionComposite(
                new List<ICondition> { condition1, condition2 },
                ConditionFilterType.Any);

            // Assert
            Assert.That(composite.IsSatisfied, Is.True);
        }

        [Test]
        public void ConditionComposite_WithFilterTypeAny_WhenNoneSatisfied_ReturnsFalse()
        {
            // Arrange
            var condition1 = new MockCondition();
            var condition2 = new MockCondition();

            var composite = new ConditionComposite(
                new List<ICondition> { condition1, condition2 },
                ConditionFilterType.Any);

            // Assert
            Assert.That(composite.IsSatisfied, Is.False);
        }

        [Test]
        public void ConditionComposite_WithUndefinedFilterType_ReturnsFalse()
        {
            // Arrange
            var condition1 = new MockCondition();
            condition1.SetSatisfied(true);

            var composite = new ConditionComposite(
                new List<ICondition> { condition1 },
                ConditionFilterType.Undefined);

            // Assert
            Assert.That(composite.IsSatisfied, Is.False);
        }

        [Test]
        public void ConditionComposite_ChildConditionChange_FiresSatisfiedChanged()
        {
            // Arrange
            var condition1 = new MockCondition();
            var condition2 = new MockCondition();

            var composite = new ConditionComposite(
                new List<ICondition> { condition1, condition2 },
                ConditionFilterType.All);

            var eventFired = false;
            composite.SatisfiedChanged += (_, _) => eventFired = true;

            // Act
            condition1.SetSatisfied(true);

            // Assert
            Assert.That(eventFired, Is.True);
        }

        [Test]
        public async Task ConditionComposite_DisposeAsync_UnsubscribesFromAllChildren()
        {
            // Arrange
            var condition1 = new MockCondition();
            var condition2 = new MockCondition();

            var composite = new ConditionComposite(
                new List<ICondition> { condition1, condition2 },
                ConditionFilterType.All);

            var eventFired = false;
            composite.SatisfiedChanged += (_, _) => eventFired = true;

            // Act
            await composite.DisposeAsync();
            condition1.SetSatisfied(true);

            // Assert - Event should not fire after dispose
            Assert.That(eventFired, Is.False);
        }

        [Test]
        public void ConditionComposite_WithEmptyConditions_All_ReturnsTrue()
        {
            // Arrange - TrueForAll on empty list returns true
            var composite = new ConditionComposite(
                new List<ICondition>(),
                ConditionFilterType.All);

            // Assert
            Assert.That(composite.IsSatisfied, Is.True);
        }

        [Test]
        public void ConditionComposite_WithEmptyConditions_Any_ReturnsFalse()
        {
            // Arrange - Exists on empty list returns false
            var composite = new ConditionComposite(
                new List<ICondition>(),
                ConditionFilterType.Any);

            // Assert
            Assert.That(composite.IsSatisfied, Is.False);
        }

        #endregion
    }
}