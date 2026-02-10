using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Registry;
using Dev.Cortez.StateMachines.Core.StateMachineConfiguration;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Core.Mono
{
    [DefaultExecutionOrder(1000)]
    public class StateMachineContainerInstallerMono : MonoBehaviour
    {
        private readonly StateMachineContainerInstaller _stateMachineContainerInstaller = new();

        public event Action<IStateMachineContainerEntry> InstallationCompleted;
        public event Action<IStateMachineContainerEntry> StateMachinesActivated;

        [SerializeField]
        private StateMachineContainer _stateMachineContainer;

        [SerializeField]
        private bool _installOnAwake;

        [SerializeField]
        private bool _activateStateMachinesAfterInstallation;

        private bool _isInstalled;

        public IStateMachineContainerEntry StateMachineContainerEntry { get; private set; }

        public async UniTask<IStateMachineContainerEntry> InstallAsync(CancellationToken cancellationToken)
        {
            if (_isInstalled)
            {
                Debug.LogWarning("State machine container is already installed");

                return StateMachineContainerEntry;
            }

            await InstallStateMachineContainerAsync();

            if (_activateStateMachinesAfterInstallation)
            {
                await ActivateStateMachinesAsync(cancellationToken);
            }

            return StateMachineContainerEntry;
        }

        private async UniTask InstallStateMachineContainerAsync()
        {
            StateMachineContainerEntry = await _stateMachineContainerInstaller.InstallAsync(_stateMachineContainer,
                destroyCancellationToken);

            _isInstalled = true;

            InstallationCompleted?.Invoke(StateMachineContainerEntry);
        }

        private async UniTask ActivateStateMachinesAsync(CancellationToken cancellationToken)
        {
            foreach (var stateMachine in StateMachineContainerEntry.StateMachines)
            {
                await stateMachine.Value.ActivateAsync(cancellationToken);
            }

            StateMachinesActivated?.Invoke(StateMachineContainerEntry);
        }

#pragma warning disable S3168
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

        private void OnDestroy()
        {
            if (_stateMachineContainerInstaller != null && StateMachineContainerEntry != null)
            {
                _stateMachineContainerInstaller.UninstallAsync(StateMachineContainerEntry).Forget();
                StateMachineContainerEntry = null;
            }
        }
    }
#pragma warning restore S3168
}