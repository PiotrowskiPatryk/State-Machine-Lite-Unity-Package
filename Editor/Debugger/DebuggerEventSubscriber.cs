using Dev.Cortez.StateMachines.Core.Data;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Core.Registry;

namespace Dev.Cortez.StateMachines.Editor.Debugger
{
    public sealed class DebuggerEventSubscriber
    {
        private readonly DebuggerHistoryManager _historyManager;

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

            foreach (var trigger in containerEntry.Triggers.Values)
            {
                trigger.TriggeredValueChanged += OnTriggerValueChanged;
            }

            foreach (var stateMachine in containerEntry.StateMachines.Values)
            {
                foreach (var state in stateMachine.States)
                {
                    state.StatusChanged += status => OnStateStatusChanged(stateMachine, state, status);
                }

                foreach (var rule in stateMachine.TransitionRules)
                {
                    var transitionRule = rule;
                    rule.Condition.SatisfiedChanged += (_, satisfied) => OnConditionChanged(transitionRule, satisfied);
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