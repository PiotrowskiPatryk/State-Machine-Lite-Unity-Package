using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Views.StateMachineGraphView
{
    /// <summary>
    ///     Draws a stepped orthogonal edge (horizontal→vertical→horizontal) between two VisualElements.
    ///     Mimics Unity Shader Graph edge routing. Hover & click are handled from the panel root.
    /// </summary>
    internal sealed class TransitionEdgeElement : VisualElement
    {
        private const float _clickPixelThreshold = 7f; // screen px

        private readonly VisualElement _from;
        private readonly VisualElement _to;

        private readonly Color _selectedColor = new(0.066f, 0.45f, 0.8313f, 1f);
        private readonly float _selectedWidth = 5.0f;
        private readonly float _baseHitTolerance = 12f;

        private static readonly string HOVER_CLASS_NAME = "hovered";

        public event Action<TransitionEdgeElement> Clicked;
        public event Action<TransitionEdgeElement, bool> HoverChanged;
        public event Action<TransitionEdgeElement, bool, bool> SelectionChanged; // (edge, isSelected, additive)

        private bool _hovered;

        private bool _pressedNearCurve;
        private int _pressedPointerId = -1;
        private Vector2 _pressWorldPos;

        public string SourceStateId { get; }
        public string TargetStateId { get; }
        public object Tag { get; set; }

        public Color LineColor { get; set; } = new(0.45f, 0.70f, 1f, 0.75f);
        public float LineWidth { get; set; } = 2.0f;
        public float ArrowLength { get; set; } = 7.0f;
        public float ArrowAngleDeg { get; set; } = 22.0f;

        /// <summary>
        ///     Minimum horizontal stub length (in local px) before the vertical connector.
        /// </summary>
        public float MinStubLength { get; set; } = 10.0f;

        public bool BringToFrontOnSelect { get; set; } = true;
        public bool IsSelected { get; private set; }

        public TransitionEdgeElement(VisualElement from, VisualElement to, string sourceStateId = null,
            string targetStateId = null)
        {
            pickingMode = PickingMode.Position;

            _from = from;
            _to = to;
            SourceStateId = sourceStateId;
            TargetStateId = targetStateId;

            style.position = Position.Absolute;
            style.left = 0;
            style.top = 0;
            style.right = 0;
            style.bottom = 0;

            generateVisualContent += OnGenerateVisualContent;

            _from?.RegisterCallback<GeometryChangedEvent>(_ => MarkDirtyRepaint());
            _to?.RegisterCallback<GeometryChangedEvent>(_ => MarkDirtyRepaint());
            RegisterCallback<GeometryChangedEvent>(_ => MarkDirtyRepaint());

            // Global (panel-level) listeners so hover/click work even under overlays
            RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }

        public void SetSelected(bool selected)
        {
            if (IsSelected == selected)
            {
                return;
            }

            IsSelected = selected;

            if (IsSelected)
            {
                AddToClassList("selected");
            }
            else
            {
                RemoveFromClassList("selected");
            }

            MarkDirtyRepaint();
        }

        public override bool ContainsPoint(Vector2 localPoint)
        {
            var tol = GetLocalHitTolerance();

            return IsNearLine(localPoint, tol);
        }

        private void OnAttachToPanel(AttachToPanelEvent _)
        {
            var root = panel?.visualTree;

            if (root == null)
            {
                return;
            }

            root.RegisterCallback<PointerMoveEvent>(OnGlobalPointerMove, TrickleDown.TrickleDown);
            root.RegisterCallback<MouseLeaveWindowEvent>(OnWindowMouseLeave);
            root.RegisterCallback<PointerDownEvent>(OnGlobalPointerDown, TrickleDown.TrickleDown);
            root.RegisterCallback<PointerUpEvent>(OnGlobalPointerUp, TrickleDown.TrickleDown);
        }

        private void OnDetachFromPanel(DetachFromPanelEvent e)
        {
            var root = (e?.originPanel ?? panel)?.visualTree;

            if (root == null)
            {
                return;
            }

            root.UnregisterCallback<PointerMoveEvent>(OnGlobalPointerMove, TrickleDown.TrickleDown);
            root.UnregisterCallback<MouseLeaveWindowEvent>(OnWindowMouseLeave);
            root.UnregisterCallback<PointerDownEvent>(OnGlobalPointerDown, TrickleDown.TrickleDown);
            root.UnregisterCallback<PointerUpEvent>(OnGlobalPointerUp, TrickleDown.TrickleDown);
        }

        private void OnGlobalPointerMove(PointerMoveEvent evt)
        {
            var local = this.WorldToLocal(evt.position);
            var isNear = IsNearLine(local, GetLocalHitTolerance());

            if (isNear != _hovered)
            {
                _hovered = isNear;

                if (_hovered)
                {
                    AddToClassList(HOVER_CLASS_NAME);
                }
                else
                {
                    RemoveFromClassList(HOVER_CLASS_NAME);
                }

                HoverChanged?.Invoke(this, _hovered);
                MarkDirtyRepaint();
            }
        }

        private void OnWindowMouseLeave(MouseLeaveWindowEvent _)
        {
            if (!_hovered)
            {
                return;
            }

            _hovered = false;
            RemoveFromClassList(HOVER_CLASS_NAME);
            HoverChanged?.Invoke(this, false);
            MarkDirtyRepaint();
        }

        private void OnGlobalPointerDown(PointerDownEvent evt)
        {
            if (evt.button != 0)
            {
                return;
            }

            var local = this.WorldToLocal(evt.position);

            if (IsNearLine(local, GetLocalHitTolerance()))
            {
                _pressedNearCurve = true;
                _pressedPointerId = evt.pointerId;
                _pressWorldPos = evt.position;
            }
        }

        private void OnGlobalPointerUp(PointerUpEvent evt)
        {
            if (evt.button != 0)
            {
                return;
            }

            var position = new Vector2(evt.localPosition.x, evt.localPosition.y);

            var wasPressed = _pressedNearCurve && (_pressedPointerId == evt.pointerId || _pressedPointerId == -1);
            var movedFar = (position - _pressWorldPos).sqrMagnitude > _clickPixelThreshold * _clickPixelThreshold;

            var local = this.WorldToLocal(evt.position);
            var isNear = IsNearLine(local, GetLocalHitTolerance());

            if (wasPressed && !movedFar && isNear)
            {
                var additive = evt.actionKey || evt.shiftKey; // Ctrl/Cmd or Shift

                if (additive)
                {
                    SetSelected(!IsSelected);
                }
                else
                {
                    SetSelected(true);
                }

                SelectionChanged?.Invoke(this, IsSelected, additive);
                Clicked?.Invoke(this);
            }

            _pressedNearCurve = false;
            _pressedPointerId = -1;
        }

        private float GetLocalHitTolerance()
        {
            var m = worldTransform;
            var sx = new Vector2(m.m00, m.m10).magnitude;
            var sy = new Vector2(m.m01, m.m11).magnitude;
            var avgScale = Mathf.Max(0.0001f, (sx + sy) * 0.5f);

            return _baseHitTolerance / avgScale;
        }

        // ---- Stepped path computation ----

        /// <summary>
        ///     Computes the 4 waypoints of the stepped H-V-H path.
        ///     Returns (start, stubEnd, stubStart, end) where:
        ///     start → stubEnd   = horizontal stub from source
        ///     stubEnd → stubStart = vertical connector
        ///     stubStart → end    = horizontal stub into target
        /// </summary>
        private (Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3) ComputeSteppedPath()
        {
            var start = this.WorldToLocal(_from.worldBound.center);
            var end = this.WorldToLocal(_to.worldBound.center);

            // Midpoint X for the vertical segment — biased by stub length
            var midX = (start.x + end.x) * 0.5f;

            // Ensure minimum horizontal stub from each end
            if (end.x >= start.x)
            {
                midX = Mathf.Max(midX, start.x + MinStubLength);
                midX = Mathf.Min(midX, end.x - MinStubLength);

                // If nodes overlap horizontally, push stubs outward
                if (midX < start.x + MinStubLength)
                {
                    midX = start.x + MinStubLength;
                }
            }
            else
            {
                // Target is to the left of source — route around
                midX = Mathf.Max(start.x + MinStubLength, end.x + MinStubLength);
                midX = Mathf.Max(midX, start.x + MinStubLength);
            }

            var p1 = new Vector2(midX, start.y);
            var p2 = new Vector2(midX, end.y);

            return (start, p1, p2, end);
        }

        // ---- Rendering ----

        private void OnGenerateVisualContent(MeshGenerationContext mgc)
        {
            if (_from == null || _to == null)
            {
                return;
            }

            var (p0, p1, p2, p3) = ComputeSteppedPath();

            var painter = mgc.painter2D;
            var active = IsSelected || _hovered;

            painter.lineWidth = active ? Mathf.Max(_selectedWidth, LineWidth + 1.5f) : LineWidth;
            painter.strokeColor = active ? _selectedColor : LineColor;
            painter.fillColor = active ? _selectedColor : LineColor;
            painter.lineJoin = LineJoin.Round;

            // Draw the 3-segment stepped path
            painter.BeginPath();
            painter.MoveTo(p0);
            painter.LineTo(p1);
            painter.LineTo(p2);
            painter.LineTo(p3);
            painter.Stroke();

            // Arrow direction: always pointing along the last horizontal segment into the target
            var lastDir = (p3 - p2).normalized;

            if (lastDir.sqrMagnitude > 0.0001f)
            {
                DrawArrow(painter, p3, lastDir);
            }
        }

        // ---- Hit testing ----

        private bool IsNearLine(Vector2 localPoint, float maxDistance)
        {
            if (_from == null || _to == null)
            {
                return false;
            }

            var (p0, p1, p2, p3) = ComputeSteppedPath();

            // Check against all 3 segments of the stepped path
            return DistancePointToSegment(localPoint, p0, p1) <= maxDistance
                   || DistancePointToSegment(localPoint, p1, p2) <= maxDistance
                   || DistancePointToSegment(localPoint, p2, p3) <= maxDistance;
        }

        private static float DistancePointToSegment(Vector2 p, Vector2 a, Vector2 b)
        {
            var ab = b - a;
            var t = Vector2.Dot(p - a, ab) / (ab.sqrMagnitude + 1e-6f);
            t = Mathf.Clamp01(t);
            var closest = a + ab * t;

            return Vector2.Distance(p, closest);
        }

        // ---- Arrow (small, modern chevron) ----

        private void DrawArrow(Painter2D painter, Vector2 tip, Vector2 direction)
        {
            var rad = ArrowAngleDeg * Mathf.Deg2Rad;
            var left = tip - Rotate(direction, rad).normalized * ArrowLength;
            var right = tip - Rotate(direction, -rad).normalized * ArrowLength;

            painter.BeginPath();
            painter.MoveTo(left);
            painter.LineTo(tip);
            painter.LineTo(right);
            painter.Stroke();
        }

        private static Vector2 Rotate(Vector2 v, float radians)
        {
            var cos = Mathf.Cos(radians);
            var sin = Mathf.Sin(radians);

            return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
        }
    }
}