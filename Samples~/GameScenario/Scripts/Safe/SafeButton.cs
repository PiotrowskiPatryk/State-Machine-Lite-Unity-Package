using System;
using Samples.GameScenario.Scripts.Interaction;
using UnityEngine;

namespace Samples.GameScenario.Scripts.Safe
{
    [RequireComponent(typeof(Interactable)), RequireComponent(typeof(AudioSource))]
    public sealed class SafeButton : MonoBehaviour
    {
        public event Action<SafeButtonType> PressedButton;

        [SerializeField]
        private Interactable _interactable;

        [SerializeField]
        private AudioSource _audioSource;

        [SerializeField]
        private SafeButtonType _safeButtonType;

        public void Activate()
        {
            _interactable.Activate();
        }

        public void Deactivate()
        {
            _interactable.Deactivate();
        }

        private void OnValidate()
        {
            if (!_audioSource)
            {
                _audioSource = GetComponent<AudioSource>();
            }

            if (!_interactable)
            {
                _interactable = GetComponent<Interactable>();
            }
        }

        private void Start()
        {
            _interactable.triggeredInteraction.AddListener(OnTriggeredInteraction);
        }

        private void OnDestroy()
        {
            _interactable.triggeredInteraction.RemoveListener(OnTriggeredInteraction);
        }

        private void OnTriggeredInteraction()
        {
            _audioSource.Play();

            PressedButton?.Invoke(_safeButtonType);
        }
    }
}