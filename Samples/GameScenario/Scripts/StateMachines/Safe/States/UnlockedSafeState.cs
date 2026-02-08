using System.Threading;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.ReferencePicker.Trigger;
using JetBrains.Annotations;

namespace Samples.GameScenario.Scripts.StateMachines.Safe
{
    [UsedImplicitly]
    public sealed class UnlockedSafeState : SafeStateBase
    {
        private TriggerReferencePicker _triggerReferencePicker;

        protected override UniTask DoEnterAsync(StateContext context, CancellationToken cancellationToken)
        {
            TriggerOpeningState(cancellationToken);

            return UniTask.CompletedTask;
        }

        protected override UniTask DoExitAsync(StateContext context, CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        private void TriggerOpeningState(CancellationToken cancellationToken)
        {
            _ = _triggerReferencePicker.ResolveReferenceAsync(cancellationToken).ContinueWith(trigger =>
                trigger?.TriggerValueAsync(true, cancellationToken).Forget());
        }
    }
}