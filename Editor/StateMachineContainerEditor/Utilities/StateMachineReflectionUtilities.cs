using System;
using System.Linq;
using Dev.Cortez.StateMachines.Core.Data;
using Dev.Cortez.StateMachines.Core.Interfaces;
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

        public static Type GetStateMachinePayloadType(string stateMachineTypeName)
        {
            var stateMachineType = Type.GetType(stateMachineTypeName);

            if (stateMachineType == null)
            {
                Debug.LogError($"Failed to find state machine payload type for '{stateMachineTypeName}'");

                return null;
            }

            if (stateMachineType.IsAbstract || stateMachineType.IsInterface)
            {
                Debug.LogError($"Provided state machine type '{stateMachineTypeName}' is abstract or interface.");

                return null;
            }

            var baseType = stateMachineType;

            while (!baseType.IsInterface)
            {
                var payloadArg =
                    baseType.GenericTypeArguments.FirstOrDefault(type => typeof(IPayload).IsAssignableFrom(type));

                if (payloadArg != null)
                {
                    return payloadArg;
                }

                baseType = baseType.BaseType;
            }

            Debug.LogWarning("Provided state machine type does not have generic arguments. Using EmptyPayload.");

            return typeof(EmptyPayload);
        }
    }
}