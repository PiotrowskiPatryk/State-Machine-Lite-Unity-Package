using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Data;
using Dev.Cortez.StateMachines.Core.Interfaces;

namespace Dev.Cortez.StateMachines.Core.Abstraction
{
    public abstract class ConditionBase : ConditionBase<EmptyPayload>
    {
        protected sealed override UniTask<bool> InitializeAsync(EmptyPayload payload,
            CancellationToken cancellationToken)
        {
            return UniTask.FromResult(true);
        }
    }

    public abstract class ConditionBase<TPayload> : ICondition, IAsyncInitializable where TPayload : IPayload
    {
        public event Action<ICondition, bool> SatisfiedChanged;

        public abstract bool IsSatisfied { get; }

        public InitializationStatus InitializationStatus { get; private set; }

        public async UniTask<bool> InitializeAsync(IPayload payload, CancellationToken cancellationToken)
        {
            using var linkedCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            InitializationStatus = InitializationStatus.Initializing;

            if (payload is not TPayload tPayload)
            {
                InitializationStatus = InitializationStatus.Failed;

                return false;
            }

            var result = await InitializeAsync(tPayload, linkedCancellationToken.Token);

            if (result)
            {
                InitializationStatus = InitializationStatus.Initialized;

                return true;
            }

            InitializationStatus = InitializationStatus.Failed;

            return false;
        }

        public virtual ValueTask DisposeAsync()
        {
            SatisfiedChanged = null;

            return default;
        }

        protected void PublishSatisfiedChangedEvent(bool value)
        {
            SatisfiedChanged?.Invoke(this, value);
        }
        
        protected abstract UniTask<bool> InitializeAsync(TPayload payload, CancellationToken cancellationToken);
    }
}