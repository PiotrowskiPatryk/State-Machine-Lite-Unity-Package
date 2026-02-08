using System;
using Dev.Cortez.StateMachines.Core.Interfaces;
using UnityEngine;

namespace StateMachineExamples.Scenario.Scripts.Objective
{
    [Serializable]
    public sealed class ScenarioObjectivePayload : IPayload
    {
        [SerializeField]
        private string objectiveName;

        public string ObjectiveName => objectiveName;

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(objectiveName);
        }
    }
}