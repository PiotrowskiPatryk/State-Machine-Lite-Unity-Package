using UnityEngine;
using UnityEngine.Events;

namespace Samples.GameScenario.Scripts.Interaction
{
    public sealed class Interactable : MonoBehaviour
    {
        public UnityEvent triggeredInteraction;
        public UnityEvent triggeredHoverEnter;
        public UnityEvent triggeredHoverExit;

        private bool _isActive;

        public void TriggerHoverEnter()
        {
            if (_isActive)
            {
                triggeredHoverEnter?.Invoke();
            }
        }

        public void TriggerHoverExit()
        {
            if (_isActive)
            {
                triggeredHoverExit?.Invoke();
            }
        }

        public void TriggerClickInteraction()
        {
            if (_isActive)
            {
                triggeredInteraction?.Invoke();
            }
        }

        public void Deactivate()
        {
            _isActive = false;
        }

        public void Activate()
        {
            _isActive = true;
        }
    }
}