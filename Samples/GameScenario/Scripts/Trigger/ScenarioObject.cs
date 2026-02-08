using UnityEngine;

namespace StateMachineExamples.Scenario.Scripts.Trigger
{
    public class ScenarioObject : MonoBehaviour
    {
        [SerializeField] private string objectId;
        [SerializeField] private ScenarioEventType eventType = ScenarioEventType.Interaction;

        public string ObjectId => objectId;

        // Called by Player Raycast
        public void Interact()
        {
            if (eventType == ScenarioEventType.Interaction)
            {
                ScenarioEventBus.RaiseEvent(eventType, objectId);
                Debug.Log($"Interacted with {objectId}");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (eventType == ScenarioEventType.ZoneEnter && other.CompareTag("Player"))
            {
                ScenarioEventBus.RaiseEvent(eventType, objectId);
                Debug.Log($"Entered Zone {objectId}");
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (eventType == ScenarioEventType.ZoneExit && other.CompareTag("Player"))
            {
                ScenarioEventBus.RaiseEvent(eventType, objectId);
            }
        }
    }
}
