using System.Threading;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Abstraction;
using Dev.Cortez.StateMachines.Core.Data;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Samples.GameScenario.Scripts.StateMachines.Key.Context;

namespace Samples.GameScenario.Scripts.StateMachines.Key.States
{
    public abstract class KeyStateBase : KeyStateBase<EmptyPayload>
    {
        protected override UniTask<bool> DoInitializeAsync(EmptyPayload payload, CancellationToken cancellationToken)
        {
            return UniTask.FromResult(true);
        }
    }

    public abstract class KeyStateBase<TStatePayload> : StateBase<KeyContext, TStatePayload>, IKeyState
        where TStatePayload : IPayload
    {
    }
}