using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Data;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Data.Definition;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.UIToolkitUtilities;
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

        private SerializedObject _serializedObject;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = _visualTreeAsset.CloneTree();
            var listView = root.Q<MultiColumnListView>("ListView");
            var newStateMachineButton = root.Q<Button>("NewStateMachineButton");
            var stateMachinesProperty = property.FindPropertyRelative("_stateMachines");

            _serializedObject = property.serializedObject;

            listView.BindColumnWithProperty(StateMachineDefinition.ID_PROPERTY_NAME, stateMachinesProperty,
                StateMachineDefinition.ID_PROPERTY_NAME);
            listView.BindColumnWithProperty(StateMachineDefinition.NAME_PROPERTY_NAME, stateMachinesProperty,
                StateMachineDefinition.NAME_PROPERTY_NAME);
            listView.BindColumnWithProperty(StateMachineDefinition.DESCRIPTION_PROPERTY_NAME, stateMachinesProperty,
                StateMachineDefinition.DESCRIPTION_PROPERTY_NAME);
            listView.BindColumnWithProperty(StateMachineDefinition.STATE_MACHINE_TYPE_PROPERTY_NAME,
                stateMachinesProperty, StateMachineDefinition.STATE_MACHINE_TYPE_PROPERTY_NAME, true);
            listView.BindColumnWithProperty(StateMachineDefinition.TRANSITION_SOLVER_TYPE_PROPERTY_NAME,
                stateMachinesProperty, StateMachineDefinition.TRANSITION_SOLVER_TYPE_PROPERTY_NAME, true);

            void Refresh()
            {
                listView.ResetAndRefreshFromArray(stateMachinesProperty);
            }

            RowActionButtonsUtility.WireLastColumnEditDelete(listView, stateMachinesProperty,
                index => { Debug.Log($"Pressed button to edit state machine with index {index}"); },
                index => { Debug.Log($"Pressed button to remove state machine with index {index}"); });

            newStateMachineButton.clicked += () =>
            {
                var so = stateMachinesProperty.serializedObject;
                so.Update();

                var index = stateMachinesProperty.arraySize;
                stateMachinesProperty.InsertArrayElementAtIndex(index);
                so.ApplyModifiedProperties();

                Refresh();

                Debug.Log("Pressed button to create new state machine");
            };

            listView.onItemsChosen += _ =>
            {
                var stateMachineDefinition = _serializedObject;

                var index = listView.selectedIndex;
                // StateMachineMenuWindow.Show(_serializedObject.targetObject);
            };

            return root;
        }
    }
}