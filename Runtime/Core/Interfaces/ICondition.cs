using System;

namespace Dev.Cortez.StateMachines.Core
{
    public interface ICondition : IAsyncDisposable
    {
        event Action<bool> SatisfiedChanged;

        bool IsSatisfied { get; }
    }
}