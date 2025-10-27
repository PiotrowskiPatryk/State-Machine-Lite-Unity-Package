using System;
using System.Linq;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.Utilities
{
    public static class StateMachineReflectionUtilities
    {
        public static Type GetTriggerPayloadType(string triggerTypeName)
        {
            var triggerType = Type.GetType(triggerTypeName);

            if (triggerType == null)
            {
                Debug.LogError($"Failed to find trigger payload type for '{triggerTypeName}'");

                return null;
            }

            var baseTrigger = triggerType.GetInterfaces().FirstOrDefault(interfaceInstance =>
                interfaceInstance.GenericTypeArguments.Length > 0);

            return baseTrigger?.GenericTypeArguments[0];
        }
    }
}