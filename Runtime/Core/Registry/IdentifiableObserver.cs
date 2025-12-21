using System;

namespace Dev.Cortez.StateMachines.Core.Registry
{
    public class IdentifiableObserver<T>
    {
        public string Identifier { get; }
        public Action<T> OnRegistered { get; }
        public Action OnUnregistered { get; }

        public IdentifiableObserver(string identifier, Action<T> onRegistered, Action onUnregistered)
        {
            Identifier = identifier ?? "Unnamed";
            OnRegistered = onRegistered;
            OnUnregistered = onUnregistered;
        }
    }
}
