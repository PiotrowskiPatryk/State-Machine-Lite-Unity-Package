using System;

namespace Dev.Cortez.StateMachines.Core
{
    public interface ICondition : IAsyncDisposable
    {
        event Action<ICondition, bool> SatisfiedChanged;

        bool IsSatisfied { get; }
    }
}