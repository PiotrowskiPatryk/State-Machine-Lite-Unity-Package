using System;
using System.Collections.Generic;
using System.Linq;
using Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.ViewModels;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Windows
{
    public class StateMachineConfiguratorWindow : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset _visualTreeAsset;

        [SerializeField]
        private VisualTreeAsset _nodeTreeAsset;

        private StateMachineDefinitionViewModel _stateMachineDefinitionViewModel;
        private Action<SerializedObject> _onSave;

        public static void ShowWindow(StateMachineDefinitionViewModel stateMachineDefinitionViewModel,
            Action<SerializedObject> onSave = null)
        {
            var window = CreateInstance<StateMachineConfiguratorWindow>();
            window._stateMachineDefinitionViewModel = stateMachineDefinitionViewModel;
            window._onSave = onSave;
            window.titleContent = new GUIContent("State Machine Configurator Window");
            window.minSize = new Vector2(1280, 800);
            window.position = EditorWindowUtilities.GetCenteredPosition(new Vector2(1280, 800));
            window.Show();
        }

        private void CreateGUI()
        {
            var root = rootVisualElement;

            _visualTreeAsset.CloneTree(root);

            root.Q<Button>("SaveButton").clicked += OnSave;
            root.Q<Button>("AddNewStateButton").clicked += AddNewState;

            BindData(root);
        }

        private void BindData(VisualElement root)
        {
            root.dataSource = _stateMachineDefinitionViewModel;

            root.Q<DropdownField>("StateMachineTypeDropdown").RegisterValueChangedCallback(newValue =>
                _stateMachineDefinitionViewModel.StateMachineType = newValue.newValue);
            root.Q<DropdownField>("TransitionSolverTypeDropdown").RegisterValueChangedCallback(newValue =>
                _stateMachineDefinitionViewModel.TransitionSolverType = newValue.newValue);
        }

        private void BindItem(VisualElement visualElement, int index)
        {
            // visualElement.Q<Label>().bindingPath = $"{_statesListView.bindingPath}.Array.data[{index}]._name";
            // visualElement.Bind(_serializedObject);
            //
            // var statesContainer = rootVisualElement.Q<VisualElement>("NodesContainer");
            // var node = _nodeTreeAsset.Instantiate();
            // node.Q<Label>().bindingPath = $"{_statesListView.bindingPath}.Array.data[{index}]._name";
            // node.Bind(_serializedObject);
            // statesContainer.Add(node);
            //
            // DragAndDropManipulator manipulator = new(node);
        }

        private void OnSelectionIndicesChanged(IEnumerable<int> selectedIndices)
        {
            // var selectedIndex = selectedIndices.FirstOrDefault();
            // var statePropertyField = rootVisualElement.Q<PropertyField>("StatePropertyField");
            // statePropertyField.bindingPath = $"{_statesListView.bindingPath}.Array.data[{selectedIndex}]";
            // statePropertyField.Bind(_serializedObject);
        }

        private void OnItemsRemoved(IEnumerable<int> removedIndexes)
        {
            var statesContainer = rootVisualElement.Q<VisualElement>("NodesContainer");

            foreach (var indexes in removedIndexes.OrderByDescending(x => x))
            {
                statesContainer.Remove(statesContainer.ElementAt(indexes));
            }
        }

        private void AddNewState()
        {
            _stateMachineDefinitionViewModel.CreateNewState();
        }

        private void OnSave()
        {
            var displayDialog = EditorUtility.DisplayDialog("Save", "Do you want to save the changes?", "Yes", "No");

            if (!displayDialog)
            {
                return;
            }

            // _onSave?.Invoke(_serializedObject);

            EditorUtility.DisplayDialog("Save", "Changes saved", "OK");
        }
    }
}