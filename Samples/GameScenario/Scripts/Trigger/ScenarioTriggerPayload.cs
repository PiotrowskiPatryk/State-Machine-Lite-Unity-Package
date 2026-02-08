using System;
using Dev.Cortez.StateMachines.Core.Interfaces;
using UnityEngine;

namespace StateMachineExamples.Scenario.Scripts.Trigger
{
    [Serializable]
    public class ScenarioTriggerPayload : IPayload
    {
        [SerializeField] private ScenarioEventType _eventType;
        [SerializeField] private string _targetId;

        public ScenarioEventType EventType => _eventType;
        public string TargetId => _targetId;

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(_targetId);
        }
    }
}
