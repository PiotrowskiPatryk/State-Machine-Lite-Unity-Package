using System;
using Dev.Cortez.StateMachines.Core.Interfaces;

namespace Dev.Cortez.StateMachines.Core.Trigger
{
    [Serializable]
    public sealed class TimeDelayTriggerPayload : IPayload
    {
        public float delayInSeconds;
        
        public bool IsValid()
        {
            return delayInSeconds >= 0f;
        }
    }
}