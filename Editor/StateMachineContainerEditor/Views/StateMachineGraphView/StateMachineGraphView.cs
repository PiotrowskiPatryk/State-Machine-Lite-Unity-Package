using System.Collections.Generic;
using System.Linq;
using Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.ViewModels;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.Views.StateMachineGraphView
{
    public class StateMachineGraphView : VisualElement
    {
        private const string UxmlPath =
            "Packages/dev.cortez.state-machines/Editor/StateMachineContainerEditor/Views/StateMachineGraphView/StateMachineGraphView.uxml";

        private const string StateNodeTemplatePath =
            "Packages/dev.cortez.state-machines/Editor/StateMachineContainerEditor/Views/StateMachineGraphView/Templates/StateNodeTemplate.uxml";

        private readonly VisualElement _statesContainer;
        private readonly Dictionary<string, GraphNode> _nodeById = new();
        private readonly VisualElement _transitionsContainer;
        private readonly VisualTreeAsset _stateNodeTemplate;

        private StateMachineDefinitionViewModel _stateMachineDefinitionViewModel;

        public StateMachineGraphView()
        {
            var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UxmlPath);
            visualTreeAsset.CloneTree(this);

            style.flexGrow = 1;
            style.flexShrink = 0;
            style.flexBasis = 0;

            _statesContainer = this.Q<VisualElement>("StatesContainer");
            _transitionsContainer = this.Q<VisualElement>("TransitionsContainer");

            _stateNodeTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(StateNodeTemplatePath);

            Assert.NotNull(_statesContainer);
            Assert.NotNull(_transitionsContainer);
            Assert.NotNull(_stateNodeTemplate);
        }

        public void Bind(StateMachineDefinitionViewModel stateMachineDefinitionViewModel)
        {
            _stateMachineDefinitionViewModel = stateMachineDefinitionViewModel;
            dataSource = stateMachineDefinitionViewModel;

            SyncNodes();

            this.TrackPropertyValue(stateMachineDefinitionViewModel.StatesSerializedProperty, _ => SyncNodes());
        }

        private void SyncNodes()
        {
            var states = _stateMachineDefinitionViewModel.States;
            var wantedIds = new HashSet<string>(states.Select(state => state.Id));

            // 1) Remove nodes that no longer exist
            foreach (var deadId in _nodeById.Keys.Where(id => !wantedIds.Contains(id)).ToList())
            {
                RemoveNode(deadId);
            }

            // 2) Add or update nodes
            foreach (var state in states)
            {
                if (!_nodeById.TryGetValue(state.Id, out var anchor))
                {
                    anchor = AddNode(state);
                }
            }
        }

        private GraphNode AddNode(StateDefinitionViewModel state)
        {
            var instance = _stateNodeTemplate.Instantiate();
            var node = instance.Q<GraphNode>();
            node.dataSource = state;
            instance.dataSource = state;

            node.SetValueWithoutNotify(state.NodePosition);

            node.RegisterValueChangedCallback(e => { state.NodePosition = e.newValue; });

            _statesContainer.Add(instance);
            _nodeById[state.Id] = node;

            return node;
        }

        private void RemoveNode(string id)
        {
            if (_nodeById.TryGetValue(id, out var ve))
            {
                ve.RemoveFromHierarchy();
                _nodeById.Remove(id);
            }
        }
    }
}