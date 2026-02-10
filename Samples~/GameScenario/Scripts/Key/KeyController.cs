using System;
using Samples.GameScenario.Scripts.Interaction;
using UnityEngine;

namespace Samples.GameScenario.Scripts.Key
{
    [RequireComponent(typeof(Interactable))]
    public sealed class KeyController : MonoBehaviour
    {
        public event Action Grabbed;

        [SerializeField]
        private AudioClip _audioClip;

        [SerializeField]
        private Interactable _interactable;

        public static KeyController Instance { get; private set; }

        public void SetNotGrabbable()
        {
            _interactable.Deactivate();
        }

        public void SetGrabbable()
        {
            _interactable.Activate();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void Awake()
        {
            Instance = this;

            _interactable.triggeredInteraction.AddListener(OnTriggeredInteraction);
        }

        private void OnDestroy()
        {
            _interactable.triggeredInteraction.RemoveListener(OnTriggeredInteraction);
        }

        private void OnTriggeredInteraction()
        {
            AudioSource.PlayClipAtPoint(_audioClip, transform.position);

            Grabbed?.Invoke();
        }
    }
}