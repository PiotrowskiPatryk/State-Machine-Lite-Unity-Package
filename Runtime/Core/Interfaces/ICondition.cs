using System;

namespace Dev.Cortez.StateMachines.Core.Interfaces
{
    public interface ICondition : IAsyncDisposable
    {
        event Action<ICondition, bool> SatisfiedChanged;

        bool IsSatisfied { get; }
    }
}