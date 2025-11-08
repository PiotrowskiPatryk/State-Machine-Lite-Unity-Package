using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Forms
{
    public abstract class FormBaseWindow<TData> : EditorWindow where TData : class
    {
        protected Action<TData> OnSubmitted;
        protected Action<TData> OnCancelled;

        protected TData Data;

        [SerializeField]
        private VisualTreeAsset _formVisualTreeAsset;

        public static void Show<TWindowType>(TData data, Action<TData> onSubmitted, Action<TData> onCancelled = null)
            where TWindowType : FormBaseWindow<TData>
        {
            var window = CreateInstance<TWindowType>();
            window.Data = data;
            window.OnSubmitted = onSubmitted;
            window.OnCancelled = onCancelled;
            window.Show();
        }

        protected virtual void OnCreatedGUI()
        {
        }

        protected abstract bool IsInputDataValid();

        protected void Submit()
        {
            if (IsInputDataValid())
            {
                OnSubmitted?.Invoke(Data);
                Close();
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Invalid input data.", "OK");
            }
        }

        protected void Cancel()
        {
            OnCancelled?.Invoke(Data);

            Close();
        }

        private void CreateGUI()
        {
            var formContainer = Instantiate(_formVisualTreeAsset).CloneTree();

            rootVisualElement.Add(formContainer);
            rootVisualElement.dataSource = Data;

            formContainer.style.flexGrow = 1;
            formContainer.style.flexShrink = 0;
            formContainer.style.flexBasis = 0;

            OnCreatedGUI();
        }
    }
}