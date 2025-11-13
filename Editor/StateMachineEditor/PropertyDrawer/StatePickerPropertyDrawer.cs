using System.Collections.Generic;
using Dev.Cortez.StateMachines.Core.ReferencePicker;
using Dev.Cortez.StateMachines.Core.StateMachineConfiguration;
using UnityEditor;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.PropertyDrawer
{
    [CustomPropertyDrawer(typeof(StateReferencePicker))]
    internal sealed class StatePickerPropertyDrawer : ReferencePickerPropertyDrawerBase
    {
        protected override Dictionary<string, string> GetOptions(StateMachineContainer stateMachineContainer)
        {
            var options = new Dictionary<string, string>();
            var stateMachineConfiguration = stateMachineContainer.StateMachineConfiguration;

            foreach (var stateMachine in stateMachineConfiguration.StateMachines)
            {
                foreach (var state in stateMachine.States)
                {
                    options.Add(state.Id, $"{stateMachine.Name}/{state.Name} [{state.Id}]");
                }
            }

            return options;
        }
    }
}