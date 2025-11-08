using System;
using System.Collections.Generic;
using Dev.Cortez.StateMachines.Core.Interfaces;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Data.Definition;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Utilities;
using Unity.Properties;
using UnityEditor;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.ViewModels
{
    public sealed class StateDefinitionViewModel : ViewModelBase
    {
        private readonly SerializedInstanceSwitcher<IPayload> _payloadSwitcher;

        [CreateProperty]
        public string Id
        {
            get => SerializedProperty.FindPropertyRelative(StateDefinition.ID_PROPERTY_NAME).stringValue;
            set => ApplyPropertyValueString(StateDefinition.ID_PROPERTY_NAME, value);
        }

        [CreateProperty]
        public string Name
        {
            get => SerializedProperty.FindPropertyRelative(StateDefinition.NAME_PROPERTY_NAME).stringValue;
            set => ApplyPropertyValueString(StateDefinition.NAME_PROPERTY_NAME, value);
        }

        [CreateProperty]
        public string Description
        {
            get =>
                SerializedProperty.
                    FindPropertyRelative(StateDefinition.DESCRIPTION_PROPERTY_NAME).stringValue;
            set => ApplyPropertyValueString(StateDefinition.DESCRIPTION_PROPERTY_NAME, value);
        }

        [CreateProperty]
        public SerializedProperty Payload =>
            SerializedProperty.FindPropertyRelative(StateDefinition.PAYLOAD_PROPERTY_NAME);

        [CreateProperty]
        public string TypeName
        {
            get => SerializedProperty.FindPropertyRelative(StateDefinition.TYPE_NAME_PROPERTY_NAME).stringValue;
            set => ApplyPropertyValueString(StateDefinition.TYPE_NAME_PROPERTY_NAME, value);
        }

        [CreateProperty]
        public string StateMachineTypeName
        {
            get => SerializedProperty.FindPropertyRelative(StateDefinition.STATE_MACHINE_TYPE_NAME_PROPERTY_NAME).
                stringValue;
            set => ApplyPropertyValueString(StateDefinition.STATE_MACHINE_TYPE_NAME_PROPERTY_NAME, value);
        }

        [CreateProperty]
        public string TypeNameShort => StateMachineReflectionUtilities.ToClassNameOnly(TypeName);

        [CreateProperty]
        public Vector2Int NodePosition
        {
            get =>
                SerializedProperty.
                    FindPropertyRelative(StateDefinition.NODE_POSITION_PROPERTY_NAME).vector2IntValue;
            set => ApplyPropertyValueVector2Int(StateDefinition.NODE_POSITION_PROPERTY_NAME, value);
        }

        [CreateProperty]
        public List<TransitionRuleDefinitionViewModel> Transitions
        {
            get
            {
                var transitions = new List<TransitionRuleDefinitionViewModel>();
                var transitionsProperty =
                    SerializedProperty.FindPropertyRelative(StateDefinition.TRANSITION_RULES_PROPERTY_NAME);

                for (var i = 0; i < transitionsProperty.arraySize; ++i)
                {
                    var transition = transitionsProperty.GetArrayElementAtIndex(i);
                    transitions.Add(new TransitionRuleDefinitionViewModel(transition));
                }

                return transitions;
            }
        }

        public override SerializedProperty SerializedProperty { get; }

        public StateDefinitionViewModel(SerializedProperty serializedProperty)
        {
            SerializedProperty = serializedProperty;
        }

        public StateDefinitionViewModel(string stateMachineType)
        {
            // TODO - Handle disposal
            var definitionWrapper = ScriptableObject.CreateInstance<StateDefinitionWrapper>();
            SerializedProperty =
                new SerializedObject(definitionWrapper).FindProperty(StateDefinitionWrapper.DATA_PROPERTY_NAME);
            Id = Guid.NewGuid().ToString("N");
            _payloadSwitcher = new SerializedInstanceSwitcher<IPayload>(Payload);
            StateMachineTypeName = stateMachineType;
        }

        public void AddTransition(TransitionRuleDefinitionViewModel dataTransitionRuleDefinitionViewModel)
        {
            var transitionsProperty =
                SerializedProperty.FindPropertyRelative(StateDefinition.TRANSITION_RULES_PROPERTY_NAME);

            Undo.RecordObject(transitionsProperty.serializedObject.targetObject, "Add transition");

            var so = transitionsProperty.serializedObject;
            so.Update();

            transitionsProperty.InsertArrayElementAtIndex(transitionsProperty.arraySize);
            so.ApplyModifiedProperties();

            var newTransitionSerializedProperty =
                transitionsProperty.GetArrayElementAtIndex(transitionsProperty.arraySize - 1);
            var newTransition = new TransitionRuleDefinitionViewModel(newTransitionSerializedProperty);
            newTransition.CopyFrom(dataTransitionRuleDefinitionViewModel);

            so.ApplyModifiedProperties();

            Notify(nameof(Transitions));
        }

        public void CopyFrom(StateDefinitionViewModel other)
        {
            Id = other.Id;
            Name = other.Name;
            Description = other.Description;
            TypeName = other.TypeName;
            StateMachineTypeName = other.StateMachineTypeName;
            NodePosition = other.NodePosition;
        }

        public void RemoveTransition(TransitionRuleDefinitionViewModel transitionRuleDefinitionViewModel)
        {
            if (transitionRuleDefinitionViewModel == null)
            {
                Debug.LogError("RemoveTransition called with null argument");

                return;
            }

            var transitionsProperty =
                SerializedProperty.FindPropertyRelative(StateDefinition.TRANSITION_RULES_PROPERTY_NAME);

            if (transitionsProperty == null)
            {
                Debug.LogError("Transitions property not found on StateDefinition");

                return;
            }

            var so = transitionsProperty.serializedObject;

            Undo.RecordObject(so.targetObject, "Remove transition");

            so.Update();

            // Try to find the exact array index by comparing property paths (most reliable when VMs point to same SO)
            var targetPath = transitionRuleDefinitionViewModel.SerializedProperty.propertyPath;
            var indexToRemove = -1;

            for (var i = 0; i < transitionsProperty.arraySize; i++)
            {
                var element = transitionsProperty.GetArrayElementAtIndex(i);

                if (element.propertyPath == targetPath)
                {
                    indexToRemove = i;

                    break;
                }
            }

            // Fallback: try to match by values (Priority + TargetState.Id) in case VMs come from different SO contexts
            if (indexToRemove < 0)
            {
                for (var i = 0; i < transitionsProperty.arraySize; i++)
                {
                    var element = transitionsProperty.GetArrayElementAtIndex(i);
                    var vm = new TransitionRuleDefinitionViewModel(element);

                    if (vm.Priority == transitionRuleDefinitionViewModel.Priority)
                    {
                        var targetIdA = vm.TargetState?.Id;
                        var targetIdB = transitionRuleDefinitionViewModel.TargetState?.Id;

                        if (targetIdA == targetIdB)
                        {
                            indexToRemove = i;

                            break;
                        }
                    }
                }
            }

            if (indexToRemove < 0)
            {
                Debug.LogWarning("Transition to remove was not found in the current state's transitions list.");
                so.ApplyModifiedProperties();

                return;
            }

            transitionsProperty.DeleteArrayElementAtIndex(indexToRemove);
            so.ApplyModifiedProperties();

            Notify(nameof(Transitions));
        }

        public void EditTransition(TransitionRuleDefinitionViewModel dataTransitionRuleDefinitionViewModel)
        {
            if (dataTransitionRuleDefinitionViewModel == null)
            {
                Debug.LogError("EditTransition called with null argument");

                return;
            }

            var transitionsProperty =
                SerializedProperty.FindPropertyRelative(StateDefinition.TRANSITION_RULES_PROPERTY_NAME);

            if (transitionsProperty == null)
            {
                Debug.LogError("Transitions property not found on StateDefinition");

                return;
            }

            var so = transitionsProperty.serializedObject;
            Undo.RecordObject(so.targetObject, "Edit transition");
            so.Update();

            // Try to locate the target element by exact property path first
            var targetPath = dataTransitionRuleDefinitionViewModel.SerializedProperty.propertyPath;
            var indexToEdit = -1;

            for (var i = 0; i < transitionsProperty.arraySize; i++)
            {
                var element = transitionsProperty.GetArrayElementAtIndex(i);

                if (element.propertyPath == targetPath)
                {
                    indexToEdit = i;

                    break;
                }
            }

            // Fallback: match by values (Priority + TargetState.Id)
            if (indexToEdit < 0)
            {
                for (var i = 0; i < transitionsProperty.arraySize; i++)
                {
                    var element = transitionsProperty.GetArrayElementAtIndex(i);
                    var vm = new TransitionRuleDefinitionViewModel(element);

                    if (vm.Priority == dataTransitionRuleDefinitionViewModel.Priority)
                    {
                        var targetIdA = vm.TargetState?.Id;
                        var targetIdB = dataTransitionRuleDefinitionViewModel.TargetState?.Id;

                        if (targetIdA == targetIdB)
                        {
                            indexToEdit = i;

                            break;
                        }
                    }
                }
            }

            if (indexToEdit < 0)
            {
                Debug.LogWarning(
                    "Transition to edit was not found in the current state's transitions list. Applying refresh only.");
                so.ApplyModifiedProperties();
                Notify(nameof(Transitions));

                return;
            }

            // Apply the edits to the found element
            var targetElement = transitionsProperty.GetArrayElementAtIndex(indexToEdit);
            var targetVm = new TransitionRuleDefinitionViewModel(targetElement);

            // Update primitive/enum fields
            targetVm.Priority = dataTransitionRuleDefinitionViewModel.Priority;
            targetVm.ConditionFilterType = dataTransitionRuleDefinitionViewModel.ConditionFilterType;

            // Update nested TargetState (copies fields into nested struct/class)
            targetVm.TargetState = dataTransitionRuleDefinitionViewModel.TargetState;

            // Sync conditions list: clear and re-add from source data
            var conditionsProp = targetElement.FindPropertyRelative(
                TransitionRuleDefinition.CONDITION_DEFINITIONS_PROPERTY_NAME);

            if (conditionsProp != null)
            {
                // Clear existing
                for (var i = conditionsProp.arraySize - 1; i >= 0; i--)
                {
                    conditionsProp.DeleteArrayElementAtIndex(i);
                }

                // Re-create from source
                foreach (var conditionVm in dataTransitionRuleDefinitionViewModel.Conditions)
                {
                    conditionsProp.InsertArrayElementAtIndex(conditionsProp.arraySize);
                    var newCondProp = conditionsProp.GetArrayElementAtIndex(conditionsProp.arraySize - 1);
                    var newCondVm = new ConditionDefinitionViewModel(newCondProp);
                    newCondVm.CopyFrom(conditionVm);
                }
            }

            so.ApplyModifiedProperties();
            Notify(nameof(Transitions));
        }

        internal class StateDefinitionWrapper : DefinitionWrapper<StateDefinition>
        {
        }
    }
}