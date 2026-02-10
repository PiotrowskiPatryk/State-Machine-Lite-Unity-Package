using System;
using Samples.GameScenario.Scripts.Interaction;
using UnityEngine;

namespace Samples.GameScenario.Scripts.Door
{
    [RequireComponent(typeof(Interactable), typeof(Animator))]
    public sealed class DoorController : MonoBehaviour
    {
        private static readonly int OPEN_DOOR_HASH = Animator.StringToHash("Open");

        public event Action Interacted;

        [SerializeField]
        private Interactable _interactable;

        [SerializeField]
        private Animator _animator;

        public static DoorController Instance { get; private set; }

        public void SetNotInteractable()
        {
            _interactable.Deactivate();
        }

        public void SetInteractable()
        {
            _interactable.Activate();
        }

        public void OpenDoor()
        {
            _animator.SetTrigger(OPEN_DOOR_HASH);
        }

        private void Awake()
        {
            Instance = this;

            _interactable.triggeredInteraction.AddListener(OnInteracted);
        }

        private void OnDestroy()
        {
            _interactable.triggeredInteraction.RemoveListener(OnInteracted);
        }

        private void OnInteracted()
        {
            Interacted?.Invoke();
        }
    }
}