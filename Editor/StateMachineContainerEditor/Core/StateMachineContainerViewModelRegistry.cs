using Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.Data;
using Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.ViewModels;
using NUnit.Framework;
using UnityEditor;

namespace Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.Core
{
    public class StateMachineContainerViewModelRegistry
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