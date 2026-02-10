using System;
using System.Collections.Generic;
using System.Linq;
using Dev.Cortez.StateMachines.Core;
using Dev.Cortez.StateMachines.Core.Attributes;
using Dev.Cortez.StateMachines.Core.Data;
using Dev.Cortez.StateMachines.Core.Interfaces;
using UnityEditor;
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

        /// <summary>
        /// Gets all concrete types that can be assigned to the given base type.
        /// Handles both concrete types and open generic type definitions.
        /// </summary>
        /// <param name="baseType">The base type to find implementations for.</param>
        /// <returns>Enumerable of concrete types assignable to the base type.</returns>
        public static IEnumerable<Type> GetConcreteTypeCandidates(Type baseType)
        {
            if (baseType == null)
            {
                return Enumerable.Empty<Type>();
            }

            // For open generic type definitions, we need to search more broadly
            // since TypeCache.GetTypesDerivedFrom doesn't work with open generics
            if (baseType.IsGenericTypeDefinition)
            {
                // Get all types from all loaded assemblies and filter
                var allTypes = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a =>
                    {
                        try { return a.GetTypes(); }
                        catch { return Array.Empty<Type>(); }
                    })
                    .Where(t => t is { IsAbstract: false, IsGenericTypeDefinition: false })
                    .Where(t => !t.IsDefined(typeof(ExcludeFromTypePickerAttribute), false))
                    .Where(t => IsAssignableToGenericType(t, baseType));

                return allTypes;
            }

            // For constructed generics (e.g., SafeStateBase<IPayload>), extract the generic definition
            if (baseType.IsGenericType && !baseType.IsGenericTypeDefinition)
            {
                var genericDefinition = baseType.GetGenericTypeDefinition();

                var allTypes = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a =>
                    {
                        try { return a.GetTypes(); }
                        catch { return Array.Empty<Type>(); }
                    })
                    .Where(t => t is { IsAbstract: false, IsGenericTypeDefinition: false })
                    .Where(t => !t.IsDefined(typeof(ExcludeFromTypePickerAttribute), false))
                    .Where(t => IsAssignableToGenericType(t, genericDefinition));

                return allTypes;
            }

            // For concrete (non-generic) base types, use TypeCache for better performance
            var derivedTypes = TypeCache.GetTypesDerivedFrom(baseType)
                .Where(t => t is { IsAbstract: false, IsGenericTypeDefinition: false })
                .Where(t => !t.IsDefined(typeof(ExcludeFromTypePickerAttribute), false));

            return derivedTypes;
        }

        /// <summary>
        /// Checks if a given type is assignable to an open generic type definition
        /// by walking up the inheritance hierarchy.
        /// </summary>
        /// <param name="givenType">The concrete type to check.</param>
        /// <param name="genericTypeDefinition">The open generic type definition to match against.</param>
        /// <returns>True if givenType inherits from a closed construction of genericTypeDefinition.</returns>
        public static bool IsAssignableToGenericType(Type givenType, Type genericTypeDefinition)
        {
            if (givenType == null || genericTypeDefinition == null)
            {
                return false;
            }

            if (!genericTypeDefinition.IsGenericTypeDefinition)
            {
                return genericTypeDefinition.IsAssignableFrom(givenType);
            }

            // Check the type itself and walk up inheritance chain
            var currentType = givenType;

            while (currentType != null && currentType != typeof(object))
            {
                if (currentType.IsGenericType &&
                    currentType.GetGenericTypeDefinition() == genericTypeDefinition)
                {
                    return true;
                }

                currentType = currentType.BaseType;
            }

            // Also check implemented interfaces
            foreach (var iface in givenType.GetInterfaces())
            {
                if (iface.IsGenericType && iface.GetGenericTypeDefinition() == genericTypeDefinition)
                {
                    return true;
                }
            }

            return false;
        }
    }
}