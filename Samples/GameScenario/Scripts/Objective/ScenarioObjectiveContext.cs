namespace StateMachineExamples.Scenario.Scripts.Objective
{
    public sealed class ScenarioObjectiveContext
    {
        public string ObjectiveName { get; }

        public ScenarioObjectiveContext(string objectiveName)
        {
            ObjectiveName = objectiveName;
        }
    }
}