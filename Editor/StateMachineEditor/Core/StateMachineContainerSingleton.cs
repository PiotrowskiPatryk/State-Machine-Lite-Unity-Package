using System;
using Dev.Cortez.StateMachines.Core.StateMachineConfiguration;
using UnityEditor;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Core
{
    // Simple singleton that binds directly to the original asset
    public class StateMachineContainerSingleton : ScriptableSingleton<StateMachineContainerSingleton>
    {
        public StateMachineContainer OriginalContainer { get; private set; }

        [field: NonSerialized]
        public SerializedObject WorkingSerializedObject { get; private set; }

        public bool TryApply(SerializedObject stateMachineContainer)
        {
            if (stateMachineContainer == null ||
                stateMachineContainer.targetObject is not StateMachineContainer selectedContainer)
            {
                return false;
            }

            // Bind directly to the original ScriptableObject so changes are applied immediately
            OriginalContainer = selectedContainer;
            WorkingSerializedObject = stateMachineContainer;

            return true;
        }
    }
}