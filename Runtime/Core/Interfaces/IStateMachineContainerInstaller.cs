#region

using System.Threading;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Registry;
using Dev.Cortez.StateMachines.Core.StateMachineConfiguration;
using JetBrains.Annotations;

#endregion

namespace Dev.Cortez.StateMachines.Core.Interfaces
{
    public interface IStateMachineContainerInstaller
    {
        UniTask<IStateMachineContainerEntry> InstallAsync(StateMachineContainer stateMachineContainer,
            CancellationToken cancellationToken);

        UniTask UninstallAsync([NotNull] IStateMachineContainerEntry stateMachineContainerEntry);
    }
}