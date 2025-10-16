using System.Linq;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.UIToolkitUtilities;
using Dev.Cortez.StateMachines.StateMachineEditor.Data;
using UnityEditor;
using UnityEngine.UIElements;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Windows;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(TriggerConfiguration))]
    public class TriggerConfigurationPropertyDrawer : PropertyDrawer
    {
        [SerializeField]
        private VisualTreeAsset _visualTreeAsset;
        
        private SerializedObject _serializedObject;

        private static void ResetItemsSource(MultiColumnListView listView, SerializedProperty arrayProp)
        {
            listView.itemsSource = Enumerable.Range(0, arrayProp.arraySize).ToList();
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = _visualTreeAsset.CloneTree();

            var multiColumnListView = root.Q<MultiColumnListView>("ListView");
            var newTriggerButton = root.Q<Button>("NewTriggerButton");

            var triggersProperty = property.FindPropertyRelative("_triggers");

            multiColumnListView.BindColumnWithProperty("_id", triggersProperty, "_id");
            multiColumnListView.BindColumnWithProperty("_name", triggersProperty, "_name");
            multiColumnListView.BindColumnWithProperty("_typeName", triggersProperty, "_typeName");
            multiColumnListView.BindColumnWithProperty("_description", triggersProperty, "_description");

            // Wire action buttons in the last column (ItemButtons.uxml)
            if (multiColumnListView.columns.Count > 0)
            {
                var buttonsColumn = multiColumnListView.columns[multiColumnListView.columns.Count - 1];

                buttonsColumn.bindCell = (ve, i) =>
                {
                    var editBtn = ve.Q<Button>("Button-Edit");
                    var deleteBtn = ve.Q<Button>("Button-Delete");

                    // Clean up previous callbacks if any
                    if (ve.userData is ButtonHandlers oldHandlers)
                    {
                        if (editBtn != null && oldHandlers.EditCallback != null)
                            editBtn.UnregisterCallback<ClickEvent>(oldHandlers.EditCallback);
                        if (deleteBtn != null && oldHandlers.DeleteCallback != null)
                            deleteBtn.UnregisterCallback<ClickEvent>(oldHandlers.DeleteCallback);
                    }

                    EventCallback<ClickEvent> onEdit = _ =>
                    {
                        if (i < 0 || i >= triggersProperty.arraySize) return;
                        var element = triggersProperty.GetArrayElementAtIndex(i);
                        var id = element.FindPropertyRelative("_id").stringValue;
                        var name = element.FindPropertyRelative("_name").stringValue;
                        var description = element.FindPropertyRelative("_description").stringValue;
                        var typeName = element.FindPropertyRelative("_typeName").stringValue;
                        var payloadProp0 = element.FindPropertyRelative("_payload");
                        IPayload payload0 = null;
                        if (payloadProp0 != null)
                        {
                            try { payload0 = payloadProp0.managedReferenceValue as IPayload; } catch { }
                        }

                        TriggerForm.ShowEdit(id, name, description, typeName, payload0, result =>
                        {
                            var so = triggersProperty.serializedObject;
                            so.Update();

                            var target = triggersProperty.GetArrayElementAtIndex(i);
                            target.FindPropertyRelative("_id").stringValue = result.Id;
                            target.FindPropertyRelative("_name").stringValue = result.Name;
                            target.FindPropertyRelative("_description").stringValue = result.Description;
                            target.FindPropertyRelative("_typeName").stringValue = result.TypeName;
                            var payloadProp = target.FindPropertyRelative("_payload");
                            if (payloadProp != null)
                            {
                                try { payloadProp.managedReferenceValue = result.Payload; } catch { }
                            }

                            so.ApplyModifiedProperties();
                            ResetItemsSource(multiColumnListView, triggersProperty);
                            multiColumnListView.RefreshItems();
                        });
                    };

                    EventCallback<ClickEvent> onDelete = _ =>
                    {
                        if (i < 0 || i >= triggersProperty.arraySize) return;

                        // Ask for confirmation before deletion
                        var element = triggersProperty.GetArrayElementAtIndex(i);
                        var nameProp = element.FindPropertyRelative("_name");
                        var name = nameProp != null ? nameProp.stringValue : string.Empty;
                        var title = "Delete Trigger";
                        var message = string.IsNullOrEmpty(name)
                            ? "Are you sure you want to delete this trigger?"
                            : $"Are you sure you want to delete '{name}'?";

                        var confirm = EditorUtility.DisplayDialog(title, message, "Delete", "Cancel");
                        if (!confirm) return;

                        var so = triggersProperty.serializedObject;
                        so.Update();
                        triggersProperty.DeleteArrayElementAtIndex(i);
                        so.ApplyModifiedProperties();
                        ResetItemsSource(multiColumnListView, triggersProperty);
                        multiColumnListView.RefreshItems();
                    };

                    if (editBtn != null) editBtn.RegisterCallback<ClickEvent>(onEdit);
                    if (deleteBtn != null) deleteBtn.RegisterCallback<ClickEvent>(onDelete);

                    ve.userData = new ButtonHandlers
                    {
                        EditCallback = onEdit,
                        DeleteCallback = onDelete
                    };
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

            newTriggerButton.clicked += () =>
            {
                TriggerForm.Show(result =>
                {
                    // Update serialized object, add new element, set fields, apply, and refresh the view
                    var so = triggersProperty.serializedObject;
                    so.Update();

                    var index = triggersProperty.arraySize;
                    triggersProperty.InsertArrayElementAtIndex(index);
                    var element = triggersProperty.GetArrayElementAtIndex(index);

                    element.FindPropertyRelative("_id").stringValue = result.Id;
                    element.FindPropertyRelative("_name").stringValue = result.Name;
                    element.FindPropertyRelative("_description").stringValue = result.Description;
                    element.FindPropertyRelative("_typeName").stringValue = result.TypeName;
                    var payloadProp = element.FindPropertyRelative("_payload");
                    if (payloadProp != null)
                    {
                        try { payloadProp.managedReferenceValue = result.Payload; } catch { }
                    }

                    so.ApplyModifiedProperties();

                    // Refresh list to reflect the new item
                    ResetItemsSource(multiColumnListView, triggersProperty);
                    multiColumnListView.RefreshItems();
                });
            };

            // Open edit form on item activation (double-click or Enter)
            multiColumnListView.onItemsChosen += _ =>
            {
                var index = multiColumnListView.selectedIndex;
                if (index < 0 || index >= triggersProperty.arraySize)
                    return;

                var element = triggersProperty.GetArrayElementAtIndex(index);
                var id = element.FindPropertyRelative("_id").stringValue;
                var name = element.FindPropertyRelative("_name").stringValue;
                var description = element.FindPropertyRelative("_description").stringValue;
                var typeName = element.FindPropertyRelative("_typeName").stringValue;
                var payloadProp2Pre = element.FindPropertyRelative("_payload");
                IPayload payload2 = null;
                if (payloadProp2Pre != null)
                {
                    try { payload2 = payloadProp2Pre.managedReferenceValue as IPayload; } catch { }
                }

                TriggerForm.ShowEdit(id, name, description, typeName, payload2, result =>
                {
                    var so = triggersProperty.serializedObject;
                    so.Update();

                    var target = triggersProperty.GetArrayElementAtIndex(index);
                    target.FindPropertyRelative("_id").stringValue = result.Id;
                    target.FindPropertyRelative("_name").stringValue = result.Name;
                    target.FindPropertyRelative("_description").stringValue = result.Description;
                    target.FindPropertyRelative("_typeName").stringValue = result.TypeName;
                    var payloadProp2 = target.FindPropertyRelative("_payload");
                    if (payloadProp2 != null)
                    {
                        try { payloadProp2.managedReferenceValue = result.Payload; } catch { }
                    }

                    so.ApplyModifiedProperties();
                    ResetItemsSource(multiColumnListView, triggersProperty);
                    multiColumnListView.RefreshItems();
                });
            };
            
            return root;
        }

        private class ButtonHandlers
        {
            public EventCallback<ClickEvent> EditCallback;
            public EventCallback<ClickEvent> DeleteCallback;
        }
    }
}
