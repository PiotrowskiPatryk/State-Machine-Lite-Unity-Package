using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Forms
{
    public abstract class FormBaseWindow<TData> where TData : class
    {
        protected Action<TData> OnSubmitted;
        protected Action<TData> OnCancelled;
        protected VisualElement RootVisualElement;

        protected TData Data;
        protected abstract string FORM_PATH { get; }

        public VisualElement Show(TData data, Action<TData> onSubmitted, Action<TData> onCancelled = null)
        {
            var form = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(FORM_PATH).Instantiate();

            RootVisualElement = form.contentContainer;

            form.contentContainer.dataSource = data;
            Data = data;
            OnSubmitted = onSubmitted;
            OnCancelled = onCancelled;

            form.style.flexGrow = 1;
            form.style.flexShrink = 0;
            form.style.flexBasis = 0;

            OnShown();

            return form;
        }

        protected virtual void OnShown()
        {
        }

        protected abstract bool IsInputDataValid();

        protected void Submit()
        {
            if (IsInputDataValid())
            {
                OnSubmitted?.Invoke(Data);
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Invalid input data.", "OK");
            }
        }

        protected void Cancel()
        {
            OnCancelled?.Invoke(Data);
        }
    }
}