using System;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Data.Repository;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.ViewModels;
using NUnit.Framework;
using Unity.Properties;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using ItemOptionsMenu = Dev.Cortez.StateMachines.Editor.StateMachineEditor.Views.VisualElements.ItemOptionsMenu;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Views.MainMenu
{
    public class MainMenuView : VisualElement
    {
        private readonly VisualElement _customContentContainer;

        public event Action PressedNewStateMachineButton;
        public event Action PressedNewTriggerButton;
        public event Action<int> PressedEditStateMachineButton;
        public event Action<int> PressedDeleteStateMachineButton;
        public event Action<int> PressedEditTriggerButton;
        public event Action<int> PressedDeleteTriggerButton;

        private MultiColumnListView _stateMachineListView;
        private MultiColumnListView _triggerListView;

        public MainMenuView()
        {
            var visualTreeAsset =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(StateMachineEditorViewRepository.MAIN_MENU_VIEW_PATH);
            visualTreeAsset.CloneTree(this);

            _customContentContainer = this.Q<VisualElement>("CustomContentContainer");
            _customContentContainer.pickingMode = PickingMode.Ignore;

            InitializeEventListeners();
            InitializeStateMachineListView();
            InitializeTriggerListView();

            Assert.NotNull(_stateMachineListView);
            Assert.NotNull(_triggerListView);
        }

        public void DisplayCustomContent(VisualElement visualElement)
        {
            _customContentContainer.pickingMode = PickingMode.Position;
            _customContentContainer.Add(visualElement);
        }

        public void ClearCustomContent()
        {
            _customContentContainer.Clear();
            _customContentContainer.pickingMode = PickingMode.Ignore;
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
            _stateMachineListView.columns[4].bindCell = DoBindStateMachineMenuCell;
            _stateMachineListView.columns[4].unbindCell = DoUnbindStateMachineMenuCell;
        }

        private void DoUnbindStateMachineMenuCell(VisualElement visualElement, int index)
        {
            var itemOptionsMenu = visualElement.Q<ItemOptionsMenu>();

            itemOptionsMenu?.UnsubscribeFromEvents();
        }

        private void DoBindStateMachineMenuCell(VisualElement visualElement, int index)
        {
            var itemOptionsMenu = visualElement.Q<ItemOptionsMenu>();

            // TODO - Handle unsubscribing
            if (itemOptionsMenu != null)
            {
                itemOptionsMenu.SubscribeToEvents(PressedEditStateMachineButton, PressedDeleteStateMachineButton);
            }
            else
            {
                Debug.LogError("Unable to bind menu item.");
            }

            return;

            void PressedDeleteStateMachineButton()
            {
                OnPressedDeleteStateMachineButton(index);
            }

            void PressedEditStateMachineButton()
            {
                OnPressedEditStateMachineButton(index);
            }
        }

        private void InitializeTriggerListView()
        {
            _triggerListView = this.Q<MultiColumnListView>("TriggerListView");
            _triggerListView.columns[0].bindCell = DoBindTriggerIdCell;
            _triggerListView.columns[1].bindCell = DoBindTriggerNameCell;
            _triggerListView.columns[2].bindCell = DoBindTriggerDescriptionCell;
            _triggerListView.columns[3].bindCell = DoBindTriggerTypeCell;
            _triggerListView.columns[4].bindCell = DoBindTriggerMenuCell;
            _stateMachineListView.columns[4].unbindCell = DoUnbindTriggerMenuCell;
        }

        private void DoUnbindTriggerMenuCell(VisualElement visualElement, int index)
        {
            var itemOptionsMenu = visualElement.Q<ItemOptionsMenu>();

            itemOptionsMenu?.UnsubscribeFromEvents();
        }

        private void DoBindTriggerMenuCell(VisualElement visualElement, int index)
        {
            var itemOptionsMenu = visualElement.Q<ItemOptionsMenu>();

            // TODO - Handle unsubscribing
            if (itemOptionsMenu != null)
            {
                itemOptionsMenu.SubscribeToEvents(PressedEditTriggerButton, PressedDeleteTriggerButton);
            }
            else
            {
                Debug.LogError("Unable to bind menu item.");
            }

            Action test = PressedDeleteTriggerButton;

            return;

            void PressedDeleteTriggerButton()
            {
                OnPressedDeleteTriggerButton(index);
            }

            void PressedEditTriggerButton()
            {
                OnPressedEditTriggerButton(index);
            }
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
            BindStateMachineTablePropertyCell(visualElement, nameof(StateMachineDefinitionViewModel.TypeName),
                index);
        }

        private void DoBindStateMachineDescriptionCell(VisualElement visualElement, int index)
        {
            BindStateMachineTablePropertyCell(visualElement,
                nameof(StateMachineDefinitionViewModel.Description), index);
        }

        private void DoBindStateMachineNameCell(VisualElement visualElement, int index)
        {
            BindStateMachineTablePropertyCell(visualElement, nameof(StateMachineDefinitionViewModel.Name),
                index);
        }

        private void DoBindStateMachineIdCell(VisualElement visualElement, int index)
        {
            BindStateMachineTablePropertyCell(visualElement, nameof(StateMachineDefinitionViewModel.Id),
                index);
        }

        private static void BindStateMachineTablePropertyCell(VisualElement visualElement, string propertyName,
            int index)
        {
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

        private void OnPressedEditStateMachineButton(int itemIndex)
        {
            Debug.Log($"Editing state machine at index {itemIndex}");

            PressedEditStateMachineButton?.Invoke(itemIndex);
        }

        private void OnPressedDeleteStateMachineButton(int itemIndex)
        {
            Debug.Log($"Deleting state machine at index {itemIndex}");

            PressedDeleteStateMachineButton?.Invoke(itemIndex);
        }

        private void OnPressedEditTriggerButton(int itemIndex)
        {
            Debug.Log($"Editing trigger at index {itemIndex}");

            PressedEditTriggerButton?.Invoke(itemIndex);
        }

        private void OnPressedDeleteTriggerButton(int itemIndex)
        {
            Debug.Log($"Deleting trigger at index {itemIndex}");

            PressedDeleteTriggerButton?.Invoke(itemIndex);
        }
    }
}