using System;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Data.Configuration;
using UnityEditor;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Core
{
    // Persist lightweight cache across domain reloads
    [FilePath("ProjectSettings/StateMachineContainerSingleton.asset", FilePathAttribute.Location.ProjectFolder)]
    public class StateMachineContainerSingleton : ScriptableSingleton<StateMachineContainerSingleton>
    {
        [NonSerialized]
        private StateMachineContainer _workingCopy;

        // Persisted across reloads
        [SerializeField]
        private string _originalContainerGuid;

        [SerializeField]
        private string _workingJson;

        public StateMachineContainer OriginalContainer { get; private set; }

        [field: NonSerialized]
        public SerializedObject WorkingSerializedObject { get; private set; }

        static StateMachineContainerSingleton()
        {
            AssemblyReloadEvents.beforeAssemblyReload += () => instance.OnBeforeAssemblyReload();
            AssemblyReloadEvents.afterAssemblyReload += () => instance.OnAfterAssemblyReload();
        }

        public bool TryApply(SerializedObject stateMachineContainer)
        {
            if (stateMachineContainer == null ||
                stateMachineContainer.targetObject is not StateMachineContainer selectedContainer)
            {
                return false;
            }

            var path = AssetDatabase.GetAssetPath(selectedContainer);
            var guid = AssetDatabase.AssetPathToGUID(path);

            if (OriginalContainer == null || OriginalContainer != selectedContainer)
            {
                OriginalContainer = selectedContainer;
                _originalContainerGuid = guid;
                RecreateWorkingFromOriginal();
                // If we have cached json for this guid, apply it
                ApplyCachedJsonIfMatches();
            }
            else if (_workingCopy == null || WorkingSerializedObject == null)
            {
                RecreateWorkingFromOriginal();
                ApplyCachedJsonIfMatches();
            }

            return true;
        }

        private void OnEnable()
        {
            // When domain reloads, ScriptableSingleton data is restored. Recreate runtime objects.
            TryRestoreWorkingFromCache();
        }

        private void RecreateWorkingFromOriginal()
        {
            if (_workingCopy != null)
            {
                DestroyImmediate(_workingCopy);
                _workingCopy = null;
                WorkingSerializedObject = null;
            }

            if (OriginalContainer == null)
            {
                return;
            }

            _workingCopy = Instantiate(OriginalContainer);
            _workingCopy.name = OriginalContainer.name + " (Working Copy)";
            WorkingSerializedObject = new SerializedObject(_workingCopy);
        }

        private void ApplyCachedJsonIfMatches()
        {
            if (!string.IsNullOrEmpty(_workingJson) && _workingCopy != null)
            {
                JsonUtility.FromJsonOverwrite(_workingJson, _workingCopy);
                WorkingSerializedObject = new SerializedObject(_workingCopy);
            }
        }

        private void OnBeforeAssemblyReload()
        {
            if (OriginalContainer == null || _workingCopy == null)
            {
                _workingJson = null;
                Save(true);

                return;
            }

            var path = AssetDatabase.GetAssetPath(OriginalContainer);
            _originalContainerGuid = AssetDatabase.AssetPathToGUID(path);

            // Cache current working copy as JSON
            _workingJson = JsonUtility.ToJson(_workingCopy);
            Save(true);
        }

        private void OnAfterAssemblyReload()
        {
            // ScriptableSingleton will rehydrate _originalContainerGuid and _workingJson
            TryRestoreWorkingFromCache();
        }

        private void TryRestoreWorkingFromCache()
        {
            if (string.IsNullOrEmpty(_originalContainerGuid))
            {
                return;
            }

            var path = AssetDatabase.GUIDToAssetPath(_originalContainerGuid);
            var original = AssetDatabase.LoadAssetAtPath<StateMachineContainer>(path);

            if (original == null)
            {
                // Asset was moved/deleted; clear cache
                _originalContainerGuid = null;
                _workingJson = null;
                Save(true);

                return;
            }

            OriginalContainer = original;
            RecreateWorkingFromOriginal();
            ApplyCachedJsonIfMatches();
        }
    }
}