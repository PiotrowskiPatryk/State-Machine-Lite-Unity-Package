using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Core.ReferencePicker.Trigger;
using UnityEngine;

namespace Samples.GameScenario.Scripts.Environment
{
    public sealed class EnvironmentLightController : MonoBehaviour
    {
        [SerializeField]
        private TriggerReferencePicker _invalidSafeSequenceTriggered;

        [SerializeField]
        private Light _light;

        [SerializeField]
        private AudioSource _audioSource;

        [SerializeField]
        private float _invalidLightColorDisplayDurationInSeconds = 3.0f;

        private ITrigger _invalidSafeSequenceTrigger;
        private Color _defaultLightColor;

        private void Start()
        {
            _defaultLightColor = _light.color;
            _invalidSafeSequenceTriggered.Observe(OnResolvedTriggerReference, OnUnresolvedTriggerReference,
                destroyCancellationToken);
        }

        private void OnDestroy()
        {
            if (_invalidSafeSequenceTrigger != null)
            {
                _invalidSafeSequenceTrigger.TriggeredValueChanged -= OnInvalidSafeSequenceTriggered;
            }
        }

        private async UniTaskVoid PlayInvalidEffectAsync()
        {
            _audioSource.Play();

            var elapsedInSeconds = 0f;
            var halfDuration = _invalidLightColorDisplayDurationInSeconds / 2f;

            // Transition from default color to red
            while (elapsedInSeconds < halfDuration)
            {
                elapsedInSeconds += Time.deltaTime;
                var t = elapsedInSeconds / halfDuration;
                _light.color = Color.Lerp(_defaultLightColor, Color.red, t);
                await UniTask.Yield();
            }

            _light.color = Color.red;

            // Transition from red back to default color
            elapsedInSeconds = 0f;

            while (elapsedInSeconds < halfDuration)
            {
                elapsedInSeconds += Time.deltaTime;
                var t = elapsedInSeconds / halfDuration;
                _light.color = Color.Lerp(Color.red, _defaultLightColor, t);
                await UniTask.Yield();
            }

            _light.color = _defaultLightColor;
        }

        private void OnUnresolvedTriggerReference()
        {
            _invalidSafeSequenceTrigger.TriggeredValueChanged -= OnInvalidSafeSequenceTriggered;
            _invalidSafeSequenceTrigger = null;
        }

        private void OnResolvedTriggerReference(ITrigger trigger)
        {
            _invalidSafeSequenceTrigger = trigger;
            _invalidSafeSequenceTrigger.TriggeredValueChanged += OnInvalidSafeSequenceTriggered;
        }

        private void OnInvalidSafeSequenceTriggered(ITrigger trigger, bool isTriggered)
        {
            if (isTriggered)
            {
                PlayInvalidEffectAsync().Forget();
            }
        }
    }
}