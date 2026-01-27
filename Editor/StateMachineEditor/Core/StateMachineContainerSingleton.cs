using System;
using Dev.Cortez.StateMachines.Core.StateMachineConfiguration;
using UnityEditor;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Core
{
    public sealed class StateMachineContainerSingleton : ScriptableSingleton<StateMachineContainerSingleton>
    {
        public StateMachineContainer OriginalContainer { get; private set; }

        [field: NonSerialized]
        public SerializedObject WorkingSerializedObject { get; private set; }

        public bool TryApply(SerializedObject stateMachineContainer)
        {
            if (stateMachineContainer is not { targetObject: StateMachineContainer selectedContainer })
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