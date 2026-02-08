using System.Threading;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Abstraction;
using Dev.Cortez.StateMachines.Core.Data;
using Dev.Cortez.StateMachines.Core.Interfaces;

namespace Samples.GameScenario.Scripts.StateMachines.Safe
{
    /// <summary>
    /// Base class for Safe states that don't require initialization payload.
    /// </summary>
    public abstract class SafeStateBase : SafeStateBase<EmptyPayload>
    {
        protected override UniTask<bool> DoInitializeAsync(EmptyPayload payload,
            CancellationToken cancellationToken)
        {
            return UniTask.FromResult(true);
        }
    }

    /// <summary>
    /// Base class for Safe states with a specific payload type.
    /// All SafeStateBase&lt;T&gt; classes implement ISafeState for common type matching.
    /// </summary>
    public abstract class SafeStateBase<TStatePayload> : StateBase<StateContext, TStatePayload>, ISafeState
        where TStatePayload : IPayload
    {
    }
}