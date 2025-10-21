using UnityEngine;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Data
{
    [CreateAssetMenu(menuName = "State Machines/Configuration/StateMachineContainer")]
    public class StateMachineContainer : ScriptableObject
    {
        [SerializeField]
        private TriggerConfiguration _triggerConfiguration;

        [SerializeField]
        private StateMachineConfiguration _stateMachineConfiguration;
    }
}