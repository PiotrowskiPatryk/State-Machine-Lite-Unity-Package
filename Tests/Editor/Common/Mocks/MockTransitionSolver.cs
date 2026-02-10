using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Attributes;
using Dev.Cortez.StateMachines.Core.Data;
using Dev.Cortez.StateMachines.Core.Interfaces;

namespace Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Common.Mocks
{
    /// <summary>
    /// A controllable ITransitionSolver implementation for testing purposes.
    /// Records all method calls and allows manual triggering of events.
    /// </summary>
    [ExcludeFromTypePicker]
    public sealed class MockTransitionSolver : ITransitionSolver
    {
        public event Action<TransitionRule> TransitionRuleApplied;

        /// <summary>
        /// Records all transition rules that were applied via ApplyTransitionRulesAsync.
        /// </summary>
        public IReadOnlyDictionary<IState, IReadOnlyList<TransitionRule>> AppliedTransitionRules { get; private set; }

        /// <summary>
        /// Records all states for which ApplyRulesForActiveState was called.
        /// </summary>
        public List<IState> ApplyRulesForActiveStateCalls { get; } = new();

        /// <summary>
        /// Configures whether ApplyTransitionRulesAsync should succeed.
        /// </summary>
        public bool ApplyTransitionRulesShouldSucceed { get; set; } = true;

        /// <summary>
        /// Tracks the number of times DisposeAsync was called.
        /// </summary>
        public int DisposeCallCount { get; private set; }

        /// <summary>
        /// Tracks the number of times ApplyTransitionRulesAsync was called.
        /// </summary>
        public int ApplyTransitionRulesCallCount { get; private set; }

        public UniTask<bool> ApplyTransitionRulesAsync(
            IReadOnlyDictionary<IState, IReadOnlyList<TransitionRule>> transitionRules,
            CancellationToken cancellationToken)
        {
            ApplyTransitionRulesCallCount++;
            AppliedTransitionRules = transitionRules;

            return UniTask.FromResult(ApplyTransitionRulesShouldSucceed);
        }

        public void ApplyRulesForActiveState(IState state)
        {
            ApplyRulesForActiveStateCalls.Add(state);
        }

        /// <summary>
        /// Manually triggers the TransitionRuleApplied event for testing.
        /// </summary>
        public void TriggerTransitionRuleApplied(TransitionRule rule)
        {
            TransitionRuleApplied?.Invoke(rule);
        }

        public ValueTask DisposeAsync()
        {
            DisposeCallCount++;
            TransitionRuleApplied = null;

            return default;
        }

        /// <summary>
        /// Resets all tracking counters and state.
        /// </summary>
        public void Reset()
        {
            AppliedTransitionRules = null;
            ApplyRulesForActiveStateCalls.Clear();
            ApplyTransitionRulesShouldSucceed = true;
            DisposeCallCount = 0;
            ApplyTransitionRulesCallCount = 0;
        }
    }
}