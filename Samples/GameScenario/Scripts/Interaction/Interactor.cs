using UnityEngine;

namespace Samples.GameScenario.Scripts.Interaction
{
    public sealed class Interactor : MonoBehaviour
    {
        [SerializeField]
        private Camera _camera;

        [SerializeField]
        private float _interactDistanceInMeters = 1.0f;

        private Interactable _currentInteractable;

        private void Update()
        {
            if (!Physics.Raycast(_camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)), out var hit) ||
                !hit.collider.TryGetComponent(out Interactable interactable))
            {
                if (_currentInteractable != null)
                {
                    TriggerHoverExitForLastInteractable();
                }

                return;
            }

            if (!IsWithinInteractionRange(hit))
            {
                return;
            }

            if (_currentInteractable && interactable != _currentInteractable)
            {
                TriggerHoverEnterForNewInteractable(interactable);
            }

            if (Input.GetMouseButtonDown(0))
            {
                interactable.TriggerClickInteraction();
            }
        }

        private bool IsWithinInteractionRange(RaycastHit hit)
        {
            return hit.distance <= _interactDistanceInMeters;
        }

        private void TriggerHoverEnterForNewInteractable(Interactable interactable)
        {
            _currentInteractable.TriggerHoverEnter();
            _currentInteractable = interactable;
        }

        private void TriggerHoverExitForLastInteractable()
        {
            _currentInteractable.TriggerHoverExit();
            _currentInteractable = null;
        }
    }
}