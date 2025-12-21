using System;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Core.Registry;

namespace Dev.Cortez.StateMachines.Core.ReferencePicker
{
    [Serializable]
    public sealed class StateMachineReferencePicker : ReferencePickerBase<IStateMachine>
    {
        protected override void DoSubscribe(StateMachineContainerRegistry stateMachineContainerRegistry, IdentifiableObserver<IStateMachine> observer)
        {
            stateMachineContainerRegistry.SubscribeStateMachine(observer);
        }

        protected override void DoUnsubscribe(StateMachineContainerRegistry stateMachineContainerRegistry, IdentifiableObserver<IStateMachine> observer)
        {
            stateMachineContainerRegistry.UnsubscribeStateMachine(observer);
        }
    }
}