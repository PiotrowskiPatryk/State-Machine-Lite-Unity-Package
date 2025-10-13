using UnityEngine;

namespace Dev.Cortez.StateMachines.Core.Configuration
{
    [CreateAssetMenu(menuName = "State Machines/Configuration/StateMachineContainer")]
    public class StateMachineContainer : ScriptableObject
    {
        [SerializeField]
        private TriggerConfiguration _triggerConfiguration;
    }
}
