using System;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.VisualElements
{
    [UxmlElement]
    public partial class ItemOptionsMenu : VisualElement
    {
        public event Action DeleteItemButtonClicked;
        public event Action EditItemButtonClicked;

        private Button _deleteButton;
        private Button _editButton;

        public ItemOptionsMenu()
        {
            RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }

        public void SubscribeToEvents(Action editHandler, Action deleteHandler)
        {
            EditItemButtonClicked = editHandler;
            DeleteItemButtonClicked = deleteHandler;
        }

        public void UnsubscribeFromEvents()
        {
            EditItemButtonClicked = null;
            DeleteItemButtonClicked = null;
        }

        private void OnAttachToPanel(AttachToPanelEvent _)
        {
            _deleteButton = this.Q<Button>("DeleteItemButton");
            _editButton = this.Q<Button>("EditItemButton");

            if (_deleteButton != null)
            {
                _deleteButton.clicked += OnDeleteClicked;
            }

            if (_editButton != null)
            {
                _editButton.clicked += OnEditClicked;
            }
        }

        private void OnDetachFromPanel(DetachFromPanelEvent _)
        {
            // Clean up to prevent leaks / duplicate subscriptions
            if (_deleteButton != null)
            {
                _deleteButton.clicked -= OnDeleteClicked;
            }

            if (_editButton != null)
            {
                _editButton.clicked -= OnEditClicked;
            }
        }

        private void OnDeleteClicked()
        {
            DeleteItemButtonClicked?.Invoke();
        }

        private void OnEditClicked()
        {
            EditItemButtonClicked?.Invoke();
        }
    }
}