using System.Collections.Generic;
using Dev.Cortez.StateMachines.Core.Interfaces;

namespace Dev.Cortez.StateMachines.Core.Registry
{
    public class StateMachineContainerRegistry
    {
        private readonly HashSet<IStateMachineContainerEntry> _entries = new();
        private readonly ObserverTopic<IStateMachine> _stateMachineTopic = new();
        private readonly ObserverTopic<IState> _stateTopic = new();
        private readonly ObserverTopic<ITrigger> _triggerTopic = new();

        public static StateMachineContainerRegistry Instance { get; } = new();

        public void SubscribeStateMachine(IdentifiableObserver<IStateMachine> observer)
        {
            _stateMachineTopic.Subscribe(observer);
        }

        public void UnsubscribeStateMachine(IdentifiableObserver<IStateMachine> observer)
        {
            _stateMachineTopic.Unsubscribe(observer);
        }

        public void SubscribeState(IdentifiableObserver<IState> observer)
        {
            _stateTopic.Subscribe(observer);
        }

        public void UnsubscribeState(IdentifiableObserver<IState> observer)
        {
            _stateTopic.Unsubscribe(observer);
        }

        public void SubscribeTrigger(IdentifiableObserver<ITrigger> observer)
        {
            _triggerTopic.Subscribe(observer);
        }

        public void UnsubscribeTrigger(IdentifiableObserver<ITrigger> observer)
        {
            _triggerTopic.Unsubscribe(observer);
        }

        public void RegisterStateMachineContainer(IStateMachineContainerEntry entry)
        {
            if (!_entries.Add(entry))
            {
                return;
            }

            foreach (var stateMachine in entry.StateMachines)
            {
                _stateMachineTopic.AddItem(stateMachine.Value);

                foreach (var state in stateMachine.Value.States)
                {
                    _stateTopic.AddItem(state);
                }
            }

            foreach (var trigger in entry.Triggers)
            {
                _triggerTopic.AddItem(trigger.Value);
            }
        }

        public void UnregisterStateMachineContainer(IStateMachineContainerEntry entry)
        {
            if (!_entries.Contains(entry))
            {
                return;
            }

            foreach (var stateMachine in entry.StateMachines)
            {
                _stateMachineTopic.RemoveItem(stateMachine.Value);

                foreach (var state in stateMachine.Value.States)
                {
                    _stateTopic.RemoveItem(state);
                }
            }

            foreach (var trigger in entry.Triggers)
            {
                _triggerTopic.RemoveItem(trigger.Value);
            }

            _entries.Remove(entry);
        }
    }
}