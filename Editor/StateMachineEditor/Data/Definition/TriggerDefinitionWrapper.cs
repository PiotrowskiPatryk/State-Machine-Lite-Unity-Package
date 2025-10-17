using UnityEngine;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Data.Definition
{
    internal sealed class TriggerDefinitionWrapper : ScriptableObject
    {
        [SerializeField]
        internal TriggerDefinition Data = new TriggerDefinition();
    }
}
