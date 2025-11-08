using UnityEngine;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Data.Configuration
{
    [CreateAssetMenu(menuName = "State Machines/Configuration/StateMachineContainer")]
    public class StateMachineContainer : ScriptableObject
    {
        public static string STATE_MACHINE_CONFIGURATION_PROPERTY_NAME = nameof(_stateMachineConfiguration);
        public static string TRIGGER_CONFIGURATION_PROPERTY_NAME = nameof(_triggerConfiguration);

        [SerializeField]
        private TriggerConfiguration _triggerConfiguration;

        [SerializeField]
        private StateMachineConfiguration _stateMachineConfiguration;
    }
}