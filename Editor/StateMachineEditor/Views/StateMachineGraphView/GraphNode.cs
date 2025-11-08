using UnityEngine;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Views.StateMachineGraphView
{
    [UxmlElement]
    public partial class GraphNode : BindableElement, INotifyValueChanged<Vector2Int>
    {
        private Vector2Int _value;

        [UxmlAttribute("pixels-per-unit")]
        public float PixelsPerUnit { get; set; } = 1f;

        [UxmlAttribute("snap")]
        public int Snap { get; set; } = 1;

        [UxmlAttribute("clamp-to-parent")]
        public bool ClampToParent { get; set; } = false;

        // ---- Bind target for runtime TwoWay binding ----
        public Vector2Int value
        {
            get => _value;
            set
            {
                if (_value == value)
                {
                    return;
                }

                var old = _value;
                SetValueWithoutNotify(value);

                using (var evt = ChangeEvent<Vector2Int>.GetPooled(old, _value))
                {
                    evt.target = this;
                    SendEvent(evt); // runtime binder listens to this (via INotifyValueChanged)
                }
            }
        }

        public GraphNode()
        {
            style.position = Position.Absolute; // parent must be Relative
            this.AddManipulator(new DragManipulator(this));
        }

        public void SetValueWithoutNotify(Vector2Int v)
        {
            _value = v;
            style.left = _value.x * PixelsPerUnit;
            style.top = _value.y * PixelsPerUnit;
        }

        // ---------- Dragging ----------
        private sealed class DragManipulator : PointerManipulator
        {
            private readonly GraphNode _owner;
            private bool _active;
            private Vector2 _startLocal;
            private Vector2Int _startValue;

            public DragManipulator(GraphNode owner)
            {
                _owner = owner;
            }

            protected override void RegisterCallbacksOnTarget()
            {
                target.RegisterCallback<PointerDownEvent>(OnDown);
                target.RegisterCallback<PointerMoveEvent>(OnMove);
                target.RegisterCallback<PointerUpEvent>(OnUp);
                target.RegisterCallback<PointerCaptureOutEvent>(_ => _active = false);
            }

            protected override void UnregisterCallbacksFromTarget()
            {
                target.UnregisterCallback<PointerDownEvent>(OnDown);
                target.UnregisterCallback<PointerMoveEvent>(OnMove);
                target.UnregisterCallback<PointerUpEvent>(OnUp);
                target.UnregisterCallback<PointerCaptureOutEvent>(_ => _active = false);
            }

            private void OnDown(PointerDownEvent evt)
            {
                if (evt.button != 0)
                {
                    return;
                }

                // Do not start node drag when interacting with port buttons (Output/Input)
                if (evt.target is VisualElement ve)
                {
                    // Check the target and its ancestors up to this GraphNode
                    var cur = ve;

                    while (cur != null && cur != _owner)
                    {
                        if (cur.name is "OutputNode" or "InputNode")
                        {
                            return; // let specialized handlers (e.g., edge create) process this
                        }

                        cur = cur.parent;
                    }
                }

                _active = true;
                _startValue = _owner.value;

                var parent = _owner.parent ?? _owner;
                _startLocal = parent.WorldToLocal(evt.position);

                target.CapturePointer(evt.pointerId);
                evt.StopImmediatePropagation();
            }

            private void OnMove(PointerMoveEvent evt)
            {
                if (!_active || !target.HasPointerCapture(evt.pointerId))
                {
                    return;
                }

                var parent = _owner.parent ?? _owner;
                var currentLocal = parent.WorldToLocal(evt.position);
                var delta = currentLocal - _startLocal;

                var ppu = Mathf.Max(0.0001f, _owner.PixelsPerUnit);
                var nx = _startValue.x + Mathf.RoundToInt(delta.x / ppu);
                var ny = _startValue.y + Mathf.RoundToInt(delta.y / ppu);

                if (_owner.Snap > 1)
                {
                    nx = Mathf.RoundToInt((float)nx / _owner.Snap) * _owner.Snap;
                    ny = Mathf.RoundToInt((float)ny / _owner.Snap) * _owner.Snap;
                }

                if (_owner.ClampToParent)
                {
                    var parentRect = parent.contentRect;
                    var w = _owner.layout.width > 0 ? _owner.layout.width : _owner.resolvedStyle.width;
                    var h = _owner.layout.height > 0 ? _owner.layout.height : _owner.resolvedStyle.height;

                    var minX = (int)Mathf.Floor(parentRect.xMin / ppu);
                    var maxX = (int)Mathf.Floor((parentRect.xMax - w) / ppu);
                    var minY = (int)Mathf.Floor(parentRect.yMin / ppu);
                    var maxY = (int)Mathf.Floor((parentRect.yMax - h) / ppu);

                    nx = Mathf.Clamp(nx, minX, maxX);
                    ny = Mathf.Clamp(ny, minY, maxY);
                }

                _owner.value = new Vector2Int(nx, ny); // fires ChangeEvent<Vector2Int>
                evt.StopPropagation();
            }

            private void OnUp(PointerUpEvent evt)
            {
                if (!_active || evt.button != 0)
                {
                    return;
                }

                _active = false;
                target.ReleasePointer(evt.pointerId);
                evt.StopPropagation();
            }
        }
    }
}