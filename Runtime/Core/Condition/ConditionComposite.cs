using System.Collections.Generic;
using Dev.Cortez.StateMachines.Core.Abstraction;

namespace Dev.Cortez.StateMachines.Core.Condition
{
    public sealed class ConditionComposite : ConditionBase
    {
        private readonly List<ICondition> _conditions;
        private readonly ConditionFilterType _conditionFilterType;

        public override bool IsSatisfied { get; }

        public ConditionComposite(List<ICondition> conditions, ConditionFilterType conditionFilterType)
        {
            _conditions = conditions;
            _conditionFilterType = conditionFilterType;
        }
    }
}