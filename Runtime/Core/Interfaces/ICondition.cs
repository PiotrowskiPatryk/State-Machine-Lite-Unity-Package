using System;

namespace Dev.Cortez.StateMachines.Core
{
    public interface ICondition
    {
        event Action<bool> SatisfiedChanged;

        bool IsSatisfied { get; }
    }
}