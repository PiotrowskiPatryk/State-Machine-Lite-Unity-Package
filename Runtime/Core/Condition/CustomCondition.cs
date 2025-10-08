using System;
using System.Threading.Tasks;

namespace Dev.Cortez.StateMachines.Core.Condition
{
    public class CustomCondition : ICondition
    {
        public event Action<bool> SatisfiedChanged;

        private Action<bool> _cachedCondition;
        public bool IsSatisfied { get; }

        public CustomCondition(Action<bool> condition)
        {
            IsSatisfied = false;

            _cachedCondition = condition;
            _cachedCondition += OnConditionChanged;
        }

        public ValueTask DisposeAsync()
        {
            _cachedCondition -= OnConditionChanged;

            return default;
        }

        private void OnConditionChanged(bool condition)
        {
            SatisfiedChanged?.Invoke(condition);
        }
    }
}