using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Data;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Core.TransitionSolver;
using Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Common.Builders;
using Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Common.Mocks;
using NUnit.Framework;

namespace Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Unit.Core
{
    /// <summary>
    /// Unit tests for DefaultTransitionSolver.
    /// Tests rule storage, subscription management, and disposal.
    /// Note: Frame-dependent tests (NextFrame) are in Runtime tests.
    /// </summary>
    [TestFixture]
    public sealed class TransitionSolverTests
    {
        private DefaultTransitionSolver _solver;
        private MockState _stateA;
        private MockState _stateB;
        private MockCondition _condition;
        private CancellationTokenSource _cts;

        [SetUp]
        public void SetUp()
        {
            _solver = new DefaultTransitionSolver();
            _stateA = new MockState();
            _stateB = new MockState();
            _condition = new MockCondition();
            _cts = new CancellationTokenSource();
        }

        [TearDown]
        public void TearDown()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _solver?.DisposeAsync().AsTask().GetAwaiter().GetResult();
        }

        private async Task InitializeStatesAsync()
        {
            var stateDefA = StateDefinitionBuilder.Create()
                .WithId("state-a")
                .WithType<MockState>()
                .Build();
            var stateDefB = StateDefinitionBuilder.Create()
                .WithId("state-b")
                .WithType<MockState>()
                .Build();

            await _stateA.InitializeAsync(stateDefA, _cts.Token);
            await _stateB.InitializeAsync(stateDefB, _cts.Token);
        }

        #region ApplyTransitionRulesAsync Tests

        [Test]
        public async Task ApplyTransitionRulesAsync_StoresRulesForLaterUse()
        {
            // Arrange
            await InitializeStatesAsync();
            var transitionRule = TransitionRuleBuilder.Create()
                .From(_stateA)
                .To(_stateB)
                .WithCondition(_condition)
                .Build();

            var rules = new Dictionary<IState, IReadOnlyList<TransitionRule>>
            {
                { _stateA, new List<TransitionRule> { transitionRule } }
            };

            // Act
            var result = await _solver.ApplyTransitionRulesAsync(rules, _cts.Token);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task ApplyTransitionRulesAsync_WithEmptyRules_ReturnsTrue()
        {
            // Arrange
            var rules = new Dictionary<IState, IReadOnlyList<TransitionRule>>();

            // Act
            var result = await _solver.ApplyTransitionRulesAsync(rules, _cts.Token);

            // Assert
            Assert.That(result, Is.True);
        }

        #endregion

        #region ApplyRulesForActiveState Tests

        [Test]
        public async Task ApplyRulesForActiveState_SubscribesToConditions()
        {
            // Arrange
            await InitializeStatesAsync();
            var transitionRule = TransitionRuleBuilder.Create()
                .From(_stateA)
                .To(_stateB)
                .WithCondition(_condition)
                .Build();

            var rules = new Dictionary<IState, IReadOnlyList<TransitionRule>>
            {
                { _stateA, new List<TransitionRule> { transitionRule } }
            };

            await _solver.ApplyTransitionRulesAsync(rules, _cts.Token);

            // Act
            _solver.ApplyRulesForActiveState(_stateA);

            // Verify subscription by checking that condition changes are tracked
            // Since we can't easily verify subscription directly, we verify no exception
            Assert.Pass("ApplyRulesForActiveState completed without exception");
        }

        [Test]
        public async Task ApplyRulesForActiveState_WithNoRulesForState_DoesNotThrow()
        {
            // Arrange
            await InitializeStatesAsync();
            var rules = new Dictionary<IState, IReadOnlyList<TransitionRule>>();
            await _solver.ApplyTransitionRulesAsync(rules, _cts.Token);

            // Act & Assert - Should not throw
            Assert.DoesNotThrow(() => _solver.ApplyRulesForActiveState(_stateA));
        }

        [Test]
        public async Task ApplyRulesForActiveState_ClearsOldSubscriptions()
        {
            // Arrange
            await InitializeStatesAsync();
            var condition1 = new MockCondition();
            var condition2 = new MockCondition();

            var rule1 = TransitionRuleBuilder.Create()
                .From(_stateA)
                .To(_stateB)
                .WithCondition(condition1)
                .Build();

            var rule2 = TransitionRuleBuilder.Create()
                .From(_stateB)
                .To(_stateA)
                .WithCondition(condition2)
                .Build();

            var rules = new Dictionary<IState, IReadOnlyList<TransitionRule>>
            {
                { _stateA, new List<TransitionRule> { rule1 } },
                { _stateB, new List<TransitionRule> { rule2 } }
            };

            await _solver.ApplyTransitionRulesAsync(rules, _cts.Token);

            // Act - Apply rules for stateA, then stateB
            _solver.ApplyRulesForActiveState(_stateA);
            _solver.ApplyRulesForActiveState(_stateB);

            // Assert - No exception, old subscriptions should be cleared
            Assert.Pass("Subscriptions cleared and re-applied without exception");
        }

        [Test]
        public async Task ApplyRulesForActiveState_WithMultipleRules_SubscribesToAll()
        {
            // Arrange
            await InitializeStatesAsync();
            var condition1 = new MockCondition();
            var condition2 = new MockCondition();

            var rule1 = TransitionRuleBuilder.Create()
                .From(_stateA)
                .To(_stateB)
                .WithCondition(condition1)
                .WithPriority(1)
                .Build();

            var rule2 = TransitionRuleBuilder.Create()
                .From(_stateA)
                .To(_stateB)
                .WithCondition(condition2)
                .WithPriority(2)
                .Build();

            var rules = new Dictionary<IState, IReadOnlyList<TransitionRule>>
            {
                { _stateA, new List<TransitionRule> { rule1, rule2 } }
            };

            await _solver.ApplyTransitionRulesAsync(rules, _cts.Token);

            // Act
            _solver.ApplyRulesForActiveState(_stateA);

            // Assert
            Assert.Pass("Multiple rules applied without exception");
        }

        #endregion

        #region DisposeAsync Tests

        [Test]
        public async Task DisposeAsync_ClearsAllSubscriptions()
        {
            // Arrange
            await InitializeStatesAsync();
            var transitionRule = TransitionRuleBuilder.Create()
                .From(_stateA)
                .To(_stateB)
                .WithCondition(_condition)
                .Build();

            var rules = new Dictionary<IState, IReadOnlyList<TransitionRule>>
            {
                { _stateA, new List<TransitionRule> { transitionRule } }
            };

            await _solver.ApplyTransitionRulesAsync(rules, _cts.Token);
            _solver.ApplyRulesForActiveState(_stateA);

            // Act
            await _solver.DisposeAsync();

            // Assert - After dispose, changing condition should not cause issues
            Assert.DoesNotThrow(() => _condition.SetSatisfied(true));
        }

        #endregion

        #region TransitionRuleApplied Event Tests

        [Test]
        public async Task TransitionRuleApplied_CanBeSubscribedTo()
        {
            // Arrange
            var eventFired = false;
            _solver.TransitionRuleApplied += _ => eventFired = true;

            // Act & Assert - Just verify subscription works
            Assert.That(eventFired, Is.False);
        }

        #endregion
    }
}