using System.Linq;
using Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.Forms.Condition;
using Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.ViewModels;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.Forms.TransitionRule
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

        protected override bool IsInputDataValid()
        {
            // TODO implement validation
            return true;
        }

        protected override void OnCreatedGUI()
        {
            _submitButton = rootVisualElement.Q<Button>("SubmitButton");
            _cancelButton = rootVisualElement.Q<Button>("CancelButton");
            _submitButton.clicked += Submit;
            _cancelButton.clicked += Cancel;

            _newConditionButton = rootVisualElement.Q<Button>("NewConditionButton");
            _conditionsListView = rootVisualElement.Q<MultiColumnListView>("ConditionsListView");
            _conditionsListView.columns[0].bindCell = DoBindConditionNameValue;
            _conditionsListView.columns[1].bindCell = DoBindConditionDescriptionValue;
            _conditionsListView.columns[2].bindCell = DoBindConditionTypeValue;
            _conditionsListView.columns[3].bindCell = DoBindConditionRuleValue;
            rootVisualElement.dataSource = Data.TransitionRuleDefinitionViewModel;

            _newConditionButton.clicked += OnNewConditionButtonPressed;

            _initialStateTextField = rootVisualElement.Q<TextField>("InitialStateTextField");
            _initialStateTextField.value = Data.SourceStateDefinitionViewModel.Name;
            _targetStateDropdown = rootVisualElement.Q<DropdownField>("TargetStateDropdownField");

            _targetStateDropdown.RegisterValueChangedCallback(OnTargetStateChanged);

            PopulateDropdown();
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

        private void OnNewConditionButtonPressed()
        {
            var conditionDefinitionViewModel = new ConditionDefinitionViewModel();

            FormBaseWindow<ConditionDefinitionViewModel>.Show<ConditionForm>(conditionDefinitionViewModel,
                OnAddedCondition);
        }

        private void OnAddedCondition(ConditionDefinitionViewModel conditionDefinitionViewModel)
        {
            Data.TransitionRuleDefinitionViewModel.AddCondition(conditionDefinitionViewModel);
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