using Dev.Cortez.StateMachines.Core.StateMachineConfiguration;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.ViewModels;
using NUnit.Framework;
using UnityEditor;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Core
{
    public sealed class StateMachineContainerViewModelRegistry
    {
        public StateMachineConfigurationViewModel StateMachineConfigurationViewModel { get; }
        public TriggerConfigurationViewModel TriggerConfigurationViewModel { get; }

        public StateMachineContainerViewModelRegistry(SerializedObject serializedObject)
        {
            StateMachineConfigurationViewModel = new StateMachineConfigurationViewModel(
                serializedObject.FindProperty(StateMachineContainer.STATE_MACHINE_CONFIGURATION_PROPERTY_NAME));

            TriggerConfigurationViewModel = new TriggerConfigurationViewModel(
                serializedObject.FindProperty(StateMachineContainer.TRIGGER_CONFIGURATION_PROPERTY_NAME));

            Assert.NotNull(StateMachineConfigurationViewModel);
        }
    }
}