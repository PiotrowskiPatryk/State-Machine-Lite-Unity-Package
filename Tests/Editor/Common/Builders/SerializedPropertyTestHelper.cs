using System;
using Dev.Cortez.StateMachines.Core.StateMachineConfiguration.Definition;
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
            var wrapper = ScriptableObject.CreateInstance<DefinitionWrapper<TData>>();
            var so = new SerializedObject(wrapper);
            var property = so.FindProperty(DefinitionWrapper<TData>.DATA_PROPERTY_NAME);

            return (so, property);
        }

        /// <summary>
        /// Creates a DefinitionWrapper ScriptableObject instance for testing.
        /// Remember to call <see cref="Cleanup"/> when done to destroy the temporary object.
        /// </summary>
        /// <typeparam name="TData">The type of data to wrap</typeparam>
        /// <returns>The wrapper instance</returns>
        public static DefinitionWrapper<TData> CreateDefinitionWrapper<TData>()
            where TData : class, new()
        {
            return ScriptableObject.CreateInstance<DefinitionWrapper<TData>>();
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
        public DefinitionWrapper<TData> Wrapper { get; }

        /// <summary>
        /// The SerializedObject for the wrapper.
        /// </summary>
        public SerializedObject SerializedObject { get; }

        /// <summary>
        /// The SerializedProperty for the data.
        /// </summary>
        public SerializedProperty Property { get; }

        /// <summary>
        /// Direct access to the data instance.
        /// </summary>
        public TData Data => Wrapper.Data;

        public TestPropertyContext()
        {
            Wrapper = ScriptableObject.CreateInstance<DefinitionWrapper<TData>>();
            SerializedObject = new SerializedObject(Wrapper);
            Property = SerializedObject.FindProperty(DefinitionWrapper<TData>.DATA_PROPERTY_NAME);
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
