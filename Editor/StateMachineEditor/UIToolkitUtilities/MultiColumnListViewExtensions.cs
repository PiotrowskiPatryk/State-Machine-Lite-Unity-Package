using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.UIToolkitUtilities
{
    public static class MultiColumnListViewExtensions
    {
        public static void SetItemsSourceFromArray(this MultiColumnListView listView, SerializedProperty arrayProp)
        {
            listView.itemsSource = Enumerable.Range(0, arrayProp.arraySize).ToList();
        }

        public static void ResetAndRefreshFromArray(this MultiColumnListView listView, SerializedProperty arrayProp)
        {
            listView.SetItemsSourceFromArray(arrayProp);
            listView.RefreshItems();
        }

        public static void BindColumnWithProperty(this MultiColumnListView multiColumnListView, string columnId,
            SerializedProperty property, string propertyName, bool isTypeValue = false)
        {
            multiColumnListView.itemsSource = Enumerable.Range(0, property.arraySize).ToList();

            var idColumn = multiColumnListView.columns.FirstOrDefault(column => column.bindingPath.Equals(columnId));

            if (idColumn != null)
            {
                idColumn.bindCell = (visualElement, i) =>
                {
                    if (i < 0 || i >= property.arraySize)
                    {
                        return;
                    }

                    var elementProp = property.GetArrayElementAtIndex(i);
                    var relProp = elementProp.FindPropertyRelative(columnId);
                    var label = visualElement as Label ?? visualElement.Q<Label>();

                    if (label == null)
                    {
                        return;
                    }

                    if (relProp == null)
                    {
                        label.text = "(null)";

                        return;
                    }

                    if (isTypeValue)
                    {
                        // Render a friendly type display name instead of the full assembly-qualified string
                        var type = TypePickerUtility.TryGetTypeFromProperty(relProp);

                        if (type != null)
                        {
                            label.text = TypePickerUtility.GetNiceDisplayName(type);
                        }
                        else
                        {
                            var raw = relProp.stringValue;

                            if (string.IsNullOrEmpty(raw))
                            {
                                label.text = "None";
                            }
                            else
                            {
                                // Trim assembly part and show the full type name
                                var nameOnly = raw;
                                var commaIdx = raw.IndexOf(',');

                                if (commaIdx > 0)
                                {
                                    nameOnly = raw.Substring(0, commaIdx).Trim();
                                }

                                label.text = nameOnly;
                            }
                        }
                    }
                    else
                    {
                        label.text = relProp.propertyType == SerializedPropertyType.String
                            ? relProp.stringValue
                            : relProp.displayName;
                    }
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