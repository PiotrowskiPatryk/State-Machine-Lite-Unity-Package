namespace Dev.Cortez.StateMachines.Core.Trigger
{
    public enum TriggerActivationRule
    {
        Undefined = 0,
        Immediately = 1,
        AfterFixedFrame = 2,
        AfterTime = 3,
        AfterTimeUnscaled = 4,
    }
}