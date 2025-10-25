using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Windows
{
    /// <summary>
    ///     Lightweight, reusable base for small editor "form" windows built with UI Toolkit.
    ///     Encapsulates common window setup, UXML cloning, button wiring, and submit lifecycle.
    ///     Derived classes should focus only on binding data, validation, and gathering results.
    /// </summary>
    public abstract class BaseEditorFormWindow<TResult> : EditorWindow
    {
        [SerializeField]
        protected VisualTreeAsset _visualTreeAsset;

        protected Button _submitButton;
        protected Button _cancelButton;
        protected VisualElement _rootContainer;

        /// <summary>
        ///     External submit callback set by implementors.
        /// </summary>
        protected Action<TResult> OnSubmit;

        protected abstract string SubmitButtonLabel { get; }
        protected abstract string CancelButtonLabel { get; }

        public void CreateGUI()
        {
            var root = rootVisualElement;
            root.Clear();
            root.style.flexGrow = 1f;
            root.StretchToParentSize();

            if (_visualTreeAsset != null)
            {
                _visualTreeAsset.CloneTree(root);
                _rootContainer = root.Q<VisualElement>("Root") ?? root;
                _rootContainer.style.flexGrow = 1f;
                _rootContainer.StretchToParentSize();
                _rootContainer.AddToClassList("unity-theme-variables");

                _submitButton = root.Q<Button>("CreateButton") ?? root.Q<Button>("SubmitButton");
                _cancelButton = root.Q<Button>("CancelButton");
            }
            else
            {
                _rootContainer = new VisualElement { style = { flexGrow = 1f } };
                root.Add(_rootContainer);

                var buttons = new VisualElement
                {
                    style =
                    {
                        flexDirection = FlexDirection.Row,
                        justifyContent = Justify.FlexEnd
                    }
                };

                root.Add(buttons);
                _submitButton = new Button { text = SubmitButtonLabel };
                _cancelButton = new Button { text = CancelButtonLabel };
                buttons.Add(_submitButton);
                buttons.Add(_cancelButton);
            }

            if (_submitButton != null)
            {
                _submitButton.text = SubmitButtonLabel;
            }

            if (_cancelButton != null)
            {
                _cancelButton.text = CancelButtonLabel;
            }

            if (_cancelButton != null)
            {
                _cancelButton.clicked += Close;
            }

            if (_submitButton != null)
            {
                _submitButton.clicked += OnSubmitClicked;
            }

            BuildView();
        }

        /// <summary>
        ///     Called during CreateGUI after the UXML has been cloned and the base layout is ready.
        ///     Implementors should build the view (bind data, create property fields, etc.).
        /// </summary>
        protected abstract void BuildView();

        /// <summary>
        ///     Validate the form before submitting.
        ///     Return true if valid; otherwise false and set error message.
        /// </summary>
        protected abstract bool Validate(out string errorMessage);

        /// <summary>
        ///     Gather result payload for the form.
        /// </summary>
        protected abstract TResult GatherResult();

        protected static Rect GetCenteredPosition(Vector2 size)
        {
            var main = EditorGUIUtility.GetMainWindowPosition();
            var x = main.x + (main.width - size.x) * 0.5f;
            var y = main.y + (main.height - size.y) * 0.5f;

            return new Rect(x, y, size.x, size.y);
        }

        private void OnSubmitClicked()
        {
            if (!Validate(out var error))
            {
                if (!string.IsNullOrEmpty(error))
                {
                    EditorUtility.DisplayDialog("Validation", error, "OK");
                }

                return;
            }

            var result = GatherResult();
            OnSubmit?.Invoke(result);
            Close();
        }
    }
}