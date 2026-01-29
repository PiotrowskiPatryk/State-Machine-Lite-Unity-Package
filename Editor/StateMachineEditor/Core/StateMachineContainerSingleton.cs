using System;
using Dev.Cortez.StateMachines.Core.StateMachineConfiguration;
using UnityEditor;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Core
{
    /// <summary>
    /// Singleton that maintains the currently edited StateMachineContainer across domain reloads.
    /// Decoupled from Inspector selection - changes are applied immediately to the original asset.
    /// </summary>
    public sealed class StateMachineContainerSingleton : ScriptableSingleton<StateMachineContainerSingleton>
    {
        // This persists through domain reload (ScriptableSingleton serializes it)
        public StateMachineContainer OriginalContainer { get; private set; }

        // Non-serialized - will be recreated after domain reload from OriginalContainer
        [field: NonSerialized]
        public SerializedObject WorkingSerializedObject { get; private set; }

        /// <summary>
        /// Applies a new container to edit. Called when user opens editor from PropertyDrawer.
        /// Creates an independent SerializedObject that is not tied to Inspector selection.
        /// </summary>
        public bool TryApply(SerializedObject stateMachineContainer)
        {
            if (stateMachineContainer is not { targetObject: StateMachineContainer selectedContainer })
            {
                return false;
            }

            OriginalContainer = selectedContainer;
            // Create independent SerializedObject from the container directly
            // This ensures changes are applied immediately and the object is not tied to Inspector selection
            WorkingSerializedObject = new SerializedObject(OriginalContainer);

            return true;
        }

        /// <summary>
        /// Ensures WorkingSerializedObject is valid. Called after domain reload.
        /// Recreates the SerializedObject from the persisted OriginalContainer.
        /// </summary>
        /// <returns>True if a valid SerializedObject exists or was recreated.</returns>
        public bool EnsureWorkingSerializedObject()
        {
            // If container was cleared or destroyed, nothing to restore
            if (OriginalContainer == null)
            {
                return false;
            }

            // Recreate SerializedObject if null or stale
            if (WorkingSerializedObject == null ||
                WorkingSerializedObject.targetObject == null ||
                WorkingSerializedObject.targetObject != OriginalContainer)
            {
                WorkingSerializedObject = new SerializedObject(OriginalContainer);
            }

            return true;
        }

        /// <summary>
        /// Clears the current container. Call when user explicitly closes the editor.
        /// </summary>
        public void Clear()
        {
            OriginalContainer = null;
            WorkingSerializedObject = null;
        }
    }
}