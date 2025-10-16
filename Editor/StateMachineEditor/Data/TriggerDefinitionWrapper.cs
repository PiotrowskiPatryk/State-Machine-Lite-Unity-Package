using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Data;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Data
{
    internal sealed class TriggerDefinitionWrapper : ScriptableObject
    {
        [SerializeField]
        internal TriggerDefinition Data = new TriggerDefinition();
    }
}
