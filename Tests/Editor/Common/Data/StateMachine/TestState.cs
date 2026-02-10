using System.Threading;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Abstraction;
using Dev.Cortez.StateMachines.Core.Attributes;
using Dev.Cortez.StateMachines.Core.Data;

namespace Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Common.Data.StateMachine
{
    [ExcludeFromTypePicker]
    public class TestState : StateBase
    {
        protected override UniTask DoEnterAsync(EmptyContext context, CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        protected override UniTask DoExitAsync(EmptyContext context, CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        protected override UniTask<bool> DoInitializeAsync(EmptyPayload payload,
            CancellationToken cancellationToken)
        {
            return UniTask.FromResult(true);
        }
    }
}