using Dev.Cortez.StateMachines.Editor.StateMachineEditor.UIToolkitUtilities;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Data;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Utilities;
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

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = _visualTreeAsset.CloneTree();
            var listView = root.Q<MultiColumnListView>("ListView");
            var newTriggerButton = root.Q<Button>("NewTriggerButton");
            var triggersProperty = property.FindPropertyRelative("_triggers");

            // Bind columns (keeps nice type name rendering via extension)
            listView.BindColumnWithProperty(TriggerDefinitionPropertyNames.Id, triggersProperty, TriggerDefinitionPropertyNames.Id);
            listView.BindColumnWithProperty(TriggerDefinitionPropertyNames.Name, triggersProperty, TriggerDefinitionPropertyNames.Name);
            listView.BindColumnWithProperty(TriggerDefinitionPropertyNames.TypeName, triggersProperty, TriggerDefinitionPropertyNames.TypeName);
            listView.BindColumnWithProperty(TriggerDefinitionPropertyNames.Description, triggersProperty, TriggerDefinitionPropertyNames.Description);

            void Refresh()
            {
                listView.ResetAndRefreshFromArray(triggersProperty);
            }

            // Wire Edit/Delete buttons via reusable utility
            RowActionButtonsUtility.WireLastColumnEditDelete(listView, triggersProperty,
                onEdit: index =>
                {
                    TriggerDefinitionEditorUtility.OpenEditForm(triggersProperty, index, result =>
                    {
                        var so = triggersProperty.serializedObject;
                        so.Update();
                        var target = triggersProperty.GetArrayElementAtIndex(index);
                        TriggerDefinitionEditorUtility.WriteFromResult(target, result);
                        so.ApplyModifiedProperties();
                        Refresh();
                    });
                },
                onDelete: index =>
                {
                    var element = triggersProperty.GetArrayElementAtIndex(index);
                    var nameProp = element.FindPropertyRelative(TriggerDefinitionPropertyNames.Name);
                    var name = nameProp != null ? nameProp.stringValue : string.Empty;
                    var title = "Delete Trigger";
                    var message = string.IsNullOrEmpty(name)
                        ? "Are you sure you want to delete this trigger?"
                        : $"Are you sure you want to delete '{name}'?";

                    var confirm = EditorUtility.DisplayDialog(title, message, "Delete", "Cancel");
                    
                    if (!confirm)
                    {
                        return;
                    }

                    var so = triggersProperty.serializedObject;
                    so.Update();
                    triggersProperty.DeleteArrayElementAtIndex(index);
                    so.ApplyModifiedProperties();
                    Refresh();
                });

            // Add new trigger
            newTriggerButton.clicked += () =>
            {
                TriggerForm.Show(result =>
                {
                    var so = triggersProperty.serializedObject;
                    so.Update();

                    var index = triggersProperty.arraySize;
                    triggersProperty.InsertArrayElementAtIndex(index);
                    var element = triggersProperty.GetArrayElementAtIndex(index);

                    TriggerDefinitionEditorUtility.WriteFromResult(element, result);
                    so.ApplyModifiedProperties();
                    Refresh();
                });
            };

            // Open edit form on item activation (double-click or Enter)
            listView.onItemsChosen += _ =>
            {
                var index = listView.selectedIndex;
                if (index < 0 || index >= triggersProperty.arraySize) return;

                TriggerDefinitionEditorUtility.OpenEditForm(triggersProperty, index, result =>
                {
                    var so = triggersProperty.serializedObject;
                    so.Update();
                    var target = triggersProperty.GetArrayElementAtIndex(index);
                    TriggerDefinitionEditorUtility.WriteFromResult(target, result);
                    so.ApplyModifiedProperties();
                    Refresh();
                });
            };

            return root;
        }
    }
}
