using UnityEngine;
using UnityEngine.Events;

namespace Samples.GameScenario.Scripts.Interaction
{
    public sealed class Interactable : MonoBehaviour
    {
        public UnityEvent triggeredInteraction;
        public UnityEvent triggeredHoverEnter;
        public UnityEvent triggeredHoverExit;

        public void TriggerHoverEnter()
        {
            triggeredHoverEnter?.Invoke();
        }

        public void TriggerHoverExit()
        {
            triggeredHoverExit?.Invoke();
        }

        public void TriggerClickInteraction()
        {
            triggeredInteraction?.Invoke();
        }
    }
}