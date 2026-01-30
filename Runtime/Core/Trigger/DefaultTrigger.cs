using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Abstraction;

namespace Dev.Cortez.StateMachines.Core.Trigger
{
    public sealed class DefaultTrigger : TriggerBase<DefaultTriggerPayload>
    {
        private DefaultTriggerPayload _payload;

        private CancellationTokenSource _lifecycleCts;

        public DefaultTrigger(string id, string name, string description) : base(id, name, description)
        {
        }

        public override async UniTask<bool> TriggerValueAsync(bool targetValue, CancellationToken cancellationToken)
        {
            if (targetValue == IsTriggered)
            {
                return true;
            }

            CancelAndDisposeLifecycleToken();

            if (targetValue)
            {
                _lifecycleCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

                try
                {
                    await HandleActivationDelay(_lifecycleCts.Token);
                    IsTriggered = true;

                    if (_payload.DeactivationRule == TriggerDeactivationRule.Never)
                    {
                        return true;
                    }

                    await HandleDeactivationDelay(_lifecycleCts.Token);
                    IsTriggered = false;
                }
                catch (OperationCanceledException)
                {
                    return false;
                }
            }
            else
            {
                IsTriggered = false;
            }

            return true;
        }

        protected override ValueTask DoDisposeAsync()
        {
            CancelAndDisposeLifecycleToken();

            return base.DoDisposeAsync();
        }

        protected override UniTask<bool> InitializeAsync(DefaultTriggerPayload payload,
            CancellationToken cancellationToken)
        {
            _payload = payload;

            return UniTask.FromResult(true);
        }

        private void CancelAndDisposeLifecycleToken()
        {
            _lifecycleCts?.Cancel();
            _lifecycleCts?.Dispose();
            _lifecycleCts = null;
        }

        private UniTask HandleActivationDelay(CancellationToken cancellationToken)
        {
            return _payload.ActivationRule switch
            {
                TriggerActivationRule.AfterFixedFrame => UniTask.DelayFrame(_payload.ActivationFrameDelay,
                    PlayerLoopTiming.Update, cancellationToken),
                TriggerActivationRule.AfterTime => UniTask.Delay(TimeSpan.FromSeconds(_payload.ActivationTimeDelay),
                    cancellationToken: cancellationToken),
                TriggerActivationRule.AfterTimeUnscaled => UniTask.Delay(
                    TimeSpan.FromSeconds(_payload.ActivationTimeDelay), true, cancellationToken: cancellationToken),
                TriggerActivationRule.Immediately => UniTask.CompletedTask,
#pragma warning disable S3928
                _ => throw new ArgumentOutOfRangeException(nameof(_payload.ActivationRule),
                    "Unsupported activation rule.")
#pragma warning restore S3928
            };
        }

        private UniTask HandleDeactivationDelay(CancellationToken cancellationToken)
        {
            return _payload.DeactivationRule switch
            {
                TriggerDeactivationRule.NextFrame => UniTask.Yield(PlayerLoopTiming.Update, cancellationToken),
                TriggerDeactivationRule.AfterFixedFrame => UniTask.DelayFrame(_payload.DeactivationFrameDelay,
                    PlayerLoopTiming.Update, cancellationToken),
                TriggerDeactivationRule.AfterTime => UniTask.Delay(TimeSpan.FromSeconds(_payload.DeactivationTimeDelay),
                    cancellationToken: cancellationToken),
                TriggerDeactivationRule.AfterTimeUnscaled => UniTask.Delay(
                    TimeSpan.FromSeconds(_payload.DeactivationTimeDelay), true, cancellationToken: cancellationToken),
                TriggerDeactivationRule.Never => UniTask.CompletedTask,
#pragma warning disable S3928
                _ => throw new ArgumentOutOfRangeException(nameof(_payload.DeactivationRule), _payload.DeactivationRule,
                    "Unsupported deactivation rule.")
#pragma warning restore S3928
            };
        }
    }
}