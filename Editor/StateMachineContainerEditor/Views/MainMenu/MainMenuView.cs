using System;
using Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.ViewModels;
using NUnit.Framework;
using Unity.Properties;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.Views.MainMenu
{
    public class MainMenuView : VisualElement
    {
        private const string UxmlPath =
            "Packages/dev.cortez.state-machines/Editor/StateMachineContainerEditor/Views/MainMenu/MainMenuView.uxml";

        public event Action PressedNewStateMachineButton;
        public event Action PressedNewTriggerButton;

        private MultiColumnListView _stateMachineListView;
        private MultiColumnListView _triggerListView;

        public MainMenuView()
        {
            var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UxmlPath);
            visualTreeAsset.CloneTree(this);

            InitializeEventListeners();
            InitializeStateMachineListView();
            InitializeTriggerListView();

            Assert.NotNull(_stateMachineListView);
            Assert.NotNull(_triggerListView);
        }

        public void Bind(StateMachineConfigurationViewModel stateMachineConfigurationViewModel,
            TriggerConfigurationViewModel triggerConfigurationViewModel)
        {
            _stateMachineListView.dataSource = stateMachineConfigurationViewModel;
            _triggerListView.dataSource = triggerConfigurationViewModel;

            this.Q<Label>("StateMachinesCountValue").dataSource = stateMachineConfigurationViewModel;
            this.Q<Label>("TriggersCountValue").dataSource = triggerConfigurationViewModel;
        }

        private void InitializeEventListeners()
        {
            this.Q<Button>("NewStateMachineButton").clicked += OnNewStateMachineButtonPressed;
            this.Q<Button>("NewTriggerButton").clicked += OnNewTriggerButtonPressed;
        }

        private void InitializeStateMachineListView()
        {
            _stateMachineListView = this.Q<MultiColumnListView>("StateMachineListView");
            _stateMachineListView.columns[0].bindCell = DoBindStateMachineIdCell;
            _stateMachineListView.columns[1].bindCell = DoBindStateMachineNameCell;
            _stateMachineListView.columns[2].bindCell = DoBindStateMachineDescriptionCell;
            _stateMachineListView.columns[3].bindCell = DoBindStateMachineTypeCell;
        }

        private void InitializeTriggerListView()
        {
            _triggerListView = this.Q<MultiColumnListView>("TriggerListView");
            _triggerListView.columns[0].bindCell = DoBindTriggerIdCell;
            _triggerListView.columns[1].bindCell = DoBindTriggerNameCell;
            _triggerListView.columns[2].bindCell = DoBindTriggerDescriptionCell;
            _triggerListView.columns[3].bindCell = DoBindTriggerTypeCell;
        }

        private void DoBindTriggerTypeCell(VisualElement visualElement, int index)
        {
            BindTriggerTablePropertyCell(visualElement, nameof(TriggerDefinitionViewModel.TypeName), index);
        }

        private void DoBindTriggerDescriptionCell(VisualElement visualElement, int index)
        {
            BindTriggerTablePropertyCell(visualElement, nameof(TriggerDefinitionViewModel.Description), index);
        }

        private void DoBindTriggerNameCell(VisualElement visualElement, int index)
        {
            BindTriggerTablePropertyCell(visualElement, nameof(TriggerDefinitionViewModel.Name), index);
        }

        private void DoBindTriggerIdCell(VisualElement visualElement, int index)
        {
            BindTriggerTablePropertyCell(visualElement, nameof(TriggerDefinitionViewModel.Id), index);
        }

        private void DoBindStateMachineTypeCell(VisualElement visualElement, int index)
        {
            BindStateMachineTablePropertyCell(visualElement, nameof(StateMachineDefinitionViewModel.StateMachineType),
                index);
        }

        private void DoBindStateMachineDescriptionCell(VisualElement visualElement, int index)
        {
            BindStateMachineTablePropertyCell(visualElement,
                nameof(StateMachineDefinitionViewModel.StateMachineDescription), index);
        }

        private void DoBindStateMachineNameCell(VisualElement visualElement, int index)
        {
            BindStateMachineTablePropertyCell(visualElement, nameof(StateMachineDefinitionViewModel.StateMachineName),
                index);
        }

        private void DoBindStateMachineIdCell(VisualElement visualElement, int index)
        {
            BindStateMachineTablePropertyCell(visualElement, nameof(StateMachineDefinitionViewModel.StateMachineId),
                index);
        }

        private static void BindStateMachineTablePropertyCell(VisualElement visualElement, string propertyName,
            int index)
        {
            Debug.Log("Binding");

            var label = visualElement.Q<Label>();
            label.SetBinding("value", new DataBinding
            {
                dataSourcePath =
                    new PropertyPath(
                        $"{nameof(StateMachineConfigurationViewModel.StateMachines)}[{index}].{propertyName}"),
                bindingMode = BindingMode.ToTarget
            });
        }

        private static void BindTriggerTablePropertyCell(VisualElement visualElement, string propertyName,
            int index)
        {
            Debug.Log("Binding");

            var label = visualElement.Q<Label>();
            label.SetBinding("value", new DataBinding
            {
                dataSourcePath =
                    new PropertyPath(
                        $"{nameof(TriggerConfigurationViewModel.Triggers)}[{index}].{propertyName}"),
                bindingMode = BindingMode.ToTarget
            });
        }

        private void OnNewTriggerButtonPressed()
        {
            PressedNewTriggerButton?.Invoke();
        }

        private void OnNewStateMachineButtonPressed()
        {
            PressedNewStateMachineButton?.Invoke();
        }
    }
}