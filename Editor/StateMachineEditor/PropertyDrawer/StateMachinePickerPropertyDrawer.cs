using System.Collections.Generic;
using System.Linq;
using Dev.Cortez.StateMachines.Core.ReferencePicker;
using Dev.Cortez.StateMachines.Core.StateMachineConfiguration;
using UnityEditor;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.PropertyDrawer
{
    [CustomPropertyDrawer(typeof(StateMachineReferencePicker))]
    internal sealed class StateMachinePickerPropertyDrawer : ReferencePickerPropertyDrawerBase
    {
        protected override Dictionary<string, string> GetOptions(StateMachineContainer stateMachineContainer)
        {
            var stateMachineConfiguration = stateMachineContainer.StateMachineConfiguration;

            return stateMachineConfiguration.StateMachines.ToDictionary(stateMachine => stateMachine.Id,
                stateMachine => $"{stateMachine.Name} [{stateMachine.Id}]");
        }
    }
}