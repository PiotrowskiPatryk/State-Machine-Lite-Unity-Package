using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Interfaces;

namespace Dev.Cortez.StateMachines.Core.Abstraction
{
    public abstract class TriggerBase<TPayload> : TriggerBase, ITrigger<TPayload> where TPayload : IPayload
    {
        protected TriggerBase(string id, string name, string description) : base(id, name, description)
        {
        }

        public abstract UniTask<bool> InitializeAsync(TPayload payload, CancellationToken cancellationToken);
    }

    public abstract class TriggerBase : ITrigger
    {
        public event Action<ITrigger, bool> TriggeredValueChanged;
        public string Id { get; }
        public string Name { get; }
        public string Description { get; }
        public abstract bool IsTriggered { get; }

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