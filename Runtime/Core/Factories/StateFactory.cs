using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Data;
using Dev.Cortez.StateMachines.Logging;
using JetBrains.Annotations;

namespace Dev.Cortez.StateMachines.Core.Factories
{
    public static class StateFactory
    {
        [ItemCanBeNull]
        public static async UniTask<IState> CreateAsync(StateSettings stateSettings, IPayload payload, CancellationToken cancellationToken)
        {
            using var linkedCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            
            if (stateSettings == null)
            {
                LoggerService.Logger.LogError("Unable to create state. Configuration data is null.");
                return null;
            }
            
            if (stateSettings.StateId == null)
            {
                LoggerService.Logger.LogError("Unable to create state. Provided state id is null.");
                return null;
            }
            
            if (stateSettings.StateType == null)
            {
                LoggerService.Logger.LogError("Unable to create state. Provided state type is null.");
                return null;
            }
            
            var isAssignableFromState = stateSettings.StateType.IsAssignableFrom(typeof(IState));

            if (!isAssignableFromState)
            {
                LoggerService.Logger.LogError("Unable to create state. State type is not assignable from IState.");
                return null;
            }
            
            var stateInstance = Activator.CreateInstance(stateSettings.StateType) as IState;

            if (stateInstance == null)
            {
                LoggerService.Logger.LogError("An error occured during creating a state instance.");
                return null;
            }
            
            payload ??= new EmptyPayload();
            var isInitialized = await stateInstance.InitializeAsync(stateSettings, payload, linkedCancellationToken.Token);

            if (isInitialized)
            {
                return stateInstance;
            }

            LoggerService.Logger.LogError("Unable to initialize state. Initialization failed.");
            await stateInstance.DisposeAsync();
            return null;
        }
    }
}
