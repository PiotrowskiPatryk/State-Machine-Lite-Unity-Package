using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Registry;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dev.Cortez.StateMachines.Core.ReferencePicker
{
    [Serializable]
    public abstract class ReferencePickerBase
    {
        public static string SELECTED_STATE_MACHINE_CONTAINER_GUID_PROPERTY_NAME =
            nameof(_selectedStateMachineContainerGuid);

        public static string SELECTED_ITEM_ID_PROPERTY_NAME = nameof(_selectedItemId);
        
        [SerializeField]
        protected string _selectedStateMachineContainerGuid;
        [SerializeField]
        protected string _selectedItemId;

        protected ReferencePickerBase()
        {
        }

        protected ReferencePickerBase(string selectedItemId, string selectedStateMachineContainerGuid = null)
        {
            _selectedItemId = selectedItemId;
            _selectedStateMachineContainerGuid = selectedStateMachineContainerGuid;
        }
    }
    
    [Serializable]
    public abstract class ReferencePickerBase<TItem> : ReferencePickerBase where TItem : class
    {
        protected ReferencePickerBase()
        {
        }

        protected ReferencePickerBase(string selectedItemId, string selectedStateMachineContainerGuid = null) 
            : base(selectedItemId, selectedStateMachineContainerGuid)
        {
        }

        public bool IsReferenceSelected => !string.IsNullOrWhiteSpace(_selectedItemId);
        
        [CanBeNull]
        public TItem Reference { get; private set; }
        
        public bool IsReferenceResolved => Reference != null;

        public async UniTask<TItem> ResolveReferenceAsync(CancellationToken cancellationToken)
        {
            if (IsReferenceResolved)
            {
                return Reference;
            }

            Observe(onItemResolved: null, onItemUnresolved: null, cancellationToken: cancellationToken);
            
            await UniTask.WaitWhile(() => !IsReferenceResolved, cancellationToken: cancellationToken);
            return Reference;
        }
        
        public void Observe(Action<TItem> onItemResolved, Action onItemUnresolved, CancellationToken cancellationToken)
        {
            if (!IsReferenceSelected)
            {
                return;
            }

            var stateMachineContainerRegistry = StateMachineContainerRegistry.Instance;
            
            var observer = new IdentifiableObserver<TItem>(_selectedItemId, 
                onRegistered: item =>
                {
                    Reference = item;
                    onItemResolved?.Invoke(item);
                }, 
                onUnregistered: () =>
                {
                    Reference = null;
                    onItemUnresolved?.Invoke();
                });
            
            DoSubscribe(stateMachineContainerRegistry, observer);
            cancellationToken.Register(() => DoUnsubscribe(stateMachineContainerRegistry, observer));
        }

        protected abstract void DoSubscribe(StateMachineContainerRegistry stateMachineContainerRegistry, IdentifiableObserver<TItem> observer);
        protected abstract void DoUnsubscribe(StateMachineContainerRegistry stateMachineContainerRegistry, IdentifiableObserver<TItem> observer);
    }
}