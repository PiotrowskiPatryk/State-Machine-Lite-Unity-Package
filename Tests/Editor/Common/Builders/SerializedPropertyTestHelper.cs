using System;
using Dev.Cortez.StateMachines.Core.StateMachineConfiguration;
using Dev.Cortez.StateMachines.Core.StateMachineConfiguration.Definition;
using Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Common.Data;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Common.Builders
{
    /// <summary>
    /// Utility class for creating SerializedProperty instances in tests.
    /// Provides helper methods to create temporary ScriptableObject wrappers
    /// that can be used to test SerializedProperty-based code.
    /// </summary>
    public static class SerializedPropertyTestHelper
    {
        /// <summary>
        /// Creates a test SerializedProperty by wrapping the data in a temporary ScriptableObject.
        /// Remember to call <see cref="Cleanup"/> when done to destroy the temporary object.
        /// </summary>
        /// <typeparam name="TData">The type of data to wrap (must have parameterless constructor)</typeparam>
        /// <returns>A tuple containing the SerializedObject and the SerializedProperty for the data</returns>
        public static (SerializedObject serializedObject, SerializedProperty property) CreateTestProperty<TData>()
            where TData : class, new()
        {
            var context = CreateTestContext<TData>();
            return (context.SerializedObject, context.Property);
        }

        /// <summary>
        /// Cleans up a SerializedObject by destroying its target object.
        /// Call this in test teardown to prevent memory leaks.
        /// </summary>
        /// <param name="serializedObject">The SerializedObject to clean up</param>
        public static void Cleanup(SerializedObject serializedObject)
        {
            if (serializedObject?.targetObject != null)
            {
                Object.DestroyImmediate(serializedObject.targetObject);
            }

            serializedObject?.Dispose();
        }

        /// <summary>
        /// Cleans up a ScriptableObject by destroying it.
        /// Call this in test teardown to prevent memory leaks.
        /// </summary>
        /// <param name="scriptableObject">The ScriptableObject to destroy</param>
        public static void Cleanup(ScriptableObject scriptableObject)
        {
            if (scriptableObject != null)
            {
                Object.DestroyImmediate(scriptableObject);
            }
        }

        /// <summary>
        /// Creates a complete test context for ViewModel testing.
        /// Returns a disposable context that automatically cleans up on dispose.
        /// </summary>
        /// <typeparam name="TData">The type of data to wrap</typeparam>
        /// <returns>A disposable test context</returns>
        public static TestPropertyContext<TData> CreateTestContext<TData>()
            where TData : class, new()
        {
            return new TestPropertyContext<TData>();
        }
    }

    /// <summary>
    /// Disposable test context that manages SerializedObject lifecycle.
    /// Use with 'using' statement for automatic cleanup.
    /// </summary>
    /// <typeparam name="TData">The type of data being wrapped</typeparam>
    public sealed class TestPropertyContext<TData> : IDisposable
        where TData : class, new()
    {
        private bool _disposed;

        /// <summary>
        /// The wrapper ScriptableObject.
        /// </summary>
        public TestWrapperBase Wrapper { get; }

        /// <summary>
        /// The SerializedObject for the wrapper.
        /// </summary>
        public SerializedObject SerializedObject { get; }

        /// <summary>
        /// The SerializedProperty for the data.
        /// </summary>
        public SerializedProperty Property { get; }

        public TestPropertyContext()
        {
            // Create the appropriate concrete wrapper based on TData type
            Wrapper = CreateWrapper();

            if (Wrapper == null)
            {
                throw new InvalidOperationException(
                    $"No test wrapper defined for type {typeof(TData).FullName}. " +
                    "Add a concrete wrapper class to TestDefinitionWrapper.cs.");
            }

            SerializedObject = new SerializedObject(Wrapper);
            Property = SerializedObject.FindProperty(GetDataPropertyName());

            if (Property == null)
            {
                throw new InvalidOperationException(
                    $"Could not find Data property in wrapper for type {typeof(TData).FullName}.");
            }
        }

        private TestWrapperBase CreateWrapper()
        {
            var dataType = typeof(TData);

            if (dataType == typeof(ConditionDefinition))
            {
                return ScriptableObject.CreateInstance<ConditionDefinitionWrapper>();
            }

            if (dataType == typeof(StateDefinition))
            {
                return ScriptableObject.CreateInstance<StateDefinitionWrapper>();
            }

            if (dataType == typeof(TransitionRuleDefinition))
            {
                return ScriptableObject.CreateInstance<TransitionRuleDefinitionWrapper>();
            }

            if (dataType == typeof(TriggerDefinition))
            {
                return ScriptableObject.CreateInstance<TriggerDefinitionWrapper>();
            }

            if (dataType == typeof(StateMachineDefinition))
            {
                return ScriptableObject.CreateInstance<StateMachineDefinitionWrapper>();
            }

            if (dataType == typeof(TriggerConfiguration))
            {
                return ScriptableObject.CreateInstance<TriggerConfigurationWrapper>();
            }

            if (dataType == typeof(StateMachineConfiguration))
            {
                return ScriptableObject.CreateInstance<StateMachineConfigurationWrapper>();
            }

            if (dataType == typeof(AllPropertyTypesData))
            {
                return ScriptableObject.CreateInstance<AllPropertyTypesDataWrapper>();
            }

            return null;
        }

        private string GetDataPropertyName()
        {
            // All wrappers use the same property name
            return "Data";
        }

        /// <summary>
        /// Updates the SerializedObject to reflect any changes to the underlying data.
        /// </summary>
        public void Update()
        {
            SerializedObject.Update();
        }

        /// <summary>
        /// Applies any modifications made through SerializedProperty back to the data.
        /// </summary>
        public void ApplyModifiedProperties()
        {
            SerializedObject.ApplyModifiedProperties();
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            SerializedObject?.Dispose();

            if (Wrapper != null)
            {
                Object.DestroyImmediate(Wrapper);
            }
        }
    }
}
