using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Data;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Logging;
using JetBrains.Annotations;

namespace Dev.Cortez.StateMachines.Core.Factories
{
    public static class StateMachineFactory
    {
        [ItemCanBeNull]
        public static async UniTask<IStateMachine> CreateAsync([CanBeNull] StateMachineSettings settings, [CanBeNull] IPayload payload, CancellationToken cancellationToken)
        {
            using var linkedCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            
            if (settings == null)
            {
                LoggerService.Logger.LogError("Unable to create state machine. Configuration data is null.");
                return null;
            }

            if (settings.InitialState == null)
            {
                LoggerService.Logger.LogError("Unable to create state machine. Initial state is null.");
                return null;
            }
            
            var isStateMachineType = settings.Type.IsAssignableFrom(typeof(IStateMachine));

            if (!isStateMachineType)
            {
                LoggerService.Logger.LogError("Unable to create state machine. State machine type is not assignable from IStateMachine.");
                return null;           
            }

            if (Activator.CreateInstance(settings.Type) is not IStateMachine stateMachineInstance)
            {
                LoggerService.Logger.LogError("An error occured during creating a state machine instance.");
                return null;
            }

            payload ??= new EmptyPayload();
            var isInitializedProperly = await stateMachineInstance.InitializeAsync(settings, payload, linkedCancellationToken.Token);

            if (isInitializedProperly)
            {
                return stateMachineInstance;
            }

            LoggerService.Logger.LogError("Unable to initialize state machine. Initialization failed.");
            await stateMachineInstance.DisposeAsync();
            return null;
        }
    }
}