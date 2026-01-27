using System;
using System.Linq;
using Dev.Cortez.StateMachines.Core;
using Dev.Cortez.StateMachines.Core.Data;
using Dev.Cortez.StateMachines.Core.Interfaces;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Utilities
{
    public static class StateMachineReflectionUtilities
    {
        public static Type GetTriggerPayloadType(string triggerTypeName)
        {
            return ResolvePayloadType(triggerTypeName);
        }

        public static Type GetConditionPayloadType(string conditionTypeName)
        {
            return ResolvePayloadType(conditionTypeName);
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

            while (baseType is { IsInterface: false })
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

        public static Type GetBaseStateFromStateMachine(string stateMachineTypeName)
        {
            var stateMachineType = Type.GetType(stateMachineTypeName);

            if (stateMachineType == null)
            {
                Debug.LogError($"Failed to find state type for '{stateMachineTypeName}'");

                return null;
            }

            if (stateMachineType.IsAbstract || stateMachineType.IsInterface)
            {
                Debug.LogError($"Provided state machine type '{stateMachineTypeName}' is abstract or interface.");

                return null;
            }

            var baseType = stateMachineType;

            while (baseType is { IsInterface: false })
            {
                var payloadArg =
                    baseType.GenericTypeArguments.FirstOrDefault(type => typeof(IState).IsAssignableFrom(type));

                if (payloadArg != null)
                {
                    return payloadArg;
                }

                baseType = baseType.BaseType;
            }

            Debug.LogError("Provided state machine type does not have generic arguments.");

            return null;
        }

        public static Type GetStatePayloadType(string stateTypeName)
        {
            return ResolvePayloadType(stateTypeName);
        }

        public static string ToClassNameOnly(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                return typeName;
            }

            var comma = typeName.IndexOf(',');

            if (comma >= 0)
            {
                typeName = typeName.Substring(0, comma);
            }

            var lastDot = typeName.LastIndexOf('.');
            var lastPlus = typeName.LastIndexOf('+');
            var sep = Math.Max(lastDot, lastPlus);

            var name = sep >= 0 ? typeName.Substring(sep + 1) : typeName;

            var tick = name.IndexOf('`');

            if (tick >= 0)
            {
                name = name.Substring(0, tick);
            }

            return name;
        }

        public static string ToClassNameOnly(Type type)
        {
            if (type == null)
            {
                return null;
            }

            var source = type.FullName ?? type.Name;

            return ToClassNameOnly(source);
        }

        private static Type ResolvePayloadType(string typeName)
        {
            var stateMachineType = Type.GetType(typeName);

            if (stateMachineType == null)
            {
                Debug.LogError($"Failed to find payload type for '{typeName}'");

                return null;
            }

            if (stateMachineType.IsAbstract || stateMachineType.IsInterface)
            {
                Debug.LogError($"Provided type '{typeName}' is abstract or interface.");

                return null;
            }

            var baseType = stateMachineType;

            while (baseType is { IsInterface: false })
            {
                var payloadArg =
                    baseType.GenericTypeArguments.FirstOrDefault(type => typeof(IPayload).IsAssignableFrom(type));

                if (payloadArg != null)
                {
                    return payloadArg;
                }

                baseType = baseType.BaseType;
            }

            Debug.LogWarning("Provided type does not have generic arguments. Using EmptyPayload.");

            return typeof(EmptyPayload);
        }
    }
}