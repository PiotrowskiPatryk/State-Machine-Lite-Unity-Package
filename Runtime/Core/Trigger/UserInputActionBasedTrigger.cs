using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Abstraction;
using Dev.Cortez.StateMachines.Core.Interfaces;
using UnityEngine.InputSystem;

namespace Dev.Cortez.StateMachines.Core.Trigger
{
    [Serializable]
    public sealed class UserInputActionBasedTriggerPayload : IPayload
    {
        public InputActionReference inputActionReference;
        public ActionType actionType;

        public bool IsValid()
        {
            return inputActionReference?.action != null && actionType != ActionType.Undefined;
        }

        public enum ActionType
        {
            Undefined = 0,
            Performed = 1,
            Cancelled = 2
        }
    }

    public class UserInputActionBasedTrigger : TriggerBase<UserInputActionBasedTriggerPayload>
    {
        public UserInputActionBasedTrigger(string id, string name, string description) : base(id, name, description)
        {
        }

        public override UniTask<bool> TriggerValueAsync(bool targetValue, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override UniTask<bool> InitializeAsync(UserInputActionBasedTriggerPayload payload,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}