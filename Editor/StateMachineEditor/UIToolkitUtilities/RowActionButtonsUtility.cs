using UnityEditor;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.UIToolkitUtilities
{
    /// <summary>
    /// Utility to wire per-row Edit/Delete buttons for a MultiColumnListView last column using ItemButtons.uxml template.
    /// Keeps registration/unregistration safe for recycled rows.
    /// </summary>
    public static class RowActionButtonsUtility
    {
        private class ButtonHandlers
        {
            public EventCallback<ClickEvent> EditCallback;
            public EventCallback<ClickEvent> DeleteCallback;
        }

        /// <summary>
        /// Wires the last column of the provided list view to invoke the supplied callbacks when Edit/Delete are pressed.
        /// </summary>
        /// <param name="listView">Target MultiColumnListView.</param>
        /// <param name="arrayProp">Serialized array property backing the list.</param>
        /// <param name="onEdit">Called with row index when Edit is clicked.</param>
        /// <param name="onDelete">Called with row index when Delete is clicked. Should perform its own confirmation.</param>
        public static void WireLastColumnEditDelete(MultiColumnListView listView, SerializedProperty arrayProp, System.Action<int> onEdit, System.Action<int> onDelete)
        {
            if (listView == null || listView.columns == null || listView.columns.Count == 0)
                return;

            var buttonsColumn = listView.columns[listView.columns.Count - 1];

            buttonsColumn.bindCell = (ve, i) =>
            {
                var editBtn = ve.Q<Button>("Button-Edit");
                var deleteBtn = ve.Q<Button>("Button-Delete");

                // Clean previous if any
                if (ve.userData is ButtonHandlers old)
                {
                    if (editBtn != null && old.EditCallback != null) editBtn.UnregisterCallback<ClickEvent>(old.EditCallback);
                    if (deleteBtn != null && old.DeleteCallback != null) deleteBtn.UnregisterCallback<ClickEvent>(old.DeleteCallback);
                }

                EventCallback<ClickEvent> edit = _ =>
                {
                    if (i < 0 || i >= arrayProp.arraySize) return;
                    onEdit?.Invoke(i);
                };

                EventCallback<ClickEvent> delete = _ =>
                {
                    if (i < 0 || i >= arrayProp.arraySize) return;
                    onDelete?.Invoke(i);
                };

                if (editBtn != null) editBtn.RegisterCallback<ClickEvent>(edit);
                if (deleteBtn != null) deleteBtn.RegisterCallback<ClickEvent>(delete);

                ve.userData = new ButtonHandlers { EditCallback = edit, DeleteCallback = delete };
            };

            buttonsColumn.unbindCell = (ve, i) =>
            {
                var editBtn = ve.Q<Button>("Button-Edit");
                var deleteBtn = ve.Q<Button>("Button-Delete");
                if (ve.userData is ButtonHandlers handlers)
                {
                    if (editBtn != null && handlers.EditCallback != null)
                        editBtn.UnregisterCallback<ClickEvent>(handlers.EditCallback);
                    if (deleteBtn != null && handlers.DeleteCallback != null)
                        deleteBtn.UnregisterCallback<ClickEvent>(handlers.DeleteCallback);
                    ve.userData = null;
                }
            };
        }
    }
}
