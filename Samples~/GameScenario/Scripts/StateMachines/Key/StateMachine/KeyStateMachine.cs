using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Abstraction;
using Dev.Cortez.StateMachines.Core.Data;
using Samples.GameScenario.Scripts.Key;
using UnityEngine;

namespace Samples.GameScenario.Scripts.StateMachines.Key
{
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