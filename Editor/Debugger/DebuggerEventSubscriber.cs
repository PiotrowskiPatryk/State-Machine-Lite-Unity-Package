using System;
using System.Collections.Generic;
using System.Linq;
using Dev.Cortez.StateMachines.Core.Data;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Core.Registry;

namespace Dev.Cortez.StateMachines.Editor.Debugger
{
    public sealed class DebuggerEventSubscriber
    {
        private readonly DebuggerHistoryManager _historyManager;
        private readonly Dictionary<IState, Action<StateStatus>> _stateHandlers = new();
        private readonly Dictionary<ICondition, Action<ICondition, bool>> _conditionHandlers = new();

        private IStateMachineContainerEntry _containerEntry;

        public bool IsSubscribed { get; private set; }
        public IStateMachineContainerEntry SubscribedContainerEntry => _containerEntry;

        public DebuggerEventSubscriber(DebuggerHistoryManager historyManager)
        {
            _historyManager = historyManager;
        }

        public void Subscribe(IStateMachineContainerEntry containerEntry)
        {
            if (IsSubscribed || containerEntry == null)
            {
                return;
            }

            _containerEntry = containerEntry;
            _stateHandlers.Clear();
            _conditionHandlers.Clear();

            foreach (var trigger in containerEntry.Triggers.Values)
            {
                trigger.TriggeredValueChanged += OnTriggerValueChanged;
            }

            foreach (var stateMachine in containerEntry.StateMachines.Values)
            {
                foreach (var state in stateMachine.States)
                {
                    Action<StateStatus> stateHandler = status => OnStateStatusChanged(stateMachine, state, status);
                    _stateHandlers[state] = stateHandler;
                    state.StatusChanged += stateHandler;
                }

                foreach (var rule in stateMachine.TransitionRules)
                {
                    var transitionRule = rule;
                    Action<ICondition, bool> conditionHandler = (_, satisfied) => OnConditionChanged(transitionRule, satisfied);
                    _conditionHandlers[rule.Condition] = conditionHandler;
                    rule.Condition.SatisfiedChanged += conditionHandler;
                }
            }

            IsSubscribed = true;
        }

        public void Unsubscribe()
        {
            if (!IsSubscribed || _containerEntry == null)
            {
                return;
            }

            foreach (var trigger in _containerEntry.Triggers.Values)
            {
                trigger.TriggeredValueChanged -= OnTriggerValueChanged;
            }

            foreach (var stateMachine in _containerEntry.StateMachines.Values)
            {
                foreach (var state in stateMachine.States)
                {
                    if (_stateHandlers.TryGetValue(state, out var stateHandler))
                    {
                        state.StatusChanged -= stateHandler;
                    }
                }

                foreach (var condition in stateMachine.TransitionRules.Select(rule => rule.Condition))
                {
                    if (_conditionHandlers.TryGetValue(condition, out var conditionHandler))
                    {
                        condition.SatisfiedChanged -= conditionHandler;
                    }
                }
            }

            _stateHandlers.Clear();
            _conditionHandlers.Clear();
            _containerEntry = null;
            IsSubscribed = false;
        }

        private void OnTriggerValueChanged(ITrigger trigger, bool value)
        {
            _historyManager.Log("Trigger", trigger.Name, $"Value changed to {value}");
        }

        private void OnStateStatusChanged(IStateMachine stateMachine, IState state, StateStatus status)
        {
            _historyManager.Log("State", state.Name, $"Status: {status}");
            _historyManager.Log("StateMachine", stateMachine.Name, $"State '{state.Name}' → {status}");
        }

        private void OnConditionChanged(TransitionRule rule, bool satisfied)
        {
            var transitionKey = $"{rule.CurrentState.Name}->{rule.TargetState.Name}";
            _historyManager.Log("Condition", transitionKey, satisfied ? "Satisfied" : "Not Satisfied");
        }
    }
}
