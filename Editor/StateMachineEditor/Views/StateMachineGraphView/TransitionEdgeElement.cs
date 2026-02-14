#region

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

#endregion

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Views.StateMachineGraphView
{
    /// <summary>
    ///     Draws an orthogonal edge between two port VisualElements, routing around source/target nodes.
    ///     Supports rounded corners and automatic wrap-around when nodes overlap horizontally.
    ///     Hover & click are handled from the panel root.
    /// </summary>
    internal sealed class TransitionEdgeElement : VisualElement
    {
        private const float _clickPixelThreshold = 7f; // screen px

        private readonly VisualElement _from;
        private readonly VisualElement _to;
        private readonly VisualElement _sourceNode;
        private readonly VisualElement _targetNode;

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

        // Reusable list to avoid GC pressure during path recomputation
        private readonly List<Vector2> _pathPoints = new(8);

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
        public float MinStubLength { get; set; } = 40.0f;

        /// <summary>
        ///     Padding around node bounds when routing the vertical segment.
        /// </summary>
        public float NodePadding { get; set; } = 40.0f;

        /// <summary>
        ///     Radius for rounded corners at 90-degree bends.
        /// </summary>
        public float CornerRadius { get; set; } = 15.0f;

        /// <summary>
        ///     Lane offset index for this edge within its corridor group.
        ///     0 = centered, negative = left, positive = right.
        ///     Assigned by the graph view to separate overlapping edges.
        /// </summary>
        public float LaneOffset { get; set; } = 0f;

        /// <summary>
        ///     Spacing in pixels between adjacent lanes.
        /// </summary>
        public float LaneSpacing { get; set; } = 25f;

        public bool BringToFrontOnSelect { get; set; } = true;
        public bool IsSelected { get; private set; }

        public TransitionEdgeElement(VisualElement from, VisualElement to,
            VisualElement sourceNode, VisualElement targetNode,
            string sourceStateId = null, string targetStateId = null)
        {
            pickingMode = PickingMode.Position;

            _from = from;
            _to = to;
            _sourceNode = sourceNode;
            _targetNode = targetNode;
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
            _sourceNode?.RegisterCallback<GeometryChangedEvent>(_ => MarkDirtyRepaint());
            _targetNode?.RegisterCallback<GeometryChangedEvent>(_ => MarkDirtyRepaint());
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
        ///     Computes an orthogonal path from the right edge of the source port to the left edge of
        ///     the target port, routing around both source and target node bounds with padding.
        ///     Returns a variable-length list of waypoints (always orthogonal segments).
        /// </summary>
        private List<Vector2> ComputePath()
        {
            _pathPoints.Clear();

            // Anchor at port edges: right edge of output, left edge of input
            var fromBound = _from.worldBound;
            var toBound = _to.worldBound;

            var startWorld = new Vector2(fromBound.xMax, fromBound.center.y);
            var endWorld = new Vector2(toBound.xMin, toBound.center.y);

            var start = this.WorldToLocal(startWorld);
            var end = this.WorldToLocal(endWorld);

            // Get node bounds in local space for avoidance
            var srcNodeWorld = _sourceNode != null ? _sourceNode.worldBound : fromBound;
            var tgtNodeWorld = _targetNode != null ? _targetNode.worldBound : toBound;

            var srcRight = this.WorldToLocal(new Vector2(srcNodeWorld.xMax, 0)).x;
            var tgtLeft = this.WorldToLocal(new Vector2(tgtNodeWorld.xMin, 0)).x;
            var srcLeft = this.WorldToLocal(new Vector2(srcNodeWorld.xMin, 0)).x;
            var tgtRight = this.WorldToLocal(new Vector2(tgtNodeWorld.xMax, 0)).x;
            var srcTop = this.WorldToLocal(new Vector2(0, srcNodeWorld.yMin)).y;
            var srcBottom = this.WorldToLocal(new Vector2(0, srcNodeWorld.yMax)).y;
            var tgtTop = this.WorldToLocal(new Vector2(0, tgtNodeWorld.yMin)).y;
            var tgtBottom = this.WorldToLocal(new Vector2(0, tgtNodeWorld.yMax)).y;

            _pathPoints.Add(start);

            var laneShift = LaneOffset * LaneSpacing;
            var gap = tgtLeft - srcRight;

            if (gap >= MinStubLength * 2)
            {
                // Normal case: enough horizontal space between nodes
                // Simple 3-segment H-V-H path through the gap, shifted by lane offset
                var midX = srcRight + gap * 0.5f + laneShift;

                _pathPoints.Add(new Vector2(midX, start.y));
                _pathPoints.Add(new Vector2(midX, end.y));
            }
            else
            {
                // Nodes overlap or are too close horizontally — route around
                // Use a 5-segment path: H → V → H → V → H
                var stubX = Mathf.Max(srcRight, tgtRight) + NodePadding + laneShift;

                // Choose vertical route: go above or below, whichever is shorter
                var aboveY = Mathf.Min(srcTop, tgtTop) - NodePadding;
                var belowY = Mathf.Max(srcBottom, tgtBottom) + NodePadding;

                var midY = Mathf.Abs(start.y - aboveY) < Mathf.Abs(start.y - belowY)
                    ? aboveY
                    : belowY;

                // Right stub from source
                _pathPoints.Add(new Vector2(stubX, start.y));
                // Vertical to the routing lane
                _pathPoints.Add(new Vector2(stubX, midY));

                // Left of target
                var entryX = Mathf.Min(srcLeft, tgtLeft) - NodePadding + laneShift;
                // Horizontal across
                _pathPoints.Add(new Vector2(entryX, midY));
                // Vertical down/up to target
                _pathPoints.Add(new Vector2(entryX, end.y));
            }

            _pathPoints.Add(end);

            return _pathPoints;
        }

        // ---- Rendering ----

        private void OnGenerateVisualContent(MeshGenerationContext mgc)
        {
            if (_from == null || _to == null)
            {
                return;
            }

            var path = ComputePath();

            if (path.Count < 2)
            {
                return;
            }

            var painter = mgc.painter2D;
            var active = IsSelected || _hovered;

            painter.lineWidth = active ? Mathf.Max(_selectedWidth, LineWidth + 1.5f) : LineWidth;
            painter.strokeColor = active ? _selectedColor : LineColor;
            painter.fillColor = active ? _selectedColor : LineColor;
            painter.lineJoin = LineJoin.Round;
            painter.lineCap = LineCap.Round;

            painter.BeginPath();
            painter.MoveTo(path[0]);

            var r = CornerRadius;

            for (var i = 1; i < path.Count; i++)
            {
                if (r > 0.5f && i < path.Count - 1)
                {
                    // Rounded corner: shorten the incoming segment and arc into the next
                    var prev = path[i - 1];
                    var corner = path[i];
                    var next = path[i + 1];

                    var toPrev = (prev - corner).normalized;
                    var toNext = (next - corner).normalized;

                    // Clamp radius to half the length of the shorter adjacent segment
                    var lenPrev = Vector2.Distance(prev, corner);
                    var lenNext = Vector2.Distance(corner, next);
                    var maxR = Mathf.Min(lenPrev, lenNext) * 0.5f;
                    var cr = Mathf.Min(r, maxR);

                    if (cr > 0.5f)
                    {
                        var arcStart = corner + toPrev * cr;
                        var arcEnd = corner + toNext * cr;

                        painter.LineTo(arcStart);
                        painter.ArcTo(corner, arcEnd, cr);
                    }
                    else
                    {
                        painter.LineTo(corner);
                    }
                }
                else
                {
                    painter.LineTo(path[i]);
                }
            }

            painter.Stroke();

            // Arrow direction: along the last segment into the target
            var lastIdx = path.Count - 1;
            var lastDir = (path[lastIdx] - path[lastIdx - 1]).normalized;

            if (lastDir.sqrMagnitude > 0.0001f)
            {
                DrawArrow(painter, path[lastIdx], lastDir);
            }
        }

        // ---- Hit testing ----

        private bool IsNearLine(Vector2 localPoint, float maxDistance)
        {
            if (_from == null || _to == null)
            {
                return false;
            }

            var path = ComputePath();

            for (var i = 0; i < path.Count - 1; i++)
            {
                if (DistancePointToSegment(localPoint, path[i], path[i + 1]) <= maxDistance)
                {
                    return true;
                }
            }

            return false;
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