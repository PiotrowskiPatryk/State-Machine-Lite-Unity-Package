#region

using System.Collections.Generic;
using Dev.Cortez.StateMachines.Core.Trigger;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

#endregion

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.PropertyDrawer
{
    [CustomPropertyDrawer(typeof(DefaultTriggerPayload), true)]
    public class DefaultTriggerPayloadPropertyDrawer : UnityEditor.PropertyDrawer
    {
        /// <summary>
        ///     The serialized field names belonging to the base <see cref="DefaultTriggerPayload" /> class.
        ///     Used to avoid drawing these fields twice when rendering a child class.
        /// </summary>
        private static readonly HashSet<string> BasePropertyNames = new()
        {
            "_triggerActivationRule",
            "_triggerActivationFrameDelay",
            "_triggerActivationTimeDelayInSeconds",
            "_triggerDeactivationRule",
            "_triggerDeactivationFrameDelay",
            "_triggerDeactivationTimeDelayInSeconds"
        };

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();

            var isChildType = fieldInfo.FieldType != typeof(DefaultTriggerPayload);

            if (isChildType)
            {
                DrawChildPropertyGUI(property, root);
            }
            else
            {
                DrawBasePropertyGUI(property, root);
            }

            return root;
        }

        private void DrawBasePropertyGUI(SerializedProperty property, VisualElement root)
        {
            var activationRuleProp = property.FindPropertyRelative("_triggerActivationRule");
            var activationFrameDelayProp = property.FindPropertyRelative("_triggerActivationFrameDelay");
            var activationTimeDelayProp = property.FindPropertyRelative("_triggerActivationTimeDelayInSeconds");

            var deactivationRuleProp = property.FindPropertyRelative("_triggerDeactivationRule");
            var deactivationFrameDelayProp = property.FindPropertyRelative("_triggerDeactivationFrameDelay");
            var deactivationTimeDelayProp = property.FindPropertyRelative("_triggerDeactivationTimeDelayInSeconds");

            var activationRuleField = new PropertyField(activationRuleProp)
            {
                label = "When trigger should be activated:",
                tooltip = "Defines the condition for activating the trigger. \n" +
                          "Immediately: Activates when trigger activation was called. \n" +
                          "AfterFixedFrame: Activates after a set number frames. \n" +
                          "AfterTime: Activates after a set time (scaled). \n" +
                          "AfterTimeUnscaled: Activates after a set time (unscaled)."
            };

            var activationFrameDelayField = new PropertyField(activationFrameDelayProp);
            var activationTimeDelayField = new PropertyField(activationTimeDelayProp);

            root.Add(activationRuleField);
            root.Add(activationFrameDelayField);
            root.Add(activationTimeDelayField);

            var deactivationRuleField = new PropertyField(deactivationRuleProp)
            {
                label = "After trigger being activated, then deactivate it:",
                tooltip = "Defines the condition for deactivating the trigger. \n" +
                          "Never: Stays active indefinitely until manual intervention. \n" +
                          "NextFrame: Deactivates on the next frame. \n" +
                          "AfterFixedFrame: Deactivates after a set number frames. \n" +
                          "AfterTime: Deactivates after a set time (scaled). \n" +
                          "AfterTimeUnscaled: Deactivates after a set time (unscaled)."
            };

            var deactivationFrameDelayField = new PropertyField(deactivationFrameDelayProp);
            var deactivationTimeDelayField = new PropertyField(deactivationTimeDelayProp);

            root.Add(deactivationRuleField);
            root.Add(deactivationFrameDelayField);
            root.Add(deactivationTimeDelayField);

            SetupVisibilityTracking(root, activationRuleProp, activationFrameDelayField, activationTimeDelayField,
                deactivationRuleProp, deactivationFrameDelayField, deactivationTimeDelayField);
        }

        private void DrawChildPropertyGUI(SerializedProperty property, VisualElement root)
        {
            // Draw the base class properties first
            DrawBasePropertyGUI(property, root);

            // Draw additional child-specific serialized properties, skipping the base ones
            var iterator = property.Copy();
            var endProperty = iterator.GetEndProperty();

            if (iterator.NextVisible(true))
            {
                do
                {
                    if (SerializedProperty.EqualContents(iterator, endProperty))
                    {
                        break;
                    }

                    if (BasePropertyNames.Contains(iterator.name))
                    {
                        continue;
                    }

                    root.Add(new PropertyField(iterator.Copy()));
                } while (iterator.NextVisible(false));
            }
        }

        private static void SetupVisibilityTracking(
            VisualElement root,
            SerializedProperty activationRuleProp,
            PropertyField activationFrameDelayField,
            PropertyField activationTimeDelayField,
            SerializedProperty deactivationRuleProp,
            PropertyField deactivationFrameDelayField,
            PropertyField deactivationTimeDelayField)
        {
            UpdateActivationVisibility(activationRuleProp);
            UpdateDeactivationVisibility(deactivationRuleProp);

            root.TrackPropertyValue(activationRuleProp, UpdateActivationVisibility);
            root.TrackPropertyValue(deactivationRuleProp, UpdateDeactivationVisibility);

            void UpdateDeactivationVisibility(SerializedProperty changedProperty)
            {
                var rule = (TriggerDeactivationRule)changedProperty.intValue;
                deactivationFrameDelayField.style.display = rule == TriggerDeactivationRule.AfterFixedFrame
                    ? DisplayStyle.Flex
                    : DisplayStyle.None;
                deactivationTimeDelayField.style.display =
                    rule is TriggerDeactivationRule.AfterTime or TriggerDeactivationRule.AfterTimeUnscaled
                        ? DisplayStyle.Flex
                        : DisplayStyle.None;
            }

            void UpdateActivationVisibility(SerializedProperty changedProperty)
            {
                var rule = (TriggerActivationRule)changedProperty.intValue;
                activationFrameDelayField.style.display = rule == TriggerActivationRule.AfterFixedFrame
                    ? DisplayStyle.Flex
                    : DisplayStyle.None;
                activationTimeDelayField.style.display =
                    rule is TriggerActivationRule.AfterTime or TriggerActivationRule.AfterTimeUnscaled
                        ? DisplayStyle.Flex
                        : DisplayStyle.None;
            }
        }
    }
}