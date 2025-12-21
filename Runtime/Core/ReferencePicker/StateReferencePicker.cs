using System;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Core.Registry;

namespace Dev.Cortez.StateMachines.Core.ReferencePicker
{
    [Serializable]
    public sealed class StateReferencePicker : ReferencePickerBase<IState>
    {
        protected override void DoSubscribe(StateMachineContainerRegistry stateMachineContainerRegistry, IdentifiableObserver<IState> observer)
        {
            throw new NotImplementedException();
        }

        protected override void DoUnsubscribe(StateMachineContainerRegistry stateMachineContainerRegistry, IdentifiableObserver<IState> observer)
        {
            throw new NotImplementedException();
        }
    }
}