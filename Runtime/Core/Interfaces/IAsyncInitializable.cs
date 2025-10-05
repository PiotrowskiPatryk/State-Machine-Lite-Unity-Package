using System.Threading;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Data;

namespace Dev.Cortez.StateMachines.Core
{
    public interface IAsyncInitializable
    {
        InitializationStatus InitializationStatus { get; }

        UniTask<bool> InitializeAsync(IPayload payload, CancellationToken cancellationToken);
    }
}