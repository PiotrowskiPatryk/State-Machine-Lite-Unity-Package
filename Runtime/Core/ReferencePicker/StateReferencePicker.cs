using System;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Core.Registry;

namespace Dev.Cortez.StateMachines.Core.ReferencePicker
{
    [Serializable]
    public sealed class StateReferencePicker : ReferencePickerBase<IState>
    {
        public StateReferencePicker()
        {
        }

        public StateReferencePicker(string selectedItemId, string selectedStateMachineContainerGuid = null) 
            : base(selectedItemId, selectedStateMachineContainerGuid)
        {
        }

        protected override void DoSubscribe(StateMachineContainerRegistry stateMachineContainerRegistry,
            IdentifiableObserver<IState> observer)
        {
            stateMachineContainerRegistry.SubscribeState(observer);
        }

        protected override void DoUnsubscribe(StateMachineContainerRegistry stateMachineContainerRegistry,
            IdentifiableObserver<IState> observer)
        {
            stateMachineContainerRegistry.UnsubscribeState(observer);
        }
    }
}