#region

using System;
using Dev.Cortez.StateMachines.Core.Interfaces;

#endregion

namespace Dev.Cortez.StateMachines.Core.Installer
{
    public sealed class ActivatorBasedStateMachineInstallerBuilder : IStateMachineInstanceBuilder
    {
        public object CreateInstance(Type type)
        {
            return Activator.CreateInstance(type);
        }

        public TObject CreateInstance<TObject>(Type type) where TObject : class
        {
            return (TObject)Activator.CreateInstance(type);
        }

        public TObject CreateInstance<TObject>(Type type, params object[] args) where TObject : class
        {
            return (TObject)Activator.CreateInstance(type, args);
        }
    }
}