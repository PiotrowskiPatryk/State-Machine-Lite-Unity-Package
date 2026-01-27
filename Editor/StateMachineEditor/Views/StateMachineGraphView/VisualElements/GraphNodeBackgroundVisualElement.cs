using UnityEngine;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Views.StateMachineGraphView.VisualElements
{
    /// <summary>
    ///     VisualElement that renders a panning/zooming background grid and hosts graph content.
    ///     - Middle mouse drag pans.
    ///     - Mouse wheel zooms with fixed limits.
    ///     Children added to this element are placed in its content container which is transformed.
    /// </summary>
    [UxmlElement]
    public partial class GraphNodeBackgroundVisualElement : VisualElement
    {
        // UI: Zoom slider overlay
        private readonly Slider _zoomSlider;
        private readonly VisualElement _panContainer;
        private readonly VisualElement _zoomContainer;
        private readonly VisualElement _overlayContainer;
        private readonly Vector2Field _centerField;
        private readonly Button _applyCenterButton;
        private float _zoom = 1f;
        private Vector2 _pan = Vector2.zero; // in pixels

        // panning state
        private bool _panning;
        private Vector2 _panStartMouseLocal;

        private Vector2 _panStart;

        // ---- Grid settings ----
        private float _baseMinorGridSize = 20f; // pixels at zoom = 1
        private int _majorLineEvery = 5; // every N minor cells
        private Color _minorColor = new(1f, 1f, 1f, 0.05f);
        private Color _majorColor = new(1f, 1f, 1f, 0.12f);
        private float _lineWidth = 1f;

        // ---- Zoom settings ----
        private float _minZoom = 0.25f;
        private float _maxZoom = 2.5f;
        private float _zoomStep = 1.1f; // multiplicative step per wheel notches

        // ---- Pan bounds settings ----
        private bool _limitPanToBounds;

        // Content bounds are defined in content (logical) coordinates, prior to zoom and pan.
        private float _boundsX = -10000f;
        private float _boundsY = -10000f;
        private float _boundsWidth = 20000f;
        private float _boundsHeight = 20000f;

        [UxmlAttribute("base-minor-grid-size")]
        public float BaseMinorGridSize
        {
            get => _baseMinorGridSize;
            set
            {
                var v = Mathf.Max(1f, value);

                if (Mathf.Approximately(_baseMinorGridSize, v))
                {
                    return;
                }

                _baseMinorGridSize = v;
                MarkDirtyRepaint();
            }
        }

        [UxmlAttribute("major-line-every")]
        public int MajorLineEvery
        {
            get => _majorLineEvery;
            set
            {
                var v = Mathf.Max(1, value);

                if (_majorLineEvery == v)
                {
                    return;
                }

                _majorLineEvery = v;
                MarkDirtyRepaint();
            }
        }

        [UxmlAttribute("minor-color")]
        public Color MinorColor
        {
            get => _minorColor;
            set
            {
                if (_minorColor.Equals(value))
                {
                    return;
                }

                _minorColor = value;
                MarkDirtyRepaint();
            }
        }

        [UxmlAttribute("major-color")]
        public Color MajorColor
        {
            get => _majorColor;
            set
            {
                if (_majorColor.Equals(value))
                {
                    return;
                }

                _majorColor = value;
                MarkDirtyRepaint();
            }
        }

        [UxmlAttribute("line-width")]
        public float LineWidth
        {
            get => _lineWidth;
            set
            {
                var v = Mathf.Max(0.1f, value);

                if (Mathf.Approximately(_lineWidth, v))
                {
                    return;
                }

                _lineWidth = v;
                MarkDirtyRepaint();
            }
        }

        [UxmlAttribute("min-zoom")]
        public float MinZoom
        {
            get => _minZoom;
            set
            {
                var v = Mathf.Max(0.0001f, value);

                if (Mathf.Approximately(_minZoom, v))
                {
                    return;
                }

                _minZoom = v;

                // Ensure bounds consistency
                if (_maxZoom < _minZoom)
                {
                    _maxZoom = _minZoom;
                }

                // Sync slider bounds and value
                if (_zoomSlider != null)
                {
                    _zoomSlider.lowValue = _minZoom;
                    _zoomSlider.highValue = _maxZoom;

                    if (!Mathf.Approximately(_zoomSlider.value, _zoom))
                    {
                        _zoomSlider.SetValueWithoutNotify(Mathf.Clamp(_zoom, _minZoom, _maxZoom));
                    }
                }
            }
        }

        [UxmlAttribute("max-zoom")]
        public float MaxZoom
        {
            get => _maxZoom;
            set
            {
                var v = Mathf.Max(0.0001f, value);

                if (Mathf.Approximately(_maxZoom, v))
                {
                    return;
                }

                _maxZoom = v;

                if (_minZoom > _maxZoom)
                {
                    _minZoom = _maxZoom;
                }

                // Sync slider bounds and value
                if (_zoomSlider != null)
                {
                    _zoomSlider.lowValue = _minZoom;
                    _zoomSlider.highValue = _maxZoom;

                    if (!Mathf.Approximately(_zoomSlider.value, _zoom))
                    {
                        _zoomSlider.SetValueWithoutNotify(Mathf.Clamp(_zoom, _minZoom, _maxZoom));
                    }
                }
            }
        }

        [UxmlAttribute("zoom-step")]
        public float ZoomStep
        {
            get => _zoomStep;
            set
            {
                // Keep sensible range; 1 means no zoom per step
                var v = Mathf.Max(1f, value);

                if (Mathf.Approximately(_zoomStep, v))
                {
                    return;
                }

                _zoomStep = v;
            }
        }

        // ---- Pan bounds (Inspector) ----
        [UxmlAttribute("limit-pan")]
        public bool LimitPanToBounds
        {
            get => _limitPanToBounds;
            set
            {
                if (_limitPanToBounds == value)
                {
                    return;
                }

                _limitPanToBounds = value;
            }
        }

        [UxmlAttribute("bounds-x")]
        public float BoundsX
        {
            get => _boundsX;
            set
            {
                if (Mathf.Approximately(_boundsX, value))
                {
                    return;
                }

                _boundsX = value;
            }
        }

        [UxmlAttribute("bounds-y")]
        public float BoundsY
        {
            get => _boundsY;
            set
            {
                if (Mathf.Approximately(_boundsY, value))
                {
                    return;
                }

                _boundsY = value;
            }
        }

        [UxmlAttribute("bounds-width")]
        public float BoundsWidth
        {
            get => _boundsWidth;
            set
            {
                var v = Mathf.Max(0f, value);

                if (Mathf.Approximately(_boundsWidth, v))
                {
                    return;
                }

                _boundsWidth = v;
            }
        }

        [UxmlAttribute("bounds-height")]
        public float BoundsHeight
        {
            get => _boundsHeight;
            set
            {
                var v = Mathf.Max(0f, value);

                if (Mathf.Approximately(_boundsHeight, v))
                {
                    return;
                }

                _boundsHeight = v;
            }
        }

        /// <summary>
        ///     Content bounds in content (logical) space. Used to limit panning when <see cref="LimitPanToBounds" /> is true.
        /// </summary>
        public Rect ContentBounds
        {
            get => new(_boundsX, _boundsY, _boundsWidth, _boundsHeight);
            set
            {
                if (Mathf.Approximately(_boundsX, value.x) && Mathf.Approximately(_boundsY, value.y) &&
                    Mathf.Approximately(_boundsWidth, value.width) && Mathf.Approximately(_boundsHeight, value.height))
                {
                    return;
                }

                _boundsX = value.x;
                _boundsY = value.y;
                _boundsWidth = Mathf.Max(0f, value.width);
                _boundsHeight = Mathf.Max(0f, value.height);
            }
        }

        public float Zoom
        {
            get => _zoom;
            set
            {
                var clamped = Mathf.Clamp(value, MinZoom, MaxZoom);

                if (Mathf.Approximately(_zoom, clamped))
                {
                    return;
                }

                _zoom = clamped;
                // Re-clamp pan because permissible range depends on zoom and viewport size
                _pan = ClampPan(_pan);
                ApplyTransform();

                // Keep UI slider in sync without causing feedback loop
                if (_zoomSlider != null && !Mathf.Approximately(_zoomSlider.value, _zoom))
                {
                    _zoomSlider.SetValueWithoutNotify(_zoom);
                }

                MarkDirtyRepaint();
            }
        }

        public Vector2 Pan
        {
            get => _pan;
            set
            {
                var clamped = ClampPan(value);

                if (_pan == clamped)
                {
                    return;
                }

                _pan = clamped;
                ApplyTransform();
                MarkDirtyRepaint();
            }
        }

        // Expose content container for consumers if needed
        public VisualElement Content { get; }

        // Make adding children add into the content container, keeping API simple
        public override VisualElement contentContainer => Content;

        public GraphNodeBackgroundVisualElement()
        {
            pickingMode = PickingMode.Position; // receive pointer events

            // Fill parent by default
            style.position = Position.Absolute;
            style.left = 0;
            style.right = 0;
            style.top = 0;
            style.bottom = 0;

            // Draw grid in background
            generateVisualContent += OnGenerateVisualContent;

            // Content that is transformed by pan/zoom using nested containers:
            // Pan is applied to _panContainer (translate), Zoom is applied to _zoomContainer (scale).
            _panContainer = new VisualElement
            {
                name = "PanContainer",
                pickingMode = PickingMode.Position
            };
            _panContainer.style.position = Position.Absolute;
            _panContainer.style.left = 0;
            _panContainer.style.right = 0;
            _panContainer.style.top = 0;
            _panContainer.style.bottom = 0;

            _zoomContainer = new VisualElement
            {
                name = "ZoomContainer",
                pickingMode = PickingMode.Position
            };
            _zoomContainer.style.position = Position.Absolute;
            _zoomContainer.style.left = 0;
            _zoomContainer.style.right = 0;
            _zoomContainer.style.top = 0;
            _zoomContainer.style.bottom = 0;
            _zoomContainer.style.transformOrigin = new TransformOrigin(0, 0, 0);

            Content = new VisualElement
            {
                name = "GraphContent",
                pickingMode = PickingMode.Position
            };
            Content.style.position = Position.Absolute;
            Content.style.left = 0;
            Content.style.right = 0;
            Content.style.top = 0;
            Content.style.bottom = 0;

            // Build hierarchy: this -> _panContainer -> _zoomContainer -> Content
            hierarchy.Add(_panContainer);
            _panContainer.Add(_zoomContainer);
            _zoomContainer.Add(Content);

            _overlayContainer = new VisualElement
            {
                name = "Overlay",
                pickingMode = PickingMode.Ignore
            };
            _overlayContainer.style.position = Position.Absolute;
            _overlayContainer.style.right = 8;
            _overlayContainer.style.bottom = 8;
            _overlayContainer.style.flexDirection = FlexDirection.Column;
            _overlayContainer.style.alignItems = Align.FlexEnd;
            hierarchy.Add(_overlayContainer);

            _zoomSlider = new Slider("Zoom", MinZoom, MaxZoom)
            {
                tooltip = "Zoom",
                value = Zoom,
                pickingMode = PickingMode.Position,
                lowValue = MinZoom,
                highValue = MaxZoom
            };
            _zoomSlider.style.width = 200;
            _zoomSlider.RegisterValueChangedCallback(e =>
            {
                if (!Mathf.Approximately(Zoom, e.newValue))
                {
                    Zoom = e.newValue;
                }
            });
            _overlayContainer.Add(_zoomSlider);

            _centerField = new Vector2Field("Center") { pickingMode = PickingMode.Position };
            _centerField.style.width = 200;
            _centerField.RegisterValueChangedCallback(e =>
            {
                // Do not immediately recenter on each change; only update display.
            });
            _overlayContainer.Add(_centerField);

            _applyCenterButton = new Button(() => { CenterOn(_centerField.value); })
            {
                text = "Apply Center"
            };
            _applyCenterButton.style.width = 200;
            _overlayContainer.Add(_applyCenterButton);

            RegisterCallbacks();
            ApplyTransform();
        }

        public void CenterOn(Vector2 contentPosition)
        {
            var viewportRect = contentRect;
            var desired = viewportRect.center - contentPosition * Zoom;
            Pan = desired;
        }

        private void RegisterCallbacks()
        {
            RegisterCallback<PointerDownEvent>(OnPointerDown, TrickleDown.TrickleDown);
            RegisterCallback<PointerMoveEvent>(OnPointerMove);
            RegisterCallback<PointerUpEvent>(OnPointerUp);
            RegisterCallback<PointerCaptureOutEvent>(_ => _panning = false);
            RegisterCallback<WheelEvent>(OnWheel, TrickleDown.TrickleDown);
            RegisterCallback<GeometryChangedEvent>(_ => { SyncCenterField(); });
        }

        private void OnPointerDown(PointerDownEvent evt)
        {
            if (evt.button != 2)
            {
                return; // middle mouse only
            }

            _panning = true;
            _panStart = Pan;
            _panStartMouseLocal = this.WorldToLocal(evt.position);
            this.CapturePointer(evt.pointerId);
            evt.StopPropagation();
        }

        private void OnPointerMove(PointerMoveEvent evt)
        {
            if (!_panning || !this.HasPointerCapture(evt.pointerId))
            {
                return;
            }

            var current = this.WorldToLocal(evt.position);
            var delta = current - _panStartMouseLocal;
            Pan = _panStart + delta; // pixels
            evt.StopPropagation();
        }

        private void OnPointerUp(PointerUpEvent evt)
        {
            if (!_panning || evt.button != 2)
            {
                return;
            }

            _panning = false;
            this.ReleasePointer(evt.pointerId);
            evt.StopPropagation();
        }

        private void OnWheel(WheelEvent evt)
        {
            // Positive delta.y means scroll up (away), typically zoom out. We'll invert so wheel up zooms out.
            // Use multiplicative steps per 120 units of delta (commonly the notch).
            var steps = -evt.delta.y / 120f; // negative to make wheel up zoom in (common UX)

            if (Mathf.Approximately(steps, 0f))
            {
                return;
            }

            var factor = Mathf.Pow(ZoomStep, steps);

            var oldZoom = Zoom;
            var newZoom = Mathf.Clamp(oldZoom * factor, MinZoom, MaxZoom);

            if (Mathf.Approximately(oldZoom, newZoom))
            {
                return;
            }

            // Zoom around mouse position so the point under cursor stays fixed
            var mouseLocal = this.WorldToLocal(evt.mousePosition);
            var contentPoint = (mouseLocal - Pan) / oldZoom;
            Zoom = newZoom; // applies transform and repaints
            Pan = mouseLocal - contentPoint * newZoom;

            evt.StopPropagation();
        }

        private void ApplyTransform()
        {
            if (_panContainer != null)
            {
                _panContainer.style.translate = new Translate(Pan.x, Pan.y);
            }

            if (_zoomContainer != null)
            {
                _zoomContainer.style.scale = new Scale(new Vector3(Zoom, Zoom, 1f));
            }

            SyncCenterField();
        }

        /// <summary>
        ///     Clamp the requested pan to keep the viewport within <see cref="ContentBounds" /> when limiting is enabled.
        ///     Pan is in pixels (local space). Bounds are in content space, before pan/zoom.
        /// </summary>
        private Vector2 ClampPan(Vector2 desiredPan)
        {
            if (!LimitPanToBounds)
            {
                return desiredPan;
            }

            var viewportRect = contentRect;

            if (viewportRect.width <= 0f || viewportRect.height <= 0f)
            {
                return desiredPan;
            }

            var boundsRect = ContentBounds;
            var zoom = Zoom;

            var minPanX = viewportRect.xMax - zoom * (boundsRect.xMin + boundsRect.width);
            var maxPanX = viewportRect.xMin - zoom * boundsRect.xMin;
            var minPanY = viewportRect.yMax - zoom * (boundsRect.yMin + boundsRect.height);
            var maxPanY = viewportRect.yMin - zoom * boundsRect.yMin;

            var boundsScreenWidth = zoom * boundsRect.width;
            var boundsScreenHeight = zoom * boundsRect.height;

            var panX = boundsScreenWidth < viewportRect.width - 0.01f
                ? viewportRect.center.x - zoom * (boundsRect.xMin + boundsRect.width * 0.5f)
                : Mathf.Clamp(desiredPan.x, minPanX, maxPanX);

            var panY = boundsScreenHeight < viewportRect.height - 0.01f
                ? viewportRect.center.y - zoom * (boundsRect.yMin + boundsRect.height * 0.5f)
                : Mathf.Clamp(desiredPan.y, minPanY, maxPanY);

            return new Vector2(panX, panY);
        }

        private Vector2 GetViewportCenterContentPosition()
        {
            var viewportRect = contentRect;

            if (viewportRect.width <= 0f || viewportRect.height <= 0f)
            {
                return Vector2.zero;
            }

            return (viewportRect.center - Pan) / Mathf.Max(Zoom, 0.0001f);
        }

        private void SyncCenterField()
        {
            if (_centerField == null)
            {
                return;
            }

            var currentCenter = GetViewportCenterContentPosition();

            if (_centerField.value != currentCenter)
            {
                _centerField.SetValueWithoutNotify(currentCenter);
            }
        }

        private void OnGenerateVisualContent(MeshGenerationContext mgc)
        {
            var viewportRect = contentRect;

            if (viewportRect.width <= 0 || viewportRect.height <= 0)
            {
                return;
            }

            var minorStep = Mathf.Max(1f, BaseMinorGridSize * Zoom);
            var majorStep = minorStep * Mathf.Max(1, MajorLineEvery);

            var painter = mgc.painter2D;
            painter.lineWidth = LineWidth;

            void DrawLines(float step, Color color, bool vertical)
            {
                painter.strokeColor = color;
                painter.BeginPath();
                var offset = vertical ? Mod(Pan.x, step) : Mod(Pan.y, step);

                if (vertical)
                {
                    for (var x = viewportRect.xMin + offset; x < viewportRect.xMax; x += step)
                    {
                        painter.MoveTo(new Vector2(x, viewportRect.yMin));
                        painter.LineTo(new Vector2(x, viewportRect.yMax));
                    }
                }
                else
                {
                    for (var y = viewportRect.yMin + offset; y < viewportRect.yMax; y += step)
                    {
                        painter.MoveTo(new Vector2(viewportRect.xMin, y));
                        painter.LineTo(new Vector2(viewportRect.xMax, y));
                    }
                }

                painter.strokeGradient = default;
                painter.Stroke();
            }

            DrawLines(minorStep, MinorColor, true);
            DrawLines(minorStep, MinorColor, false);
            DrawLines(majorStep, MajorColor, true);
            DrawLines(majorStep, MajorColor, false);
        }

        private static float Mod(float a, float m)
        {
            if (m <= 0f)
            {
                return 0f;
            }

            var res = a % m;

            if (res < 0)
            {
                res += m;
            }

            return res;
        }
    }
}