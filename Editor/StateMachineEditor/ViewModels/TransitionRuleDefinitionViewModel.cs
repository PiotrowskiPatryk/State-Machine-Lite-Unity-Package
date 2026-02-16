using System.Collections.Generic;
using System.Linq;
using Dev.Cortez.StateMachines.Core.Condition;
using Dev.Cortez.StateMachines.Core.StateMachineConfiguration.Definition;
using Unity.Properties;
using UnityEditor;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.ViewModels
{
    public class TransitionRuleDefinitionViewModel : ViewModelBase
    {
        private StateDefinitionViewModel _cachedTargetState;
        private bool _targetStateCacheDirty = true;
        private List<ConditionDefinitionViewModel> _cachedConditions;
        private bool _conditionsCacheDirty = true;

        [CreateProperty]
        public bool IsValid => ((TransitionRuleDefinition)SerializedProperty.boxedValue).IsValid();

        [CreateProperty]
        public int Priority
        {
            get => SerializedProperty.FindPropertyRelative(TransitionRuleDefinition.PRIORITY_PROPERTY_NAME).intValue;
            set => ApplyPropertyValueInt(TransitionRuleDefinition.PRIORITY_PROPERTY_NAME, value);
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
        public StateDefinitionViewModel TargetState
        {
            get
            {
                if (!_targetStateCacheDirty)
                {
                    return _cachedTargetState;
                }

                _targetStateCacheDirty = false;
                var targetStateProperty =
                    SerializedProperty.FindPropertyRelative(
                        TransitionRuleDefinition.TARGET_STATE_PROPERTY_NAME);

                _cachedTargetState = new StateDefinitionViewModel(targetStateProperty);

                return _cachedTargetState;
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                _targetStateCacheDirty = true;

                var targetStateProperty = SerializedProperty.FindPropertyRelative(
                    TransitionRuleDefinition.TARGET_STATE_PROPERTY_NAME);

                var so = targetStateProperty.serializedObject;
                so.Update();

                var targetStateViewModel = new StateDefinitionViewModel(targetStateProperty);
                targetStateViewModel.CopyFrom(value);

                so.ApplyModifiedProperties();

                Notify();
            }
        }

        [CreateProperty]
        public List<ConditionDefinitionViewModel> Conditions
        {
            get
            {
                if (!_conditionsCacheDirty)
                {
                    return _cachedConditions;
                }

                _conditionsCacheDirty = false;
                _cachedConditions = new List<ConditionDefinitionViewModel>();
                
                var conditionsProperty =
                    SerializedProperty.FindPropertyRelative(
                        TransitionRuleDefinition.CONDITION_DEFINITIONS_PROPERTY_NAME);

                for (var i = 0; i < conditionsProperty.arraySize; ++i)
                {
                    var condition = conditionsProperty.GetArrayElementAtIndex(i);
                    _cachedConditions.Add(new ConditionDefinitionViewModel(condition));
                }

                return _cachedConditions;
            }
        }

        public override SerializedProperty SerializedProperty { get; }

        public TransitionRuleDefinitionViewModel()
        {
            // NOTE: Call Dispose() when done to clean up the ScriptableObject wrapper
            var definitionWrapper = ScriptableObject.CreateInstance<TransitionRuleDefinitionEditorWrapper>();
            SerializedProperty =
                new SerializedObject(definitionWrapper).FindProperty(TransitionRuleDefinitionEditorWrapper.
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

            _conditionsCacheDirty = true;
            Notify(nameof(Conditions));
        }

        public void EditCondition(ConditionDefinitionViewModel conditionDefinitionViewModel)
        {
            var condition =
                Conditions.FirstOrDefault(condition => condition.Id.Equals(conditionDefinitionViewModel.Id));

            condition?.CopyFrom(conditionDefinitionViewModel);

            _conditionsCacheDirty = true;
            Notify(nameof(Conditions));
        }

        public void RemoveCondition(int index)
        {
            var conditionsProperty =
                SerializedProperty.FindPropertyRelative(TransitionRuleDefinition.CONDITION_DEFINITIONS_PROPERTY_NAME);

            Undo.RecordObject(conditionsProperty.serializedObject.targetObject, "Remove condition");
            var so = conditionsProperty.serializedObject;
            so.Update();

            conditionsProperty.DeleteArrayElementAtIndex(index);
            so.ApplyModifiedProperties();

            _conditionsCacheDirty = true;
            Notify(nameof(Conditions));
        }

        public void CopyFrom(TransitionRuleDefinitionViewModel other)
        {
            Priority = other.Priority;
            ConditionFilterType = other.ConditionFilterType;
            TargetState = other.TargetState;

            foreach (var condition in other.Conditions)
            {
                AddCondition(condition);
            }
        }

    }
}