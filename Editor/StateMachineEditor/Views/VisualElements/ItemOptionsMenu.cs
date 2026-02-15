#region

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

#endregion

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Views.VisualElements
{
    [UxmlElement]
    public partial class ItemOptionsMenu : VisualElement
    {
        private const string EDIT_ICON_PATH =
            "Packages/dev.cortez.state-machines/Runtime/Assets/Sprites/Icon_Edit.png";

        private const string DELETE_ICON_PATH =
            "Packages/dev.cortez.state-machines/Runtime/Assets/Sprites/Icon_Delete.png";

        public event Action DeleteItemButtonClicked;
        public event Action EditItemButtonClicked;

        private readonly Button _editButton;
        private readonly Button _deleteButton;

        public ItemOptionsMenu()
        {
            // Container layout – matches the old UXML styling
            style.flexDirection = FlexDirection.Row;
            style.alignItems = Align.Center;
            style.justifyContent = Justify.FlexStart;
            style.alignContent = Align.Auto;
            style.flexGrow = 1;
            style.flexShrink = 0;
            style.alignSelf = Align.Center;

            // Edit button
            _editButton = CreateIconButton("EditItemButton", "Edit", EDIT_ICON_PATH);
            _editButton.clicked += OnEditClicked;
            Add(_editButton);

            // Spacer
            var spacer = new VisualElement
            {
                name = "Spacer",
                style =
                {
                    flexGrow = 0,
                    width = 20
                }
            };
            Add(spacer);

            // Delete button
            _deleteButton = CreateIconButton("DeleteItemButton", "Delete", DELETE_ICON_PATH);
            _deleteButton.clicked += OnDeleteClicked;
            Add(_deleteButton);

            // Register cleanup callback
            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }

        private void OnDetachFromPanel(DetachFromPanelEvent _)
        {
            // Clean up button subscriptions to prevent memory leaks
            if (_editButton != null)
            {
                _editButton.clicked -= OnEditClicked;
            }

            if (_deleteButton != null)
            {
                _deleteButton.clicked -= OnDeleteClicked;
            }
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

        private void OnDeleteClicked()
        {
            DeleteItemButtonClicked?.Invoke();
        }

        private void OnEditClicked()
        {
            EditItemButtonClicked?.Invoke();
        }

        private static Button CreateIconButton(string buttonName, string tooltip, string iconPath)
        {
            var button = new Button
            {
                name = buttonName,
                tooltip = tooltip
            };

            var icon = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);

            if (icon != null)
            {
                button.iconImage = new Background { texture = icon };
            }

            // Replicate the UXML inline styles
            button.style.borderTopWidth = 0;
            button.style.borderRightWidth = 0;
            button.style.borderBottomWidth = 0;
            button.style.borderLeftWidth = 0;
            button.style.backgroundColor = new Color(0.737f, 0.737f, 0.737f, 0f);
            button.style.width = 16;
            button.style.height = 16;
            button.style.paddingTop = 0;
            button.style.paddingRight = 0;
            button.style.paddingBottom = 0;
            button.style.paddingLeft = 0;
            button.style.marginTop = 0;
            button.style.marginRight = 0;
            button.style.marginBottom = 0;
            button.style.marginLeft = 0;

            return button;
        }
    }
}