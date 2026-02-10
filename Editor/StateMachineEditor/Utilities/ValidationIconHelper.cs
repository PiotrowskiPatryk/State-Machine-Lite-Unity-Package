using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Utilities
{
    /// <summary>
    /// Helper class for displaying validation icons in list views.
    /// Follows DRY principle by centralizing validation icon logic.
    /// </summary>
    public static class ValidationIconHelper
    {
        private const string INVALID_ICON_PATH =
            "Packages/dev.cortez.state-machines/Runtime/Assets/Sprites/Icon_Invalid.png";

        private const string VALIDATION_ICON_NAME = "ValidationIcon";
        private const string VALIDATION_TOOLTIP = "This item has validation errors";

        private static Texture2D _cachedIcon;

        /// <summary>
        /// Gets the cached invalid icon texture.
        /// </summary>
        public static Texture2D InvalidIcon
        {
            get
            {
                if (_cachedIcon == null)
                {
                    _cachedIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(INVALID_ICON_PATH);
                }

                return _cachedIcon;
            }
        }

        /// <summary>
        /// Creates a validation icon Image element configured with proper styling.
        /// The icon should be shown when IsValid is false.
        /// </summary>
        public static Image CreateValidationIcon()
        {
            var icon = new Image
            {
                name = VALIDATION_ICON_NAME,
                image = InvalidIcon,
                tooltip = VALIDATION_TOOLTIP,
                style =
                {
                    width = 16,
                    height = 16,
                    marginRight = 4,
                    marginLeft = 4,
                    flexShrink = 0,
                    alignSelf = Align.Center
                }
            };

            return icon;
        }

        /// <summary>
        /// Updates the visibility of a validation icon based on the IsValid property.
        /// Call this during bindItem to update icon visibility.
        /// </summary>
        /// <param name="icon">The validation icon image element</param>
        /// <param name="isValid">Whether the item is valid</param>
        public static void UpdateValidationIconVisibility(Image icon, bool isValid)
        {
            if (icon == null)
            {
                return;
            }

            icon.style.display = isValid ? DisplayStyle.None : DisplayStyle.Flex;
        }

        /// <summary>
        /// Finds or creates a validation icon within a cell container.
        /// Useful for MultiColumnListView cell binding.
        /// </summary>
        /// <param name="visualElement">The cell visual element</param>
        /// <returns>The existing or newly created validation icon</returns>
        public static Image GetOrCreateValidationIcon(VisualElement visualElement)
        {
            var icon = visualElement.Q<Image>(VALIDATION_ICON_NAME);

            if (icon != null)
            {
                return icon;
            }

            icon = CreateValidationIcon();
            visualElement.Insert(0, icon);

            return icon;
        }
    }
}