using System;
using System.Linq;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Data.Repository;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Forms.Condition;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.ViewModels;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Views.VisualElements;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Forms.TransitionRule
{
    public class TransitionRuleFormData
    {
        public TransitionRuleDefinitionViewModel TransitionRuleDefinitionViewModel { get; set; }
        public StateMachineDefinitionViewModel StateMachineDefinitionViewModel { get; set; }
        public StateDefinitionViewModel SourceStateDefinitionViewModel { get; set; }
    }

    public class TransitionRuleForm : FormBaseWindow<TransitionRuleFormData>
    {
        private Button _submitButton;
        private Button _newConditionButton;
        private Button _cancelButton;
        private TextField _initialStateTextField;
        private DropdownField _targetStateDropdown;
        private MultiColumnListView _conditionsListView;
        private VisualElement _conditionFormContainer;

        protected override string FORM_PATH => StateMachineEditorViewRepository.TRANSITION_RULE_FORM_PATH;

        public void ShowConditionForm(VisualElement visualElement)
        {
            _conditionFormContainer.style.display = DisplayStyle.Flex;
            _conditionFormContainer.visible = true;
            _conditionFormContainer.Add(visualElement);
        }

        protected override bool IsInputDataValid()
        {
            // TODO implement validation
            return true;
        }

        protected override void OnShown()
        {
            _submitButton = RootVisualElement.Q<Button>("SubmitButton");
            _cancelButton = RootVisualElement.Q<Button>("CancelButton");
            _submitButton.clicked += Submit;
            _cancelButton.clicked += Cancel;

            _newConditionButton = RootVisualElement.Q<Button>("NewConditionButton");
            _conditionsListView = RootVisualElement.Q<MultiColumnListView>("ConditionsListView");
            _conditionsListView.columns[0].bindCell = DoBindConditionNameValue;
            _conditionsListView.columns[1].bindCell = DoBindConditionDescriptionValue;
            _conditionsListView.columns[2].bindCell = DoBindConditionTypeValue;
            _conditionsListView.columns[3].bindCell = DoBindConditionRuleValue;
            _conditionsListView.columns[4].bindCell = DoBindConditionMenuCell;
            _conditionsListView.columns[4].unbindCell = DoUnbindConditionMenuCell;
            RootVisualElement.dataSource = Data.TransitionRuleDefinitionViewModel;

            _newConditionButton.clicked += OnNewConditionButtonPressed;

            _initialStateTextField = RootVisualElement.Q<TextField>("InitialStateTextField");
            _initialStateTextField.value = Data.SourceStateDefinitionViewModel.Name;
            _targetStateDropdown = RootVisualElement.Q<DropdownField>("TargetStateDropdownField");
            _conditionFormContainer = RootVisualElement.Q<VisualElement>("ConditionFormContainer");

            _targetStateDropdown.RegisterValueChangedCallback(OnTargetStateChanged);

            PopulateDropdown();
            ClearConditionForm();
        }

        private void DoBindConditionMenuCell(VisualElement visualElement, int index)
        {
            var itemOptionsMenu = visualElement.Q<ItemOptionsMenu>();

            if (itemOptionsMenu != null)
            {
                itemOptionsMenu.SubscribeToEvents(PressedEditStateButton, PressedDeleteStateButton);
            }
            else
            {
                Debug.LogError("Unable to bind menu item.");
            }

            return;

            void PressedDeleteStateButton()
            {
                Data.TransitionRuleDefinitionViewModel.RemoveCondition(index);
            }

            void PressedEditStateButton()
            {
                ShowEditConditionForm(index);
            }
        }

        private void DoUnbindConditionMenuCell(VisualElement visualElement, int index)
        {
            // TODO - CLEANUP THIS, apply DRY principle

            // Unregister Remove button
            var removeItemButton = visualElement.Q<Button>("RemoveButton");

            if (removeItemButton != null && removeItemButton.userData is Action prevRemove)
            {
                removeItemButton.clicked -= prevRemove;
                removeItemButton.userData = null;
            }

            // Unregister Edit button
            var editItemButton = visualElement.Q<Button>("EditButton");

            if (editItemButton is { userData: Action prevEdit })
            {
                editItemButton.clicked -= prevEdit;
                editItemButton.userData = null;
            }

            // Unregister row click highlight handler
            if (visualElement is { userData: EventCallback<PointerUpEvent> prevRow })
            {
                visualElement.UnregisterCallback(prevRow);
                visualElement.userData = null;
            }
        }

        private void DoBindConditionDescriptionValue(VisualElement visualElement, int index)
        {
            var condition = Data.TransitionRuleDefinitionViewModel.Conditions[index];

            visualElement.Q<Label>().text = condition.Description;
        }

        private void DoBindConditionNameValue(VisualElement visualElement, int index)
        {
            var condition = Data.TransitionRuleDefinitionViewModel.Conditions[index];

            visualElement.Q<Label>().text = condition.Name;
        }

        private void DoBindConditionTypeValue(VisualElement visualElement, int index)
        {
            var condition = Data.TransitionRuleDefinitionViewModel.Conditions[index];

            visualElement.Q<Label>().text = condition.TypeNameShort;
        }

        private void DoBindConditionRuleValue(VisualElement visualElement, int index)
        {
            var condition = Data.TransitionRuleDefinitionViewModel.Conditions[index];

            var propertyField = new PropertyField();
            propertyField.BindProperty(condition.Payload);
            propertyField.label = "When";
            propertyField.SetEnabled(false);

            visualElement.Add(propertyField);
        }

        private void PopulateDropdown()
        {
            _targetStateDropdown.choices.Clear();

            foreach (var state in Data.StateMachineDefinitionViewModel.States)
            {
                _targetStateDropdown.choices.Add(state.Name);
            }

            if (Data.TransitionRuleDefinitionViewModel.TargetState != null)
            {
                _targetStateDropdown.value = Data.TransitionRuleDefinitionViewModel.TargetState.Name;
            }
        }

        private void ShowEditConditionForm(int index)
        {
            var condition = Data.TransitionRuleDefinitionViewModel.Conditions[index];

            ShowConditionForm(condition, OnEditedCondition);
        }

        private void OnNewConditionButtonPressed()
        {
            var conditionDefinitionViewModel = new ConditionDefinitionViewModel();
            ShowConditionForm(conditionDefinitionViewModel, OnAddedCondition);
        }

        private void ShowConditionForm(ConditionDefinitionViewModel conditionDefinitionViewModel,
            Action<ConditionDefinitionViewModel> onSubmit)
        {
            var conditionForm = new ConditionForm();
            var conditionFormVisualElement =
                conditionForm.Show(conditionDefinitionViewModel, onSubmit, OnCancelledDataInternal);
            ShowConditionForm(conditionFormVisualElement);

            return;

            void OnCancelledDataInternal(ConditionDefinitionViewModel newData)
            {
                ClearConditionForm();
            }
        }

        private void ClearConditionForm()
        {
            _conditionFormContainer.Clear();
            _conditionFormContainer.style.display = DisplayStyle.None;
            _conditionFormContainer.visible = false;
        }

        private void OnAddedCondition(ConditionDefinitionViewModel conditionDefinitionViewModel)
        {
            ClearConditionForm();

            Data.TransitionRuleDefinitionViewModel.AddCondition(conditionDefinitionViewModel);
        }

        private void OnEditedCondition(ConditionDefinitionViewModel conditionDefinitionViewModel)
        {
            ClearConditionForm();

            Data.TransitionRuleDefinitionViewModel.EditCondition(conditionDefinitionViewModel);
        }

        private void OnTargetStateChanged(ChangeEvent<string> targetStateName)
        {
            var targetState =
                Data.StateMachineDefinitionViewModel.States.FirstOrDefault(state =>
                    state.Name.Equals(targetStateName.newValue));

            Data.TransitionRuleDefinitionViewModel.TargetState = targetState;
        }
    }
}