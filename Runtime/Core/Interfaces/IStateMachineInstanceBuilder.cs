#region

using System;

#endregion

namespace Dev.Cortez.StateMachines.Core.Interfaces
{
    public interface IStateMachineInstanceBuilder
    {
        object CreateInstance(Type type);
        TObject CreateInstance<TObject>(Type type) where TObject : class;
        TObject CreateInstance<TObject>(Type type, params object[] args) where TObject : class;
    }
}