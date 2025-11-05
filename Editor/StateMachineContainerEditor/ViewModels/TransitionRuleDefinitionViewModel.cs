using System.Collections.Generic;
using Dev.Cortez.StateMachines.Core.Condition;
using Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.Data.Definition;
using Unity.Properties;
using UnityEditor;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.ViewModels
{
    public class TransitionRuleDefinitionViewModel : ViewModelBase
    {
        [CreateProperty]
        public int Priority
        {
            get => SerializedProperty.FindPropertyRelative(TransitionRuleDefinition.PRIORITY_PROPERTY_NAME).intValue;
            set => ApplyPropertyValueInt(TransitionRuleDefinition.PRIORITY_PROPERTY_NAME, value);
        }

        [CreateProperty]
        public string TargetStateId

        {
            get =>
                SerializedProperty.FindPropertyRelative(
                    TransitionRuleDefinition.TARGET_STATE_ID_PROPERTY_NAME).stringValue;
            set =>
                ApplyPropertyValueString(TransitionRuleDefinition.TARGET_STATE_ID_PROPERTY_NAME, value);
        }

        [CreateProperty]
        public ConditionFilterType ConditionFilterType
        {
            get =>
                (ConditionFilterType)SerializedProperty.
                    FindPropertyRelative(TransitionRuleDefinition.CONDITION_FILTER_TYPE_PROPERTY_NAME).enumValueIndex;
            set => ApplyPropertyValueEnum(TransitionRuleDefinition.CONDITION_FILTER_TYPE_PROPERTY_NAME, (int)value);
        }

        [CreateProperty]
        public List<ConditionDefinitionViewModel> Conditions
        {
            get
            {
                var conditions = new List<ConditionDefinitionViewModel>();
                var conditionsProperty =
                    SerializedProperty.FindPropertyRelative(
                        TransitionRuleDefinition.CONDITION_DEFINITIONS_PROPERTY_NAME);

                for (var i = 0; i < conditionsProperty.arraySize; ++i)
                {
                    var condition = conditionsProperty.GetArrayElementAtIndex(i);
                    conditions.Add(new ConditionDefinitionViewModel(condition));
                }

                return conditions;
            }
        }

        [CreateProperty]
        public string InitialStateId
        {
            get =>
                SerializedProperty.FindPropertyRelative(
                    TransitionRuleDefinition.INITIAL_STATE_ID_PROPERTY_NAME).stringValue;
            set =>
                ApplyPropertyValueString(TransitionRuleDefinition.INITIAL_STATE_ID_PROPERTY_NAME, value);
        }

        public override SerializedProperty SerializedProperty { get; }

        public TransitionRuleDefinitionViewModel()
        {
            // TODO - Handle disposal
            var definitionWrapper = ScriptableObject.CreateInstance<TransitionRuleDefinitionWrapper>();
            SerializedProperty =
                new SerializedObject(definitionWrapper).FindProperty(TransitionRuleDefinitionWrapper.
                    DATA_PROPERTY_NAME);
        }

        public TransitionRuleDefinitionViewModel(SerializedProperty serializedProperty)
        {
            SerializedProperty = serializedProperty;
        }

        public void AddCondition(ConditionDefinitionViewModel conditionDefinitionViewModel)
        {
            var conditionsProperty =
                SerializedProperty.FindPropertyRelative(TransitionRuleDefinition.CONDITION_DEFINITIONS_PROPERTY_NAME);

            Undo.RecordObject(conditionsProperty.serializedObject.targetObject, "Create new condition");
            var so = conditionsProperty.serializedObject;
            so.Update();

            conditionsProperty.InsertArrayElementAtIndex(conditionsProperty.arraySize);
            so.ApplyModifiedProperties();

            var newConditionProperty = conditionsProperty.GetArrayElementAtIndex(conditionsProperty.arraySize - 1);
            var newCondition = new ConditionDefinitionViewModel(newConditionProperty);
            newCondition.CopyFrom(conditionDefinitionViewModel);

            so.ApplyModifiedProperties();

            Notify(nameof(Conditions));
        }

        public void CopyFrom(TransitionRuleDefinitionViewModel other)
        {
            InitialStateId = other.InitialStateId;
            TargetStateId = other.TargetStateId;
            Priority = other.Priority;
            ConditionFilterType = other.ConditionFilterType;

            foreach (var condition in other.Conditions)
            {
                AddCondition(condition);
            }
        }

        internal class TransitionRuleDefinitionWrapper : DefinitionWrapper<TransitionRuleDefinition>
        {
        }
    }
}