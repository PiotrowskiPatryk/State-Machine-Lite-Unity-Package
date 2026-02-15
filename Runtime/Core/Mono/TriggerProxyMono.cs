#region

using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Core.ReferencePicker.Trigger;
using UnityEngine;
using UnityEngine.Events;

#endregion

namespace Dev.Cortez.StateMachines.Core.Mono
{
    public sealed class TriggerProxyMono : MonoBehaviour
    {
        [SerializeField]
        private TriggerReferencePicker _triggerReferencePicker;

        public UnityEvent<bool> triggeredValueChanged;

        private ITrigger _trigger;

        private void Awake()
        {
            _triggerReferencePicker.Observe(OnTriggerResolved, OnTriggerUnresolved, destroyCancellationToken);
        }

        private void OnDestroy()
        {
            DetachTrigger();
        }

        private void DetachTrigger()
        {
            if (_trigger != null)
            {
                _trigger.TriggeredValueChanged -= OnTriggeredValueChanged;
                _trigger = null;
            }
        }

        public void TriggerValue(bool value)
        {
            _triggerReferencePicker.ResolveReferenceAsync(destroyCancellationToken).ContinueWith(trigger =>
                trigger.TriggerValueAsync(value, destroyCancellationToken).Forget()).Forget();
        }

        private void OnTriggerResolved(ITrigger trigger)
        {
            _trigger = trigger;
            _trigger.TriggeredValueChanged += OnTriggeredValueChanged;
        }

        private void OnTriggerUnresolved()
        {
            DetachTrigger();
        }

        private void OnTriggeredValueChanged(ITrigger _, bool value)
        {
            triggeredValueChanged?.Invoke(value);
        }
    }
}