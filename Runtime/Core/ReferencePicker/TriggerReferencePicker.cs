using System;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Core.Registry;

namespace Dev.Cortez.StateMachines.Core.ReferencePicker
{
    [Serializable]
    public sealed class TriggerReferencePicker : ReferencePickerBase<ITrigger>
    {
        public TriggerReferencePicker()
        {
        }

        public TriggerReferencePicker(string selectedItemId, string selectedStateMachineContainerGuid = null) 
            : base(selectedItemId, selectedStateMachineContainerGuid)
        {
        }

        protected override void DoSubscribe(StateMachineContainerRegistry stateMachineContainerRegistry, IdentifiableObserver<ITrigger> observer)
        {
            stateMachineContainerRegistry.SubscribeTrigger(observer);
        }

        protected override void DoUnsubscribe(StateMachineContainerRegistry stateMachineContainerRegistry, IdentifiableObserver<ITrigger> observer)
        {
            stateMachineContainerRegistry.UnsubscribeTrigger(observer);
        }
    }
}