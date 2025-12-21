using System.Collections.Concurrent;
using System.Collections.Generic;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Logging;
using JetBrains.Annotations;

namespace Dev.Cortez.StateMachines.Core.Registry
{
    public sealed class StateMachineContainerEntry : IStateMachineContainerEntry
    {
        private readonly ConcurrentDictionary<string, ITrigger> _triggers = new();
        private readonly ConcurrentDictionary<string, IStateMachine> _stateMachines = new();

        public IReadOnlyDictionary<string, ITrigger> Triggers => _triggers;
        public IReadOnlyDictionary<string, IStateMachine> StateMachines => _stateMachines;
        
        public string Id { get; }

        public StateMachineContainerEntry([NotNull] string id)
        {
            Id = id;
        }

        public bool TryRegisterTrigger([NotNull] ITrigger trigger)
        {
            LoggerService.Logger.LogTrace($"Registering trigger [{trigger.Id} {trigger.Name}]");
            
            if (!string.IsNullOrWhiteSpace(trigger.Id))
            {
                return _triggers.TryAdd(trigger.Id, trigger);
            }

            LoggerService.Logger.LogError("Unable to add trigger. Provided id cannot be null");

            return false;
        }

        public bool TryRegisterStateMachine([NotNull] IStateMachine stateMachine)
        {
            LoggerService.Logger.LogTrace($"Registering state machine [{stateMachine.Id} {stateMachine.Name}]");
            
            return _stateMachines.TryAdd(stateMachine.Id, stateMachine);
        }
    }
}