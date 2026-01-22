using System;

namespace Dev.Cortez.StateMachines.Core.Registry
{
    public class IdentifiableObserver<T> : IDisposable
    {
        public string Identifier { get; }
        public Action<T> OnRegistered { get; private set; }
        public Action OnUnregistered { get; private set; }

        public IdentifiableObserver(string identifier, Action<T> onRegistered, Action onUnregistered)
        {
            Identifier = identifier ?? "Unnamed";
            OnRegistered = onRegistered;
            OnUnregistered = onUnregistered;
        }

        public void Dispose()
        {
            OnRegistered = null;
            OnUnregistered = null;
        }
    }
}