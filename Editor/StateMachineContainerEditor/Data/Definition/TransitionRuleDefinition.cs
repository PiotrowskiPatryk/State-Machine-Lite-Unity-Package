using System;
using System.Collections.Generic;
using Dev.Cortez.StateMachines.Core.Condition;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.Data.Definition
{
    [Serializable]
    public sealed class TransitionRuleDefinition
    {
        public static string PRIORITY_PROPERTY_NAME = nameof(_priority);
        public static string CONDITION_DEFINITIONS_PROPERTY_NAME = nameof(_conditionDefinitions);
        public static string INITIAL_STATE_ID_PROPERTY_NAME = nameof(_initialStateId);
        public static string TARGET_STATE_ID_PROPERTY_NAME = nameof(_targetStateId);
        public static string CONDITION_FILTER_TYPE_PROPERTY_NAME = nameof(_conditionFilterType);

        [SerializeField]
        private string _initialStateId;

        [SerializeField]
        private string _targetStateId;

        [SerializeField]
        private int _priority;

        [SerializeField]
        private List<ConditionDefinition> _conditionDefinitions;

        [SerializeField]
        private ConditionFilterType _conditionFilterType;

        public string TargetStateId => _targetStateId;
        public string InitialStateId => _initialStateId;

        public int Priority => _priority;
        public List<ConditionDefinition> ConditionDefinitions => _conditionDefinitions;
    }
}