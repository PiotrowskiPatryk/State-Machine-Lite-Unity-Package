using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Factories;
using Dev.Cortez.StateMachines.Core.StateMachineConfiguration;
using Dev.Cortez.StateMachines.Logging;
using JetBrains.Annotations;

namespace Dev.Cortez.StateMachines.Core.Registry
{
    public class StateMachineContainerInstaller
    {
        [ItemCanBeNull]
        public async UniTask<IStateMachineContainerRegistry> InstallAsync(StateMachineContainer stateMachineContainer,
            CancellationToken cancellationToken)
        {
            using var linkedCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            var stateMachineContainerRegistry = new StateMachineContainerRegistry(Guid.NewGuid().ToString("N"));
            var triggersDefinitions = stateMachineContainer.TriggerConfiguration.Triggers;
            var stateMachineDefinitions = stateMachineContainer.StateMachineConfiguration.StateMachines;

            var triggers = await triggersDefinitions.
                Select(triggerDefinition =>
                    StateMachineFactory.CreateTriggerAsync(triggerDefinition, linkedCancellationToken.Token));
            var stateMachines = await stateMachineDefinitions.Select(stateMachine =>
                StateMachineFactory.CreateStateMachineAsync(stateMachine, linkedCancellationToken.Token));

            var triggersRegistered = TryRegisterTriggers(triggers, stateMachineContainerRegistry);

            if (!triggersRegistered)
            {
                LoggerService.Logger.LogError(
                    "Unable to install state machine container registry. Triggers are not properly registered.");

                return null;
            }

            var stateMachinesRegistered = TryRegisterStateMachines(stateMachines, stateMachineContainerRegistry);

            if (!stateMachinesRegistered)
            {
                LoggerService.Logger.LogError(
                    "Unable to install state machine container registry. State machines are not properly registered.");

                return null;
            }

            return stateMachineContainerRegistry;
        }

        private static bool TryRegisterTriggers(ITrigger[] triggers,
            StateMachineContainerRegistry stateMachineContainerRegistry)
        {
            foreach (var trigger in triggers)
            {
                LoggerService.Logger.LogTrace($"Registering trigger [{trigger.Id} {trigger.Name}]");
                var registeredTrigger = stateMachineContainerRegistry.TryRegisterTrigger(trigger);

                if (registeredTrigger)
                {
                    LoggerService.Logger.LogInfo($"Trigger [{trigger.Id} {trigger.Name}] was successfully registered");
                }
                else
                {
                    LoggerService.Logger.LogError($"Trigger [{trigger.Id} {trigger.Name}] registration failed");

                    return false;
                }
            }

            return true;
        }

        private static bool TryRegisterStateMachines(IStateMachine[] stateMachines,
            StateMachineContainerRegistry stateMachineContainerRegistry)
        {
            foreach (var stateMachine in stateMachines)
            {
                LoggerService.Logger.LogTrace($"Registering State Machine [{stateMachine.Id} {stateMachine.Name}]");
                var registeredStateMachine = stateMachineContainerRegistry.TryRegisterStateMachine(stateMachine);

                if (registeredStateMachine)
                {
                    LoggerService.Logger.LogInfo(
                        $"State Machine [{stateMachine.Id} {stateMachine.Name}] was successfully registered");
                }
                else
                {
                    LoggerService.Logger.LogError(
                        $"State Machine [{stateMachine.Id} {stateMachine.Name}] registration failed");

                    return false;
                }
            }

            return true;
        }
    }
}