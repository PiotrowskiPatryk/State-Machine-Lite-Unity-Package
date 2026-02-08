using System.Threading;
using Cysharp.Threading.Tasks;

namespace Samples.GameScenario.Scripts.StateMachines.Key
{
    public sealed class KeyPickedState : KeyStateBase
    {
        protected override UniTask DoEnterAsync(KeyContext context, CancellationToken cancellationToken)
        {
            context.KeyController.Hide();

            return UniTask.CompletedTask;
        }

        protected override UniTask DoExitAsync(KeyContext context, CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }
    }
}