using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Core;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Data.Configuration;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.PropertyDrawer
{
    [CustomEditor(typeof(StateMachineContainer))]
    public class StateMachineContainerPropertyDrawer : UnityEditor.Editor
    {
        [SerializeField]
        private VisualTreeAsset _visualTreeAsset;

        private Button _openEditorButton;

        public override VisualElement CreateInspectorGUI()
        {
            var visualElement = _visualTreeAsset.Instantiate();

            _openEditorButton = visualElement.Q<Button>("OpenEditorButton");
            _openEditorButton.clicked += OnOpenEditorButtonPressed;

            return visualElement;
        }

        private void OnDestroy()
        {
            _openEditorButton.clicked -= OnOpenEditorButtonPressed;
        }

        private void OnOpenEditorButtonPressed()
        {
            StateMachineContainerEditor.Show(serializedObject);
        }
    }
}