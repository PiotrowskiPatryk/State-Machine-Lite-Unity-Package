using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.Views.StateMachineGraphView
{
    /// <summary>
    /// Draws a bezier line with an arrow between two VisualElements (typically node ports).
    /// Positions are resolved lazily from the referenced elements on each repaint,
    /// so the edge follows its endpoints as they move (no per-frame subscriptions required).
    /// Also supports hit-testing and selection highlighting.
    /// </summary>
    internal sealed class TransitionEdgeElement : VisualElement
    {
        private readonly VisualElement _from;
        private readonly VisualElement _to;

        // Optional metadata for identification
        public string SourceStateId { get; }
        public string TargetStateId { get; }
        public object Tag { get; set; }

        public Color LineColor { get; set; } = new Color(0.45f, 0.70f, 1f, 0.95f);
        public float LineWidth { get; set; } = 2.0f;
        public float ArrowLength { get; set; } = 10.0f;
        public float ArrowAngleDeg { get; set; } = 26.0f;
        public float ControlOffset { get; set; } = 60.0f; // Horizontal offset for bezier handles

        // selection
        private bool _selected;
        private Color _selectedColor = new Color(1f, 0.9f, 0.2f, 1f);
        private float _selectedWidth = 3.5f;

        // click state
        private bool _pressedNearCurve;
        private int _pressedPointerId = -1;

        public event Action<TransitionEdgeElement> Clicked;

        public TransitionEdgeElement(VisualElement from, VisualElement to, string sourceStateId = null, string targetStateId = null)
        {
            pickingMode = PickingMode.Position; // enable hit testing
            _from = from;
            _to = to;
            SourceStateId = sourceStateId;
            TargetStateId = targetStateId;

            // Ensure this element covers the whole drawing area of its parent
            style.position = Position.Absolute;
            style.left = 0;
            style.top = 0;
            style.right = 0;
            style.bottom = 0;

            // hook painter
            generateVisualContent += OnGenerateVisualContent;

            // Redraw when either endpoint re-layouts
            if (_from != null) _from.RegisterCallback<GeometryChangedEvent>(_ => MarkDirtyRepaint());
            if (_to != null) _to.RegisterCallback<GeometryChangedEvent>(_ => MarkDirtyRepaint());

            // Also redraw when we are re-laid out
            RegisterCallback<GeometryChangedEvent>(_ => MarkDirtyRepaint());

            RegisterCallback<PointerDownEvent>(OnPointerDown);
            RegisterCallback<PointerUpEvent>(OnPointerUp);
            RegisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut);
        }

        public void SetSelected(bool selected)
        {
            if (_selected == selected) return;
            _selected = selected;
            MarkDirtyRepaint();
        }

        private void OnPointerDown(PointerDownEvent evt)
        {
            if (evt.button != 0)
            {
                return;
            }
            var local = this.WorldToLocal(evt.position);
            if (IsNearCurve(local, 12f))
            {
                _pressedNearCurve = true;
                _pressedPointerId = evt.pointerId;
                this.CapturePointer(_pressedPointerId);
                evt.StopPropagation();
            }
        }

        private void OnPointerUp(PointerUpEvent evt)
        {
            if (evt.button != 0)
            {
                return;
            }

            var local = this.WorldToLocal(evt.position);
            var wasPressed = _pressedNearCurve && (_pressedPointerId == evt.pointerId || _pressedPointerId == -1);
            var isNear = IsNearCurve(local, 12f);
            if (wasPressed && isNear)
            {
                Clicked?.Invoke(this);
                evt.StopPropagation();
            }
            if (_pressedPointerId != -1 && this.HasPointerCapture(_pressedPointerId))
            {
                this.ReleasePointer(_pressedPointerId);
            }
            _pressedNearCurve = false;
            _pressedPointerId = -1;
        }

        private void OnPointerCaptureOut(PointerCaptureOutEvent evt)
        {
            _pressedNearCurve = false;
            _pressedPointerId = -1;
        }

        public override bool ContainsPoint(Vector2 localPoint)
        {
            // Improve picking by accepting clicks near the curve
            return IsNearCurve(localPoint, 12f);
        }

        private void OnGenerateVisualContent(MeshGenerationContext mgc)
        {
            if (_from == null || _to == null)
            {
                return;
            }

            // Resolve endpoints in our local space
            Vector2 start = this.WorldToLocal(_from.worldBound.center);
            Vector2 end = this.WorldToLocal(_to.worldBound.center);

            // Control points for a pleasing curve (assume left-to-right primarily)
            float dx = Mathf.Abs(end.x - start.x);
            float offset = Mathf.Max(ControlOffset, dx * 0.35f);

            Vector2 c1 = start + new Vector2(offset, 0f);
            Vector2 c2 = end - new Vector2(offset, 0f);

            var painter = mgc.painter2D;
            painter.lineWidth = _selected ? _selectedWidth : LineWidth;
            painter.strokeColor = _selected ? _selectedColor : LineColor;
            painter.fillColor = _selected ? _selectedColor : LineColor;

            // Approximate cubic bezier with a polyline (Painter2D lacks a direct cubic call in all versions)
            painter.BeginPath();
            painter.MoveTo(start);
            const int segments = 20;
            for (int i = 1; i <= segments; i++)
            {
                float t = i / (float)segments;
                Vector2 p = Cubic(start, c1, c2, end, t);
                painter.LineTo(p);
            }
            painter.Stroke();

            // Arrow head at end
            Vector2 tangent = (end - c2).normalized;
            if (tangent.sqrMagnitude > 0.0001f)
            {
                DrawArrow(painter, end, tangent);
            }
        }

        private bool IsNearCurve(Vector2 localPoint, float maxDistance)
        {
            if (_from == null || _to == null) return false;
            Vector2 start = this.WorldToLocal(_from.worldBound.center);
            Vector2 end = this.WorldToLocal(_to.worldBound.center);
            float dx = Mathf.Abs(end.x - start.x);
            float offset = Mathf.Max(ControlOffset, dx * 0.35f);
            Vector2 c1 = start + new Vector2(offset, 0f);
            Vector2 c2 = end - new Vector2(offset, 0f);

            const int segments = 20;
            Vector2 prev = start;
            for (int i = 1; i <= segments; i++)
            {
                float t = i / (float)segments;
                Vector2 p = Cubic(start, c1, c2, end, t);
                float d = DistancePointToSegment(localPoint, prev, p);
                if (d <= maxDistance) return true;
                prev = p;
            }
            return false;
        }

        private static float DistancePointToSegment(Vector2 p, Vector2 a, Vector2 b)
        {
            var ab = b - a;
            float t = Vector2.Dot(p - a, ab) / (ab.sqrMagnitude + 1e-6f);
            t = Mathf.Clamp01(t);
            var closest = a + ab * t;
            return Vector2.Distance(p, closest);
        }

        private void DrawArrow(Painter2D painter, Vector2 tip, Vector2 direction)
        {
            // Compute two arrow wing points
            float rad = ArrowAngleDeg * Mathf.Deg2Rad;
            Vector2 back = tip - direction.normalized * ArrowLength;
            Vector2 dirLeft = Rotate(direction, rad);
            Vector2 dirRight = Rotate(direction, -rad);
            Vector2 left = tip - dirLeft.normalized * ArrowLength * 0.7f;
            Vector2 right = tip - dirRight.normalized * ArrowLength * 0.7f;

            // Draw filled triangular arrow head
            painter.BeginPath();
            painter.MoveTo(tip);
            painter.LineTo(left);
            painter.LineTo(back);
            painter.LineTo(right);
            painter.ClosePath();
            painter.Fill();
        }

        private static Vector2 Rotate(Vector2 v, float radians)
        {
            float cos = Mathf.Cos(radians);
            float sin = Mathf.Sin(radians);
            return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
        }

        private static Vector2 Cubic(Vector2 p0, Vector2 c1, Vector2 c2, Vector2 p1, float t)
        {
            float u = 1f - t;
            float uu = u * u;
            float uuu = uu * u;
            float tt = t * t;
            float ttt = tt * t;
            return p0 * uuu + c1 * (3f * uu * t) + c2 * (3f * u * tt) + p1 * ttt;
        }
    }
}
