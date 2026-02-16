#region

using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Abstraction;

#endregion

namespace Dev.Cortez.StateMachines.Core.Trigger
{
    public abstract class DefaultTrigger<TTriggerPayload> : DefaultTrigger where TTriggerPayload : DefaultTriggerPayload
    {
        protected new TTriggerPayload Payload { get; private set; }

        protected DefaultTrigger(string id, string name, string description) : base(id, name, description)
        {
        }

        protected abstract UniTask<bool> DoInitializeAsync(TTriggerPayload payload,
            CancellationToken cancellationToken);

        protected override UniTask<bool> DoInitializeInternalAsync(DefaultTriggerPayload payload,
            CancellationToken cancellationToken)
        {
            if (payload is not TTriggerPayload triggerPayload)
            {
                return UniTask.FromResult(false);
            }

            Payload = triggerPayload;

            return DoInitializeAsync(triggerPayload, cancellationToken);
        }
    }

    public class DefaultTrigger : TriggerBase<DefaultTriggerPayload>
    {
        private CancellationTokenSource _lifecycleCts;

        protected DefaultTriggerPayload Payload;

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

                    if (Payload.DeactivationRule == TriggerDeactivationRule.Never)
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

        protected override async UniTask<bool> InitializeAsync(DefaultTriggerPayload payload,
            CancellationToken cancellationToken)
        {
            Payload = payload;

            var initializationResult = await DoInitializeInternalAsync(payload, cancellationToken);

            return initializationResult;
        }

        private void CancelAndDisposeLifecycleToken()
        {
            _lifecycleCts?.Cancel();
            _lifecycleCts?.Dispose();
            _lifecycleCts = null;
        }

        protected virtual UniTask<bool> DoInitializeInternalAsync(
            DefaultTriggerPayload payload, CancellationToken cancellationToken)
        {
            return UniTask.FromResult(true);
        }

        private UniTask HandleActivationDelay(CancellationToken cancellationToken)
        {
            return Payload.ActivationRule switch
            {
                TriggerActivationRule.AfterFixedFrame => UniTask.DelayFrame(Payload.ActivationFrameDelay,
                    PlayerLoopTiming.Update, cancellationToken),
                TriggerActivationRule.AfterTime => UniTask.Delay(TimeSpan.FromSeconds(Payload.ActivationTimeDelay),
                    cancellationToken: cancellationToken),
                TriggerActivationRule.AfterTimeUnscaled => UniTask.Delay(
                    TimeSpan.FromSeconds(Payload.ActivationTimeDelay), true, cancellationToken: cancellationToken),
                TriggerActivationRule.Immediately => UniTask.CompletedTask,
#pragma warning disable S3928
                _ => throw new ArgumentOutOfRangeException(nameof(Payload.ActivationRule),
                    "Unsupported activation rule.")
#pragma warning restore S3928
            };
        }

        private UniTask HandleDeactivationDelay(CancellationToken cancellationToken)
        {
            return Payload.DeactivationRule switch
            {
                TriggerDeactivationRule.NextFrame => UniTask.Yield(PlayerLoopTiming.Update, cancellationToken),
                TriggerDeactivationRule.AfterFixedFrame => UniTask.DelayFrame(Payload.DeactivationFrameDelay,
                    PlayerLoopTiming.Update, cancellationToken),
                TriggerDeactivationRule.AfterTime => UniTask.Delay(TimeSpan.FromSeconds(Payload.DeactivationTimeDelay),
                    cancellationToken: cancellationToken),
                TriggerDeactivationRule.AfterTimeUnscaled => UniTask.Delay(
                    TimeSpan.FromSeconds(Payload.DeactivationTimeDelay), true, cancellationToken: cancellationToken),
                TriggerDeactivationRule.Never => UniTask.CompletedTask,
#pragma warning disable S3928
                _ => throw new ArgumentOutOfRangeException(nameof(Payload.DeactivationRule), Payload.DeactivationRule,
                    "Unsupported deactivation rule.")
#pragma warning restore S3928
            };
        }
    }
}