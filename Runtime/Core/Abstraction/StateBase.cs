using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Data;
using JetBrains.Annotations;

namespace Dev.Cortez.StateMachines.Core.Abstraction
{
    public abstract class StateBase : StateBase<EmptyContext>
    {
    }

    public abstract class StateBase<TStateContext> : StateBase<TStateContext, DefaultStatePayload>
    {
    }

    public abstract class StateBase<TStateContext, TStatePayload> : IState<TStateContext, TStatePayload>
        where TStatePayload : IStatePayload
    {
        public event Action<StateStatus> StatusChanged;

        private StateStatus _stateStatus;

        public StateStatus StateStatus
        {
            get => _stateStatus;
            private set
            {
                _stateStatus = value;
                StatusChanged?.Invoke(value);
            }
        }

        public InitializationStatus InitializationStatus { get; private set; } = InitializationStatus.NotInitialized;

        public bool IsActive { get; private set; }

        [NotNull]
        public string Id { get; private set; }

        public UniTask<bool> InitializeAsync(IPayload payload, CancellationToken cancellationToken)
        {
            if (payload is not TStatePayload statePayload)
            {
                InitializationStatus = InitializationStatus.Failed;

                return UniTask.FromResult(false);
            }

            Id = statePayload.Id;

            return InitializeAsyncInternal(statePayload, cancellationToken);
        }

        public async UniTask<bool> EnterAsync(TStateContext context, CancellationToken cancellationToken)
        {
            using var linkedCancellationTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            if (IsActive)
            {
                return false;
            }

            StateStatus = StateStatus.Activating;
            await DoEnterAsync(context, linkedCancellationTokenSource.Token);
            StateStatus = StateStatus.Active;

            IsActive = true;

            return true;
        }

        public async UniTask<bool> ExitAsync(TStateContext context, CancellationToken cancellationToken)
        {
            using var linkedCancellationTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            if (!IsActive)
            {
                return false;
            }

            StateStatus = StateStatus.Deactivating;
            await DoExitAsync(context, linkedCancellationTokenSource.Token);
            StateStatus = StateStatus.Inactive;
            IsActive = false;

            return true;
        }

        public ValueTask DisposeAsync()
        {
            return DoDisposeAsync();
        }

        protected abstract UniTask DoEnterAsync(TStateContext context, CancellationToken cancellationToken);
        protected abstract UniTask DoExitAsync(TStateContext context, CancellationToken cancellationToken);

        protected virtual ValueTask DoDisposeAsync()
        {
            return default;
        }

        protected virtual UniTask<bool> DoInitializeAsync(TStatePayload statePayload,
            CancellationToken cancellationToken)
        {
            return UniTask.FromResult(true);
        }

        private async UniTask<bool> InitializeAsyncInternal(TStatePayload payload,
            CancellationToken cancellationToken)
        {
            using var linkedCancellationTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            switch (InitializationStatus)
            {
                case InitializationStatus.Initialized:
                    return true;
                case InitializationStatus.Failed:
                    return false;
            }

            var initializationResult = await DoInitializeAsync(payload, linkedCancellationTokenSource.Token);

            if (!initializationResult)
            {
                InitializationStatus = InitializationStatus.Failed;

                return false;
            }

            InitializationStatus = InitializationStatus.Initialized;

            return true;
        }
    }
}