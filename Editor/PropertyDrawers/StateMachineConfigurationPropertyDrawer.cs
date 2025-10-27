using Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.Data;
using Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.Data.Definition;
using Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.ViewModels;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.UIToolkitUtilities;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Windows;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(StateMachineConfiguration))]
    public class StateMachineConfigurationPropertyDrawer : PropertyDrawer
    {
        [SerializeField]
        private VisualTreeAsset _visualTreeAsset;

        private StateMachineConfigurationViewModel _stateMachineConfigurationViewModel;
        private SerializedObject _serializedObject;
        private MultiColumnListView _listView;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = _visualTreeAsset.CloneTree();
            _listView = root.Q<MultiColumnListView>("ListView");
            var newStateMachineButton = root.Q<Button>("NewStateMachineButton");
            var stateMachinesProperty = property.FindPropertyRelative("_stateMachines");

            _stateMachineConfigurationViewModel = new StateMachineConfigurationViewModel(property);
            // _stateMachineConfigurationViewModel.OnStateMachineAdded += OnStateMachineAdded;
            _serializedObject = property.serializedObject;

            _listView.BindColumnWithProperty(StateMachineDefinition.ID_PROPERTY_NAME, stateMachinesProperty,
                StateMachineDefinition.ID_PROPERTY_NAME);
            _listView.BindColumnWithProperty(StateMachineDefinition.NAME_PROPERTY_NAME, stateMachinesProperty,
                StateMachineDefinition.NAME_PROPERTY_NAME);
            _listView.BindColumnWithProperty(StateMachineDefinition.DESCRIPTION_PROPERTY_NAME, stateMachinesProperty,
                StateMachineDefinition.DESCRIPTION_PROPERTY_NAME);
            _listView.BindColumnWithProperty(StateMachineDefinition.STATE_MACHINE_TYPE_PROPERTY_NAME,
                stateMachinesProperty, StateMachineDefinition.STATE_MACHINE_TYPE_PROPERTY_NAME, true);
            _listView.BindColumnWithProperty(StateMachineDefinition.TRANSITION_SOLVER_TYPE_PROPERTY_NAME,
                stateMachinesProperty, StateMachineDefinition.TRANSITION_SOLVER_TYPE_PROPERTY_NAME, true);

            RowActionButtonsUtility.WireLastColumnEditDelete(_listView, stateMachinesProperty,
                index => { Debug.Log($"Pressed button to edit state machine with index {index}"); },
                index => { Debug.Log($"Pressed button to remove state machine with index {index}"); });

            // newStateMachineButton.clicked += () => _stateMachineConfigurationViewModel.AddStateMachine();

            _listView.selectedIndicesChanged += _ =>
            {
                var index = _listView.selectedIndex;

                if (index < 0 || index >= stateMachinesProperty.arraySize)
                {
                    return;
                }

                var stateMachineTarget = stateMachinesProperty.GetArrayElementAtIndex(index);

                var (wrapper, serializedObject) =
                    FormBindingUtility.CreateWrapper<StateMachineDefinitionWrapper>(stateMachineTarget);

                var stateMachineDefinitionViewModel = _stateMachineConfigurationViewModel.GetStateMachineByIndex(index);

                StateMachineConfiguratorWindow.ShowWindow(stateMachineDefinitionViewModel, savedData =>
                {
                    // savedSo is the wrapper SO bound to the window; its Data holds edited values
                    savedData.Update();
                    var src = savedData.FindProperty("Data");

                    var listSo = stateMachinesProperty.serializedObject;
                    listSo.Update();

                    // Re-resolve the destination index safely (selected index can change)
                    var dstIndex = index; // or cache to itemIndex earlier

                    if (dstIndex < 0 || dstIndex >= stateMachinesProperty.arraySize)
                    {
                        return;
                    }

                    var dst = stateMachinesProperty.GetArrayElementAtIndex(dstIndex);

                    // Assign using the proper API for the type
                    if (dst.propertyType == SerializedPropertyType.ManagedReference &&
                        src.propertyType == SerializedPropertyType.ManagedReference)
                    {
                        dst.managedReferenceValue = src.managedReferenceValue;
                    }
                    else if (dst.propertyType == SerializedPropertyType.ObjectReference &&
                             src.propertyType == SerializedPropertyType.ObjectReference)
                    {
                        dst.objectReferenceValue = src.objectReferenceValue;
                    }
                    else
                    {
                        // Works for most built-in serializable types/structs
                        dst.boxedValue = src.boxedValue;
                    }

                    listSo.ApplyModifiedProperties();

                    // If you have a local helper to refresh the ListView
                    Refresh();
                });
            };

            return root;
        }

        private void Refresh()
        {
            _listView.RefreshItems();
        }

        private void OnStateMachineAdded(StateMachineDefinitionViewModel stateMachineDefinitionViewModel)
        {
            Refresh();
        }
    }
}