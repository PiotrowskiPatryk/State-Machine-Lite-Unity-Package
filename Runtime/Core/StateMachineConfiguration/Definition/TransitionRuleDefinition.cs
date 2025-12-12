using System;
using System.Collections.Generic;
using Dev.Cortez.StateMachines.Core.Condition;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Core.StateMachineConfiguration.Definition
{
    [Serializable]
    public sealed class TransitionRuleDefinition
    {
        public static string PRIORITY_PROPERTY_NAME = nameof(_priority);
        public static string CONDITION_DEFINITIONS_PROPERTY_NAME = nameof(_conditionDefinitions);
        public static string TARGET_STATE_PROPERTY_NAME = nameof(_targetState);
        public static string CONDITION_FILTER_TYPE_PROPERTY_NAME = nameof(_conditionFilterType);

        [SerializeField]
        private StateDefinition _targetState;

        [SerializeField]
        private int _priority;

        [SerializeField]
        private List<ConditionDefinition> _conditionDefinitions;

        [SerializeField]
        private ConditionFilterType _conditionFilterType;

        public StateDefinition TargetState => _targetState;
        public int Priority => _priority;
        public List<ConditionDefinition> ConditionDefinitions => _conditionDefinitions;
        public ConditionFilterType ConditionFilterType => _conditionFilterType;
    }
}