using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Interfaces;
using UnityEngine.InputSystem;

namespace Dev.Cortez.StateMachines.Core.Trigger
{
    [Serializable]
    public class UserInputActionBasedTriggerPayload : IPayload
    {
        public enum ActionType
        {
            Undefined = 0,
            Performed = 1,
            Cancelled = 2,
        }
        
        public InputActionReference inputActionReference;
        public ActionType actionType;
        
        public bool IsValid()
        {
            return inputActionReference?.action != null && actionType != ActionType.Undefined;
        }
    }
    
    public class UserInputActionBasedTrigger : ITrigger<UserInputActionBasedTriggerPayload>
    {
        public string Id { get; }
        public string Name { get; }
        public string Description { get; }
        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }

        public bool IsTriggered { get; }
        public event Action<ITrigger, bool> TriggeredValueChanged;
        public UniTask<bool> TriggerValueAsync(bool targetValue, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public UniTask<bool> InitializeAsync(UserInputActionBasedTriggerPayload payload, CancellationToken cancellationToken)
        {
            return UniTask.FromResult(true);
        }
    }
}