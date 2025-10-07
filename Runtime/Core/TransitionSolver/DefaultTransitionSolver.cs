using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Data;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Core.TransitionSolver
{
    public class DefaultTransitionSolver : ITransitionSolver
    {
        private readonly List<TransitionRule> _transitionRules = new();
        private readonly Dictionary<TransitionRule, int> _ruleOrder = new();
        private readonly Dictionary<TransitionRule, Action<bool>> _ruleSubscriptions = new();
        private readonly List<TransitionRule> _frameSuccessCache = new();
        private readonly Dictionary<TransitionRule, bool> _lastKnownSatisfied = new();

        public event Action<TransitionRule> TransitionRuleApplied;

        private CancellationTokenSource _composeCts;
        private bool _composeScheduled;

        private bool IsComposing { get; set; }

        public void SetupNewRules(IReadOnlyCollection<TransitionRule> transitionRules)
        {
            DisposeOldTransitionRules();

            if (transitionRules == null || transitionRules.Count == 0)
            {
                return;
            }

            var index = 0;

            foreach (var rule in transitionRules)
            {
                var isSatisfiedNow = rule.Condition.IsSatisfied;
                _lastKnownSatisfied[rule] = isSatisfiedNow;

                SubscribeToTransitionRule(rule);
                _transitionRules.Add(rule);
                _ruleOrder[rule] = index++;

                if (isSatisfiedNow)
                {
                    if (!_frameSuccessCache.Contains(rule))
                    {
                        _frameSuccessCache.Add(rule);
                    }

                    if (!_composeScheduled)
                    {
                        _composeScheduled = true;
                        _composeCts ??= new CancellationTokenSource();
                        ComposeOnceAsync(_composeCts.Token).Forget();
                    }
                }
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

            void ConditionRuleSatisfiedChanged(bool isSatisfied)
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
                IsComposing = true;

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
                Debug.LogWarning("Transition composition was cancelled.");
            }
            finally
            {
                IsComposing = false;

                if (!_composeScheduled)
                {
                    _composeCts?.Dispose();
                    _composeCts = null;
                }
            }
        }

        private void DisposeOldTransitionRules()
        {
            if (_composeCts is { IsCancellationRequested: false })
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
            IsComposing = false;

            _composeCts?.Dispose();
            _composeCts = null;
        }
    }
}