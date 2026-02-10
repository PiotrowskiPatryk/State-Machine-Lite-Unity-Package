namespace Dev.Cortez.StateMachines.Core.Trigger
{
    public enum TriggerDeactivationRule
    {
        Undefined = 0,
        Never = 1,
        NextFrame = 2,
        AfterFixedFrame = 3,
        AfterTime = 4,
        AfterTimeUnscaled = 5,
    }
}