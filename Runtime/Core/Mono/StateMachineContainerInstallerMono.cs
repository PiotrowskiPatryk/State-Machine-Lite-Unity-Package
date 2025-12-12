using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Registry;
using Dev.Cortez.StateMachines.Core.StateMachineConfiguration;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Core.Mono
{
    public class StateMachineContainerInstallerMono : MonoBehaviour
    {
        private readonly StateMachineContainerInstaller _stateMachineContainerInstaller = new();

        [SerializeField]
        private StateMachineContainer _stateMachineContainer;

        [SerializeField]
        private bool _installOnAwake;

        private bool _isInstalled;

        public async UniTask InstallAsync(CancellationToken cancellationToken)
        {
            if (_isInstalled)
            {
                Debug.LogWarning("State machine container is already installed");

                return;
            }

            var instance = await _stateMachineContainerInstaller.InstallAsync(_stateMachineContainer,
                destroyCancellationToken);

            _isInstalled = true;
        }

        private async void Awake()
        {
            try
            {
                if (_installOnAwake)
                {
                    await InstallAsync(destroyCancellationToken);
                }
            }
            catch (Exception exception)
            {
                Debug.LogError("An error occurred during installing state machine container");
                Debug.LogException(exception);
            }
        }
    }
}