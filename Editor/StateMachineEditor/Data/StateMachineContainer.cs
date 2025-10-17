using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Data;
using UnityEngine;

namespace Dev.Cortez.StateMachines.StateMachineEditor.Data
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
