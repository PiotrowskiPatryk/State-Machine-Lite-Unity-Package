using System.Collections.Generic;
using System.Linq;
using Dev.Cortez.StateMachines.Core.ReferencePicker;
using Dev.Cortez.StateMachines.Core.StateMachineConfiguration;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Data.Repository;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.PropertyDrawer
{
    public abstract class ReferencePickerPropertyDrawerBase : UnityEditor.PropertyDrawer
    {
        private DropdownField _referenceDropdownField;

        private Dictionary<string, string> _referenceDropdownOptions;

        protected SerializedProperty SerializedProperty { get; private set; }

        protected StateMachineContainer SelectedStateMachineContainer { get; private set; }

        private string SelectedItemReferenceId
        {
            get => SerializedProperty.FindPropertyRelative(ReferencePickerBase.SELECTED_ITEM_ID_PROPERTY_NAME).
                stringValue;

            set
            {
                if (value == SelectedItemReferenceId)
                {
                    return;
                }

                SerializedProperty.serializedObject.Update();
                SerializedProperty.FindPropertyRelative(ReferencePickerBase.SELECTED_ITEM_ID_PROPERTY_NAME).
                    stringValue = value;
                SerializedProperty.serializedObject.ApplyModifiedProperties();
            }
        }

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

            InitializeReferenceDropdown(contentContainer);
            InitializeStateMachineContainerDropdown(contentContainer);

            contentContainer.Q<Foldout>("Foldout").text = property.displayName;

            return propertyDrawer;
        }

        protected abstract Dictionary<string, string> GetOptions(StateMachineContainer stateMachineContainer);

        private void InitializeReferenceDropdown(VisualElement visualElement)
        {
            _referenceDropdownField = visualElement.Q<DropdownField>("ReferenceDropdownField");
            _referenceDropdownField.choices.Clear();

            _referenceDropdownField.RegisterValueChangedCallback(valueChangedCallback =>
            {
                var referenceName = valueChangedCallback.newValue;
                var referenceEntry = _referenceDropdownOptions.FirstOrDefault(kvp => kvp.Value.Equals(referenceName));
                SelectedItemReferenceId = referenceEntry.Key;
            });
        }

        private void LoadStateMachineContainerData(string guid)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);

            SelectedStateMachineContainer =
                AssetDatabase.LoadAssetAtPath<StateMachineContainer>(assetPath);
            OnStateMachineContainerChanged(SelectedStateMachineContainer);
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

        private void OnStateMachineContainerChanged(StateMachineContainer stateMachineContainer)
        {
            if (stateMachineContainer == null)
            {
                Debug.LogWarning("Selected state machine container is null. Select state machine container first");

                return;
            }

            UpdateReferencesDropdown(stateMachineContainer);
        }

        private void UpdateReferencesDropdown(StateMachineContainer stateMachineContainer)
        {
            _referenceDropdownField.choices.Clear();
            _referenceDropdownOptions = GetOptions(stateMachineContainer);

            foreach (var option in _referenceDropdownOptions)
            {
                _referenceDropdownField.choices.Add(option.Value);
            }

            if (_referenceDropdownOptions.TryGetValue(SelectedItemReferenceId, out var dropdownValue))
            {
                _referenceDropdownField.value = dropdownValue;
            }
            else
            {
                SelectedItemReferenceId = null;
                _referenceDropdownField.value = null;
            }
        }
    }
}