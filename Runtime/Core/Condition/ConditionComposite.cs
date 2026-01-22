using System.Collections.Generic;
using System.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Abstraction;
using Dev.Cortez.StateMachines.Core.Interfaces;

namespace Dev.Cortez.StateMachines.Core.Condition
{
    public sealed class ConditionComposite : ConditionBase
    {
        private readonly List<ICondition> _conditions;
        private readonly ConditionFilterType _conditionFilterType;

        public override bool IsSatisfied
        {
            get
            {
                return _conditionFilterType switch
                {
                    ConditionFilterType.Undefined => false,
                    ConditionFilterType.All => _conditions.TrueForAll(condition => condition?.IsSatisfied == true),
                    ConditionFilterType.Any => _conditions.Exists(condition => condition?.IsSatisfied == true),
                    _ => false
                };
            }
        }

        public ConditionComposite(List<ICondition> conditions, ConditionFilterType conditionFilterType)
        {
            _conditions = conditions;
            _conditionFilterType = conditionFilterType;

            foreach (var condition in conditions)
            {
                condition.SatisfiedChanged += OnConditionSatisfiedChanged;
            }
        }

        public override ValueTask DisposeAsync()
        {
            foreach (var condition in _conditions)
            {
                condition.SatisfiedChanged -= OnConditionSatisfiedChanged;
            }

            return base.DisposeAsync();
        }

        // TODO - Refactor this functionality to publish only changed condition events, not everytime
        private void OnConditionSatisfiedChanged(ICondition condition, bool _)
        {
            PublishSatisfiedChangedEvent(IsSatisfied);
        }
    }
}