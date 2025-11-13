using Dev.Cortez.StateMachines.Core.ReferencePicker;
using Dev.Cortez.StateMachines.Core.StateMachineConfiguration;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Data.Repository;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.PropertyDrawer
{
    public abstract class ReferencePickerPropertyDrawerBase : UnityEditor.PropertyDrawer
    {
        protected SerializedProperty SerializedProperty { get; private set; }
        protected StateMachineContainer SelectedStateMachineContainer { get; private set; }

        [ItemCanBeNull]
        private string SelectedStateMachineContainerGuid
        {
            get => SerializedProperty.FindPropertyRelative(ReferencePickerBase.
                SELECTED_STATE_MACHINE_CONTAINER_GUID_PROPERTY_NAME).stringValue;

            set
            {
                if (value == SelectedStateMachineContainerGuid)
                {
                    return;
                }

                SerializedProperty.serializedObject.Update();
                SerializedProperty.
                    FindPropertyRelative(ReferencePickerBase.SELECTED_STATE_MACHINE_CONTAINER_GUID_PROPERTY_NAME).
                    stringValue = value;
                SerializedProperty.serializedObject.ApplyModifiedProperties();

                LoadStateMachineContainerData(value);
            }
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedProperty = property;

            var propertyDrawer = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(StateMachineEditorViewRepository.
                REFERENCE_PICKER_PROPERTY_DRAWER_PATH).Instantiate();
            var contentContainer = propertyDrawer.contentContainer;

            InitializeStateMachineContainerDropdown(contentContainer);

            propertyDrawer.contentContainer.Q<Foldout>("Foldout").text = property.displayName;

            return propertyDrawer;
        }

        private void LoadStateMachineContainerData(string guid)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);

            SelectedStateMachineContainer =
                AssetDatabase.LoadAssetAtPath<StateMachineContainer>(assetPath);
            OnStateMachineContainerChanged();
        }

        private void InitializeStateMachineContainerDropdown(VisualElement visualElement)
        {
            var dropdownField = visualElement.Q<DropdownField>("StateMachineContainerDropdownField");
            var stateMachineContainers = AssetDatabase.FindAssets($"t:{nameof(StateMachineContainer)}");

            dropdownField.choices.Clear();

            foreach (var stateMachineContainer in stateMachineContainers)
            {
                var stateMachineContainerPath = AssetDatabase.GUIDToAssetPath(stateMachineContainer);

                dropdownField.choices.Add(stateMachineContainerPath);
            }

            if (SelectedStateMachineContainerGuid != null)
            {
                var selectedStateMachineContainerPath =
                    AssetDatabase.GUIDToAssetPath(SelectedStateMachineContainerGuid);

                dropdownField.SetValueWithoutNotify(selectedStateMachineContainerPath);
                LoadStateMachineContainerData(SelectedStateMachineContainerGuid);
            }

            dropdownField.RegisterValueChangedCallback(valueChangedCallback =>
            {
                SelectedStateMachineContainerGuid = AssetDatabase.AssetPathToGUID(valueChangedCallback.newValue);
            });
        }

        private void OnStateMachineContainerChanged()
        {
            Debug.Log("State Machine Container changed");
        }
    }
}