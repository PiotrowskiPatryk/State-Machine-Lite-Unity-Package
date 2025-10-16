using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.UIToolkitUtilities
{
    public static class MultiColumnListViewExtensions
    {
        public static void BindColumnWithProperty(this MultiColumnListView multiColumnListView, string columnId,
            SerializedProperty property, string propertyName)
        {
            multiColumnListView.itemsSource = Enumerable.Range(0, property.arraySize).ToList();
            
            var idColumn = multiColumnListView.columns.FirstOrDefault(column => column.bindingPath.Equals(columnId));

            if (idColumn != null)
            {
                idColumn.bindCell = (visualElement, i) =>
                {
                    if (i < 0 || i >= property.arraySize) return;
                    var elementProp = property.GetArrayElementAtIndex(i);
                    var idProp = elementProp.FindPropertyRelative(columnId);
                    var label = visualElement as Label ?? visualElement.Q<Label>();
                    if (label != null)
                        label.text = idProp != null ? idProp.stringValue : "(null)";
                };

                idColumn.unbindCell = (visualElement, i) =>
                {
                    var label = visualElement as Label ?? visualElement.Q<Label>();
                    if (label != null)
                    {
                        label.text = string.Empty;
                    }
                };
            }
            else
            {
                Debug.LogError("Unable to find column with id: " + columnId + "");
            }
        }
    }
}