using System;

namespace StateMachineExamples.Scenario.Scripts.Trigger
{
    public static class ScenarioEventBus
    {
        public static event Action<ScenarioEventType, string> OnEvent;

        public static void RaiseEvent(ScenarioEventType type, string id)
        {
            OnEvent?.Invoke(type, id);
        }
    }
}
