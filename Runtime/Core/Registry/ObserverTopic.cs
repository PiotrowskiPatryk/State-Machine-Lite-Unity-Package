using System.Collections.Generic;
using System.Linq;
using Dev.Cortez.StateMachines.Core.Interfaces;

namespace Dev.Cortez.StateMachines.Core.Registry
{
    internal class ObserverTopic<T> where T : IIdentifiable
    {
        private readonly HashSet<IdentifiableObserver<T>> _observers = new();
        private readonly Dictionary<string, T> _items = new();
        private readonly object _lock = new();

        public void Subscribe(IdentifiableObserver<T> observer)
        {
            if (observer == null)
            {
                return;
            }
                
            lock (_lock)
            {
                _observers.Add(observer);

                if (_items.TryGetValue(observer.Identifier, out var item))
                {
                    observer.OnRegistered?.Invoke(item);
                }
            }
        }

        public void Unsubscribe(IdentifiableObserver<T> observer)
        {
            if (observer == null)
            {
                return;
            }
                
            lock (_lock)
            {
                _observers.Remove(observer);
            }
        }

        public void AddItem(T item)
        {
            _items.TryAdd(item.Id, item);
            
            NotifyRegistered(item);
        }
        
        public void RemoveItem(T item)
        {
            _items.Remove(item.Id);
            
            NotifyUnregistered(item);
        }
        
        private void NotifyRegistered(T item)
        {
            lock (_lock)
            {
                foreach (var observer in _observers.Where(identifier => item.Id.Equals(identifier.Identifier)))
                {
                    observer.OnRegistered?.Invoke(item);
                }
            }
        }

        private void NotifyUnregistered(T item)
        {
            lock (_lock)
            {
                foreach (var observer in _observers.Where(identifier => item.Id.Equals(identifier.Identifier)))
                {
                    observer.OnUnregistered?.Invoke();
                }
            }
        }
    }
}
