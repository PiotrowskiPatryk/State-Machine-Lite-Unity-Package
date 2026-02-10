using System.Threading;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Abstraction;
using Dev.Cortez.StateMachines.Core.Data;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Samples.GameScenario.Scripts.StateMachines.Safe.Context;

namespace Samples.GameScenario.Scripts.StateMachines.Safe.States
{
    public abstract class SafeStateBase : SafeStateBase<EmptyPayload>
    {
        protected override UniTask<bool> DoInitializeAsync(EmptyPayload payload,
            CancellationToken cancellationToken)
        {
            return UniTask.FromResult(true);
        }
    }

    public abstract class SafeStateBase<TStatePayload> : StateBase<StateContext, TStatePayload>, ISafeState
        where TStatePayload : IPayload
    {
    }
}