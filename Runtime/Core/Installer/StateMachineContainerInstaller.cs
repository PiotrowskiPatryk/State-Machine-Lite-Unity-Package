#region

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Factories;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Core.Registry;
using Dev.Cortez.StateMachines.Core.StateMachineConfiguration;
using Dev.Cortez.StateMachines.Logging;
using JetBrains.Annotations;

#endregion

namespace Dev.Cortez.StateMachines.Core.Installer
{
    public class StateMachineContainerInstaller : IStateMachineContainerInstaller
    {
        private readonly IStateMachineFactory _stateMachineFactory;

        public StateMachineContainerInstaller(IStateMachineFactory stateMachineFactory)
        {
            _stateMachineFactory = stateMachineFactory;
        }

        [ItemCanBeNull]
        public async UniTask<IStateMachineContainerEntry> InstallAsync(StateMachineContainer stateMachineContainer,
            CancellationToken cancellationToken)
        {
            LoggerService.Logger.LogInfo($"Installing state machine container [{stateMachineContainer.name}]");

            using var linkedCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            var stateMachineContainerEntry = new StateMachineContainerEntry(Guid.NewGuid().ToString("N"));
            LoggerService.Logger.LogInfo($"Creating state machine container registry {stateMachineContainerEntry.Id}");

            var triggersDefinitions = stateMachineContainer.TriggerConfiguration.Triggers;

            LoggerService.Logger.LogInfo(
                $"Creating triggers for state machine container registry {stateMachineContainerEntry.Id}");

            var triggers = await triggersDefinitions.
                Select(triggerDefinition =>
                    _stateMachineFactory.CreateTriggerAsync(triggerDefinition, linkedCancellationToken.Token));

            LoggerService.Logger.LogInfo(
                $"Creating state machines for state machine container registry {stateMachineContainerEntry.Id}");

            var stateMachineDefinitions = stateMachineContainer.StateMachineConfiguration.StateMachines;

            var stateMachines = await stateMachineDefinitions.Select(stateMachine =>
                _stateMachineFactory.CreateStateMachineAsync(stateMachine, linkedCancellationToken.Token));

            var triggersRegistered = TryRegisterTriggers(triggers, stateMachineContainerEntry);

            LoggerService.Logger.LogInfo(
                $"Registering triggers for state machine container registry {stateMachineContainerEntry.Id}");

            if (!triggersRegistered)
            {
                LoggerService.Logger.LogError(
                    "Unable to install state machine container registry. Triggers are not properly registered.");

                return null;
            }

            LoggerService.Logger.LogInfo(
                $"Registering state machines for state machine container registry {stateMachineContainerEntry.Id}");

            var stateMachinesRegistered = TryRegisterStateMachines(stateMachines, stateMachineContainerEntry);

            if (!stateMachinesRegistered)
            {
                LoggerService.Logger.LogError(
                    "Unable to install state machine container registry. State machines are not properly registered.");

                return null;
            }

            LoggerService.Logger.LogInfo(
                $"State machine container [{stateMachineContainer.name}] was successfully installed");

            StateMachineContainerRegistry.Instance.RegisterStateMachineContainer(stateMachineContainerEntry);

            return stateMachineContainerEntry;
        }

        public UniTask UninstallAsync([NotNull] IStateMachineContainerEntry stateMachineContainerEntry)
        {
            StateMachineContainerRegistry.Instance.UnregisterStateMachineContainer(stateMachineContainerEntry);

            return stateMachineContainerEntry.DisposeAsync().AsUniTask();
        }

        private static bool TryRegisterTriggers(ITrigger[] triggers,
            StateMachineContainerEntry stateMachineContainerEntry)
        {
            foreach (var trigger in triggers)
            {
                LoggerService.Logger.LogTrace($"Registering trigger [{trigger.Id} {trigger.Name}]");
                var registeredTrigger = stateMachineContainerEntry.TryRegisterTrigger(trigger);

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

            LoggerService.Logger.LogInfo("All triggers were successfully registered");

            return true;
        }

        private static bool TryRegisterStateMachines(IStateMachine[] stateMachines,
            StateMachineContainerEntry stateMachineContainerEntry)
        {
            foreach (var stateMachine in stateMachines)
            {
                LoggerService.Logger.LogTrace($"Registering State Machine [{stateMachine.Id} {stateMachine.Name}]");
                var registeredStateMachine = stateMachineContainerEntry.TryRegisterStateMachine(stateMachine);

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

            LoggerService.Logger.LogInfo("All state machines were successfully registered");

            return true;
        }
    }
}