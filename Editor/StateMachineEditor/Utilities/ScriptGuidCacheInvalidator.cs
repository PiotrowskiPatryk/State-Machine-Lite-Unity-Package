using System.Linq;
using Dev.Cortez.StateMachines.Core.Utilities;
using UnityEditor;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Utilities
{
    /// <summary>
    /// Handles automatic cache invalidation for ScriptGuidUtility when scripts are reimported.
    /// </summary>
    internal sealed class ScriptGuidCacheInvalidator : AssetPostprocessor
    {
        /// <summary>
        /// Called when assets are imported, deleted, or moved.
        /// Clears the GUID cache if any scripts were affected.
        /// </summary>
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            var scriptsChanged = importedAssets.Any(path => path.EndsWith(".cs")) ||
                                 deletedAssets.Any(path => path.EndsWith(".cs")) ||
                                 movedAssets.Any(path => path.EndsWith(".cs"));

            if (scriptsChanged)
            {
                ScriptGuidUtility.ClearCache();
            }
        }
    }
}