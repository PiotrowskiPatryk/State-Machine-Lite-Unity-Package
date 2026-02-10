using System.Threading;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Abstraction;
using Dev.Cortez.StateMachines.Core.Data;
using JetBrains.Annotations;
using Samples.GameScenario.Scripts.Key;
using Samples.GameScenario.Scripts.StateMachines.Key.Context;
using Samples.GameScenario.Scripts.StateMachines.Key.States;

namespace Samples.GameScenario.Scripts.StateMachines.Key.StateMachine
{
    [UsedImplicitly]
    public class KeyStateMachine : StateMachineBase<IKeyState, KeyContext>
    {
        private KeyContext _keyContext;

        protected override KeyContext StateContext => _keyContext;

        protected override UniTask<bool> DoInitializeStateMachineAsync(EmptyPayload stateMachinePayload,
            CancellationToken cancellationToken)
        {
            _keyContext = new KeyContext(KeyController.Instance);

            return UniTask.FromResult(true);
        }
    }
}