using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Data;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Logging;
using JetBrains.Annotations;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Core.TransitionSolver
{
    [UsedImplicitly]
    public sealed class DefaultTransitionSolver : ITransitionSolver
    {
        private readonly List<TransitionRule> _transitionRules = new();
        private readonly Dictionary<TransitionRule, int> _ruleOrder = new();
        private readonly Dictionary<TransitionRule, Action<ICondition, bool>> _ruleSubscriptions = new();
        private readonly List<TransitionRule> _frameSuccessCache = new();
        private readonly Dictionary<TransitionRule, bool> _lastKnownSatisfied = new();

        public event Action<TransitionRule> TransitionRuleApplied;

        private Dictionary<IState, IReadOnlyList<TransitionRule>> _cachedTransitionRules;
        private CancellationTokenSource _composeCts;
        private bool _composeScheduled;
        
        public UniTask<bool> ApplyTransitionRulesAsync(IReadOnlyDictionary<IState, IReadOnlyList<TransitionRule>> transitionRules, CancellationToken cancellationToken)
        {
            _cachedTransitionRules = new Dictionary<IState, IReadOnlyList<TransitionRule>>(transitionRules);

            return UniTask.FromResult(true);
        }

        public void ApplyRulesForActiveState(IState state)
        {
            LoggerService.Logger.LogInfo($"Applying transition solver rules for state: {state.Name}");
            
            DisposeOldTransitionRules();

            var transitionRules = _cachedTransitionRules?.GetValueOrDefault(state);

            if (transitionRules == null || transitionRules.Count == 0)
            {
                return;
            }

            var index = 0;

            foreach (var transitionRule in transitionRules)
            {
                var isSatisfiedNow = transitionRule.Condition.IsSatisfied;
                _lastKnownSatisfied[transitionRule] = isSatisfiedNow;

                SubscribeToTransitionRule(transitionRule);
                _transitionRules.Add(transitionRule);
                _ruleOrder[transitionRule] = index++;

                if (!isSatisfiedNow)
                {
                    continue;
                }

                if (!_frameSuccessCache.Contains(transitionRule))
                {
                    _frameSuccessCache.Add(transitionRule);
                }

                if (_composeScheduled)
                {
                    continue;
                }

                _composeScheduled = true;
                _composeCts ??= new CancellationTokenSource();
                ComposeOnceAsync(_composeCts.Token).Forget();
            }
        }

        public ValueTask DisposeAsync()
        {
            DisposeOldTransitionRules();

            return default;
        }

        private void SubscribeToTransitionRule(TransitionRule transitionRule)
        {
            transitionRule.Condition.SatisfiedChanged += ConditionRuleSatisfiedChanged;
            _ruleSubscriptions[transitionRule] = ConditionRuleSatisfiedChanged;

            return;

            void ConditionRuleSatisfiedChanged(ICondition condition, bool isSatisfied)
            {
                if (!_ruleOrder.ContainsKey(transitionRule))
                {
                    return;
                }

                var hadValue = _lastKnownSatisfied.TryGetValue(transitionRule, out var wasSatisfied);

                if (!hadValue || wasSatisfied == isSatisfied)
                {
                    _lastKnownSatisfied[transitionRule] = isSatisfied;

                    return;
                }

                _lastKnownSatisfied[transitionRule] = isSatisfied;

                if (!isSatisfied)
                {
                    return;
                }

                if (!_frameSuccessCache.Contains(transitionRule))
                {
                    _frameSuccessCache.Add(transitionRule);
                }

                if (_composeScheduled)
                {
                    return;
                }

                _composeScheduled = true;
                _composeCts ??= new CancellationTokenSource();
                ComposeOnceAsync(_composeCts.Token).Forget();
            }
        }

        private async UniTask ComposeOnceAsync(CancellationToken cancellationToken)
        {
            try
            {
                await UniTask.NextFrame(cancellationToken);

                TransitionRule selected = null;

                if (_frameSuccessCache.Count > 0)
                {
                    _frameSuccessCache.Sort((firstTransitionRule, secondTransitionRule) =>
                    {
                        var priority = secondTransitionRule.Priority.CompareTo(firstTransitionRule.Priority);

                        if (priority != 0)
                        {
                            return priority;
                        }

                        var aIdx = _ruleOrder[firstTransitionRule];
                        var bIdx = _ruleOrder[secondTransitionRule];

                        return aIdx.CompareTo(bIdx);
                    });

                    selected = _frameSuccessCache[0];
                }

                _frameSuccessCache.Clear();
                _composeScheduled = false;

                if (selected != null)
                {
                    TransitionRuleApplied?.Invoke(selected);
                }
            }
            catch (OperationCanceledException)
            {
                LoggerService.Logger.LogWarning("Transition composition was cancelled.");
            }
            finally
            {
                if (!_composeScheduled)
                {
                    _composeCts?.Dispose();
                    _composeCts = null;
                }
            }
        }

        private void DisposeOldTransitionRules()
        {
            if (_composeCts is { IsCancellationRequested: false, })
            {
                _composeCts.Cancel();
            }

            foreach (var (rule, handler) in _ruleSubscriptions)
            {
                rule.Condition.SatisfiedChanged -= handler;
            }

            _ruleSubscriptions.Clear();
            _transitionRules.Clear();
            _ruleOrder.Clear();
            _frameSuccessCache.Clear();
            _lastKnownSatisfied.Clear();
            _composeScheduled = false;

            _composeCts?.Dispose();
            _composeCts = null;
        }
    }
}
