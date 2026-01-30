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
            switch (_payload.ActivationRule)
            {
                case TriggerActivationRule.AfterFixedFrame:
                    return UniTask.DelayFrame(_payload.ActivationFrameDelay, PlayerLoopTiming.FixedUpdate,
                        cancellationToken);
                case TriggerActivationRule.AfterTime:
                    return UniTask.Delay(TimeSpan.FromSeconds(_payload.ActivationTimeDelay),
                        cancellationToken: cancellationToken);
                case TriggerActivationRule.AfterTimeUnscaled:
                    return UniTask.Delay(TimeSpan.FromSeconds(_payload.ActivationTimeDelay), true,
                        cancellationToken: cancellationToken);
                case TriggerActivationRule.Immediately:
                    return UniTask.CompletedTask;
            }

            return UniTask.CompletedTask;
        }

        private UniTask HandleDeactivationDelay(CancellationToken cancellationToken)
        {
            switch (_payload.DeactivationRule)
            {
                case TriggerDeactivationRule.NextFrame:
                    return UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                case TriggerDeactivationRule.AfterFixedFrame:
                    return UniTask.DelayFrame(_payload.DeactivationFrameDelay, PlayerLoopTiming.FixedUpdate,
                        cancellationToken);
                case TriggerDeactivationRule.AfterTime:
                    return UniTask.Delay(TimeSpan.FromSeconds(_payload.DeactivationTimeDelay),
                        cancellationToken: cancellationToken);
                case TriggerDeactivationRule.AfterTimeUnscaled:
                    return UniTask.Delay(TimeSpan.FromSeconds(_payload.DeactivationTimeDelay), true,
                        cancellationToken: cancellationToken);
                case TriggerDeactivationRule.Never:
                    return UniTask.CompletedTask;
            }

            return UniTask.CompletedTask;
        }
    }
}