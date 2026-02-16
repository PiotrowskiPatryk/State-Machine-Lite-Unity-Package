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
            TriggerValueInternal(value).Forget();
        }

        private async UniTaskVoid TriggerValueInternal(bool value)
        {
            var trigger = await _triggerReferencePicker.ResolveReferenceAsync(destroyCancellationToken);

            if (trigger != null)
            {
                await trigger.TriggerValueAsync(value, destroyCancellationToken);
            }
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