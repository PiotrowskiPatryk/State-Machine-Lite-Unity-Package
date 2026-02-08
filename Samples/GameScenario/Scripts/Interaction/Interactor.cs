using System;
using UnityEngine;

namespace Samples.GameScenario.Scripts.Interaction
{
    public sealed class Interactor : MonoBehaviour
    {
        [SerializeField]
        private Camera _camera;

        private Interactable _currentInteractable;

        private void FixedUpdate()
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

            if (_currentInteractable && interactable != _currentInteractable)
            {
                TriggerHoverEnterForNewInteractable(interactable);
            }

            if (Input.GetMouseButtonDown(0))
            {
                interactable.TriggerClickInteraction();
            }
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