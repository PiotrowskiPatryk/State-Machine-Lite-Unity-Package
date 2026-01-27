using System.Collections.Generic;
using System.Linq;
using Dev.Cortez.StateMachines.Core.ReferencePicker;
using Dev.Cortez.StateMachines.Core.StateMachineConfiguration;
using UnityEditor;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.PropertyDrawer
{
    [CustomPropertyDrawer(typeof(TriggerReferencePicker))]
    internal sealed class TriggerPickerPropertyDrawer : ReferencePickerPropertyDrawerBase
    {
        protected override Dictionary<string, string> GetOptions(StateMachineContainer stateMachineContainer)
        {
            return stateMachineContainer.TriggerConfiguration.Triggers.ToDictionary(
                trigger => trigger.Id,
                trigger => $"{trigger.Name} [{trigger.Id}]");
        }
    }
}