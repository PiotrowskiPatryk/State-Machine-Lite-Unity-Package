using Dev.Cortez.StateMachines.Core.Trigger;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.PropertyDrawer
{
    [CustomPropertyDrawer(typeof(DefaultTriggerPayload))]
    public class DefaultTriggerPayloadPropertyDrawer : UnityEditor.PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();

            // Find properties
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

            // Initial state
            UpdateActivationVisibility(activationRuleProp);
            UpdateDeactivationVisibility(deactivationRuleProp);

            // Track changes - use TrackPropertyValue for property drawers which is reliable
            root.TrackPropertyValue(activationRuleProp, UpdateActivationVisibility);
            root.TrackPropertyValue(deactivationRuleProp, UpdateDeactivationVisibility);

            return root;

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