using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Data;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Extensions;
using Dev.Cortez.StateMachines.Logging;

namespace Dev.Cortez.StateMachines.Core.Abstraction
{
    public abstract class TriggerBase<TPayload> : TriggerBase, IAsyncInitializable where TPayload : IPayload
    {
        public InitializationStatus InitializationStatus { get; private set; }

        protected TriggerBase(string id, string name, string description) : base(id, name, description)
        {
        }

        public async UniTask<bool> InitializeAsync(IPayload payload, CancellationToken cancellationToken)
        {
            using var linkedCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            if (!InitializationStatus.CanInitialize())
            {
                LoggerService.Logger.LogError("Unable to initialize trigger base");
                return false;
            }
            
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

        protected abstract UniTask<bool> InitializeAsync(TPayload payload, CancellationToken cancellationToken);
    }

    public abstract class TriggerBase : ITrigger
    {
        public event Action<ITrigger, bool> TriggeredValueChanged;
        private bool _isTriggered;
        public string Id { get; }
        public string Name { get; }
        public string Description { get; }

        public bool IsTriggered
        {
            get => _isTriggered;
            set
            {
                if (value == _isTriggered)
                {
                    return;
                }

                _isTriggered = value;
             
                LoggerService.Logger.LogTrace($"Trigger [{Id}: {Name}] is now {_isTriggered}");
                
                TriggeredValueChanged?.Invoke(this, value);
            }
        }

        protected TriggerBase(string id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }

        public async ValueTask DisposeAsync()
        {
            await DoDisposeAsync();

            TriggeredValueChanged = null;
        }

        public abstract UniTask<bool> TriggerValueAsync(bool targetValue, CancellationToken cancellationToken);

        protected virtual ValueTask DoDisposeAsync()
        {
            return default;
        }
    }
}