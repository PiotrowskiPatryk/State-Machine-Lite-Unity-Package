using System;
using System.Collections.Generic;
using System.Linq;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Data.Repository;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.ViewModels;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Views.StateMachineGraphView.VisualElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Views.StateMachineGraphView
{
    public class StateMachineGraphView : VisualElement
    {
        private readonly VisualElement _statesContainer;
        private readonly Dictionary<string, GraphNode> _nodeById = new();
        private readonly VisualElement _transitionsContainer;
        private readonly VisualTreeAsset _stateNodeTemplate;
        private readonly List<TransitionEdgeElement> _edges = new();
        private readonly Color _nodeBorderDefault = new(48 / 255f, 73 / 255f, 98 / 255f, 1f);
        private readonly Color _nodeBorderSelected = new(0.066f, 0.45f, 0.8313f, 1f);

        private readonly GraphNodeBackgroundVisualElement _background;

        // Events
        public event Action<string> NodeClicked; // stateId
        public event Action<string, string> EdgeClicked; // sourceStateId, transitionPropertyPath
        public event Action<string, string> CreateTransitionRequested; // sourceStateId, targetStateId
        private GraphNode _selectedNode;
        private TransitionEdgeElement _selectedEdge;

        private StateMachineDefinitionViewModel _stateMachineDefinitionViewModel;

        // ---- Create edge drag ----
        private VisualElement _cursorAnchor;
        private TransitionEdgeElement _previewEdge;
        private string _previewSourceId;
        private int _dragPointerId = -1;

        public StateMachineGraphView()
        {
            var visualTreeAsset =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    StateMachineEditorViewRepository.STATE_MACHINE_GRAPH_VIEW_PATH);
            visualTreeAsset.CloneTree(this);

            style.flexGrow = 1;
            style.flexShrink = 0;
            style.flexBasis = 0;

            _background = this.Q<GraphNodeBackgroundVisualElement>();
            _statesContainer = this.Q<VisualElement>("StatesContainer");
            _transitionsContainer = this.Q<VisualElement>("TransitionsContainer");

            // Ensure transitions container overlays the content and can receive pointer events for edges
            if (_transitionsContainer != null)
            {
                _transitionsContainer.style.position = Position.Absolute;
                _transitionsContainer.style.left = 0;
                _transitionsContainer.style.top = 0;
                _transitionsContainer.style.right = 0;
                _transitionsContainer.style.bottom = 0;
                // Let children (edges) be pickable but not the container itself to avoid blocking node clicks
                _transitionsContainer.pickingMode = PickingMode.Ignore;
            }

            _stateNodeTemplate =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    StateMachineEditorViewRepository.STATE_NODE_TEMPLATE_VIEW_PATH);

            Debug.Assert(_statesContainer != null, "_statesContainer should not be null");
            Debug.Assert(_transitionsContainer != null, "_transitionsContainer should not be null");
            Debug.Assert(_stateNodeTemplate != null, "_stateNodeTemplate should not be null");
            Debug.Assert(_background != null, "_background should not be null");
        }

        public void Bind(StateMachineDefinitionViewModel stateMachineDefinitionViewModel)
        {
            _stateMachineDefinitionViewModel = stateMachineDefinitionViewModel;
            dataSource = stateMachineDefinitionViewModel;

            SyncNodes();
            SyncTransitions();

            this.TrackPropertyValue(stateMachineDefinitionViewModel.StatesSerializedProperty, _ =>
            {
                SyncNodes();
                SyncTransitions();
            });
        }

        // ---- Public API ----
        public void HighlightNodeById(string stateId, bool center = true)
        {
            if (string.IsNullOrEmpty(stateId))
            {
                return;
            }

            if (_nodeById.TryGetValue(stateId, out var node))
            {
                SelectNode(node, center);
            }
        }

        public void HighlightTransitionByPath(string propertyPath)
        {
            if (string.IsNullOrEmpty(propertyPath))
            {
                return;
            }

            var edge = _edges.FirstOrDefault(e => Equals(e.Tag, propertyPath));

            if (edge != null)
            {
                SelectEdge(edge);
            }
        }

        // ---- Internal sync ----
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

        private void SyncTransitions()
        {
            _transitionsContainer.Clear();
            _edges.Clear();

            if (_stateMachineDefinitionViewModel == null)
            {
                return;
            }

            var states = _stateMachineDefinitionViewModel.States;

            foreach (var source in states)
            {
                CreateTransitionEdges(source);
            }
        }

        private void CreateTransitionEdges(StateDefinitionViewModel source)
        {
            if (!_nodeById.TryGetValue(source.Id, out var sourceNode))
            {
                return;
            }

            var fromAnchor = sourceNode.Q<VisualElement>("OutputNode");

            if (fromAnchor == null)
            {
                return;
            }

            foreach (var tr in source.Transitions)
            {
                var targetId = tr.TargetState?.Id;

                if (string.IsNullOrEmpty(targetId))
                {
                    continue;
                }

                if (!_nodeById.TryGetValue(targetId, out var targetNode))
                {
                    continue;
                }

                var toAnchor = targetNode.Q<VisualElement>("InputNode");

                if (toAnchor == null)
                {
                    continue;
                }

                var edge = new TransitionEdgeElement(fromAnchor, toAnchor, source.Id, targetId)
                {
                    Tag = tr.SerializedProperty.propertyPath
                };

                edge.Clicked += OnEdgeClickedInternal;
                _edges.Add(edge);
                _transitionsContainer.Add(edge);
            }
        }

        private GraphNode AddNode(StateDefinitionViewModel state)
        {
            var instance = _stateNodeTemplate.Instantiate();
            var node = instance.Q<GraphNode>();
            node.dataSource = state;
            instance.dataSource = state;

            node.SetValueWithoutNotify(state.NodePosition);

            node.RegisterValueChangedCallback(e =>
            {
                state.NodePosition = e.newValue;

                // Invalidate all edges so they redraw while dragging
                foreach (var edge in _edges)
                {
                    edge?.MarkDirtyRepaint();
                }
            });

            // Node selection on click: listen on GraphNode at TrickleDown so we still get the event
            // even if the drag manipulator stops propagation at target phase.
            node.RegisterCallback<PointerUpEvent>(evt =>
            {
                if (evt.button != 0)
                {
                    return;
                }

                // From GraphView clicks we should NOT center; centering is driven by inspector interactions
                SelectNode(node, false);
                NodeClicked?.Invoke(state.Id);
                // Do not StopPropagation here to not interfere with other systems; selection is idempotent
            }, TrickleDown.TrickleDown);

            // Drag-create transition from Output to another Input
            var output = instance.Q<Button>("OutputNode");

            if (output != null)
            {
                // Listen on the node in capture (TrickleDown) phase to avoid Button's internal handlers swallowing the event
                node.RegisterCallback<PointerDownEvent>(evt => HandleOutputPortClick(state, evt, node, output),
                    TrickleDown.TrickleDown);
            }

            _statesContainer.Add(instance);
            _nodeById[state.Id] = node;

            return node;
        }

        private void HandleOutputPortClick(StateDefinitionViewModel state, PointerDownEvent evt, GraphNode node,
            Button output)
        {
            if (evt.button != 0)
            {
                return;
            }

            // Start only if the press originated on the Output port (or its children)
            if (evt.target is not VisualElement ve)
            {
                return;
            }

            var cur = ve;

            while (cur != null && cur != node)
            {
                if (cur == output)
                {
                    StartCreateEdgeDrag(evt, output, state.Id);
                    // Prevent the GraphNode drag manipulator and others from reacting
                    evt.StopImmediatePropagation();

                    break;
                }

                cur = cur.parent;
            }
        }

        private void RemoveNode(string id)
        {
            if (_nodeById.TryGetValue(id, out var ve))
            {
                ve.RemoveFromHierarchy();
                _nodeById.Remove(id);
            }
        }

        // ---- Selection helpers ----
        private void SelectNode(GraphNode node, bool center)
        {
            // Deselect any selected edge to keep selection exclusive
            if (_selectedEdge != null)
            {
                _selectedEdge.SetSelected(false);
                _selectedEdge = null;
            }

            if (_selectedNode != null)
            {
                var prev = _selectedNode.Q<VisualElement>("NodeElement");

                if (prev != null)
                {
                    prev.style.borderLeftColor = _nodeBorderDefault;
                    prev.style.borderRightColor = _nodeBorderDefault;
                    prev.style.borderTopColor = _nodeBorderDefault;
                    prev.style.borderBottomColor = _nodeBorderDefault;
                }
            }

            _selectedNode = node;
            var cur = _selectedNode?.Q<VisualElement>("NodeElement");

            if (cur != null)
            {
                cur.style.borderLeftColor = _nodeBorderSelected;
                cur.style.borderRightColor = _nodeBorderSelected;
                cur.style.borderTopColor = _nodeBorderSelected;
                cur.style.borderBottomColor = _nodeBorderSelected;
            }

            if (center && _selectedNode != null)
            {
                var contentPos = _background.Content.WorldToLocal(_selectedNode.worldBound.center);
                _background.CenterOn(contentPos);
            }
        }

        private void SelectEdge(TransitionEdgeElement edge)
        {
            // Deselect any selected node to keep selection exclusive
            if (_selectedNode != null)
            {
                var prev = _selectedNode.Q<VisualElement>("NodeElement");

                if (prev != null)
                {
                    prev.style.borderLeftColor = _nodeBorderDefault;
                    prev.style.borderRightColor = _nodeBorderDefault;
                    prev.style.borderTopColor = _nodeBorderDefault;
                    prev.style.borderBottomColor = _nodeBorderDefault;
                }

                _selectedNode = null;
            }

            if (_selectedEdge != null)
            {
                _selectedEdge.SetSelected(false);
            }

            _selectedEdge = edge;
            _selectedEdge?.SetSelected(true);

            // bring to front so highlight is visible
            _selectedEdge?.BringToFront();
        }

        private void OnEdgeClickedInternal(TransitionEdgeElement edge)
        {
            SelectEdge(edge);
            EdgeClicked?.Invoke(edge.SourceStateId, edge.Tag as string);
        }

        private void StartCreateEdgeDrag(PointerDownEvent evt, VisualElement fromAnchor, string sourceStateId)
        {
            if (evt.button != 0)
            {
                return;
            }

            // Prevent re-entrancy if a drag is already active
            if (_previewEdge != null)
            {
                return;
            }

            _previewSourceId = sourceStateId;

            // Create a cursor anchor under transitions container
            _cursorAnchor = new VisualElement
            {
                name = "CursorAnchor",
                pickingMode = PickingMode.Ignore
            };
            _cursorAnchor.style.position = Position.Absolute;
            _cursorAnchor.style.width = 1;
            _cursorAnchor.style.height = 1;
            _cursorAnchor.style.left = 0;
            _cursorAnchor.style.top = 0;

            _transitionsContainer.Add(_cursorAnchor);

            _previewEdge = new TransitionEdgeElement(fromAnchor, _cursorAnchor, sourceStateId);
            _transitionsContainer.Add(_previewEdge);

            // Initial position
            UpdateCursorAnchorPosition(evt.position);

            // Capture pointer so we reliably receive move/up even when leaving our bounds
            _dragPointerId = evt.pointerId;
            this.CapturePointer(_dragPointerId);

            // Listen in trickle-down to avoid collisions and ensure early handling
            RegisterCallback<PointerMoveEvent>(OnCreateEdgePointerMove, TrickleDown.TrickleDown);
            RegisterCallback<PointerUpEvent>(OnCreateEdgePointerUp, TrickleDown.TrickleDown);
            RegisterCallback<PointerCaptureOutEvent>(OnCreateEdgePointerCaptureOut);

            evt.StopImmediatePropagation();
        }

        private void OnCreateEdgePointerMove(PointerMoveEvent evt)
        {
            UpdateCursorAnchorPosition(evt.position);
            _previewEdge?.MarkDirtyRepaint();
            evt.StopPropagation();
        }

        private void OnCreateEdgePointerUp(PointerUpEvent evt)
        {
            string targetId = null;

            // Prefer precise picking using panel tree at pointer position
            if (panel != null)
            {
                // Pick top-most element and walk up to find an InputNode
                var picked = panel.Pick(evt.position);
                var cur = picked;

                while (cur != null)
                {
                    if (cur.name == "InputNode")
                    {
                        var node = cur.GetFirstOfType<GraphNode>();

                        if (node != null)
                        {
                            targetId = _nodeById.FirstOrDefault(p => ReferenceEquals(p.Value, node)).Key;
                        }

                        break;
                    }

                    if (ReferenceEquals(cur, this))
                    {
                        break;
                    }

                    cur = cur.parent;
                }
            }

            // Fallback: bounding check on all nodes with a bit of tolerance around input port
            if (string.IsNullOrEmpty(targetId))
            {
                const float pad = 8f;

                foreach (var kv in _nodeById)
                {
                    var input = kv.Value.Q<VisualElement>("InputNode");

                    if (input == null)
                    {
                        continue;
                    }

                    var r = input.worldBound;
                    r.xMin -= pad;
                    r.xMax += pad;
                    r.yMin -= pad;
                    r.yMax += pad;

                    if (!r.Contains(evt.position))
                    {
                        continue;
                    }

                    targetId = kv.Key;

                    break;
                }
            }

            CleanupPreview();

            if (!string.IsNullOrEmpty(targetId) && !string.Equals(targetId, _previewSourceId))
            {
                CreateTransitionRequested?.Invoke(_previewSourceId, targetId);
            }

            evt.StopPropagation();
        }

        private void UpdateCursorAnchorPosition(Vector2 worldPos)
        {
            if (_cursorAnchor == null)
            {
                return;
            }

            var local = _transitionsContainer.WorldToLocal(worldPos);
            _cursorAnchor.style.left = local.x;
            _cursorAnchor.style.top = local.y;
        }

        private void CleanupPreview()
        {
            if (_previewEdge != null)
            {
                _previewEdge.RemoveFromHierarchy();
                _previewEdge = null;
            }

            if (_cursorAnchor != null)
            {
                _cursorAnchor.RemoveFromHierarchy();
                _cursorAnchor = null;
            }

            // Release pointer capture if any
            if (_dragPointerId != -1 && this.HasPointerCapture(_dragPointerId))
            {
                this.ReleasePointer(_dragPointerId);
            }

            _dragPointerId = -1;

            UnregisterCallback<PointerMoveEvent>(OnCreateEdgePointerMove);
            UnregisterCallback<PointerUpEvent>(OnCreateEdgePointerUp);
            UnregisterCallback<PointerCaptureOutEvent>(OnCreateEdgePointerCaptureOut);
        }

        private void OnCreateEdgePointerCaptureOut(PointerCaptureOutEvent evt)
        {
            // Cancel current preview drag if pointer is lost
            CleanupPreview();
        }
    }
}