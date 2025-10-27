using Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.Data;
using Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.Forms;
using Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.Forms.StateMachineForm;
using Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.Forms.Trigger;
using Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.ViewModels;
using Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.Views.MainMenu;
using Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.Views.StateMachineMenu;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.Core
{
    public class StateMachineContainerEditor : EditorWindow
    {
        private MainMenuView _mainMenuView;
        private StateMachineContainerViewModelRegistry _stateMachineContainerViewModelRegistry;

        public static void Show(SerializedObject serializedObject)
        {
            if (serializedObject.targetObject.GetType() != typeof(StateMachineContainer))
            {
                Debug.LogError("Serialized object is not a StateMachineContainer");
            }

            var window = CreateInstance<StateMachineContainerEditor>();
            window.Initialize(serializedObject);
            window.Show();
        }

        private void OnDestroy()
        {
            DisposeEventListeners();
        }

        private void CreateGUI()
        {
            CreateMainMenuView();
            InitializeEventListeners();
            BindData();
        }

        private void CreateMainMenuView()
        {
            var root = rootVisualElement;
            root.style.flexDirection = FlexDirection.Column;

            _mainMenuView = new MainMenuView
            {
                style =
                {
                    // TODO - Do it properly
                    flexGrow = 1,
                    flexShrink = 0,
                    flexBasis = 0
                }
            };

            root.Add(_mainMenuView);

            ShowMainMenuView();
        }

        private void BindData()
        {
            rootVisualElement.dataSource = _stateMachineContainerViewModelRegistry.StateMachineConfigurationViewModel;
            _mainMenuView.Bind(_stateMachineContainerViewModelRegistry.StateMachineConfigurationViewModel,
                _stateMachineContainerViewModelRegistry.TriggerConfigurationViewModel);
        }

        private void InitializeEventListeners()
        {
            _mainMenuView.PressedNewStateMachineButton += OnPressedNewStateMachineButton;
            _mainMenuView.PressedNewTriggerButton += OnPressedNewTriggerButton;
            _mainMenuView.PressedDeleteStateMachineButton += OnPressedDeleteStateMachineButton;
            _mainMenuView.PressedDeleteTriggerButton += OnPressedDeleteTriggerButton;
            _mainMenuView.PressedEditStateMachineButton += OnPressedEditStateMachineButton;
            _mainMenuView.PressedEditTriggerButton += OnPressedEditTriggerButton;
        }

        private void DisposeEventListeners()
        {
            _mainMenuView.PressedNewStateMachineButton -= OnPressedNewStateMachineButton;
            _mainMenuView.PressedNewTriggerButton -= OnPressedNewTriggerButton;
            _mainMenuView.PressedDeleteStateMachineButton -= OnPressedDeleteStateMachineButton;
            _mainMenuView.PressedDeleteTriggerButton -= OnPressedDeleteTriggerButton;
            _mainMenuView.PressedEditStateMachineButton -= OnPressedEditStateMachineButton;
            _mainMenuView.PressedEditTriggerButton -= OnPressedEditTriggerButton;
        }

        private bool ShowDialogWindow(string header, string content, string okText, string cancelText)
        {
            return EditorUtility.DisplayDialog(header, content, okText, cancelText);
        }

        private bool ShowDialogWindow(string header, string content, string okText)
        {
            return EditorUtility.DisplayDialog(header, content, okText);
        }

        private void Initialize(SerializedObject serializedObject)
        {
            _stateMachineContainerViewModelRegistry = new StateMachineContainerViewModelRegistry(serializedObject);
        }

        private void OnPressedNewStateMachineButton()
        {
            var stateMachineDefinitionViewModel = new StateMachineDefinitionViewModel();

            FormBaseWindow<StateMachineDefinitionViewModel>.Show<StateMachineForm>(
                stateMachineDefinitionViewModel,
                _stateMachineContainerViewModelRegistry.StateMachineConfigurationViewModel.AddStateMachine);
        }

        private void OnPressedNewTriggerButton()
        {
            var triggerDefinitionViewModel = new TriggerDefinitionViewModel();

            FormBaseWindow<TriggerDefinitionViewModel>.Show<TriggerForm>(
                triggerDefinitionViewModel,
                _stateMachineContainerViewModelRegistry.TriggerConfigurationViewModel.AddTrigger);
        }

        private void OnPressedEditTriggerButton(int index)
        {
            var triggerDefinitionViewModel =
                _stateMachineContainerViewModelRegistry.TriggerConfigurationViewModel.Triggers[index];

            if (triggerDefinitionViewModel != null)
            {
                var copyTriggerDefinitionViewModel = new TriggerDefinitionViewModel(triggerDefinitionViewModel);
                FormBaseWindow<TriggerDefinitionViewModel>.Show<TriggerForm>(
                    copyTriggerDefinitionViewModel,
                    _stateMachineContainerViewModelRegistry.TriggerConfigurationViewModel.UpdateTrigger);
            }
        }

        private void OnPressedDeleteTriggerButton(int index)
        {
            var triggerName = _stateMachineContainerViewModelRegistry.TriggerConfigurationViewModel.
                GetTriggerByIndex(index)?.Name;
            var confirmedAction = ShowDialogWindow("Delete trigger",
                $"Are you sure you want to delete this trigger {triggerName}?", "Yes", "No");

            if (confirmedAction)
            {
                _stateMachineContainerViewModelRegistry.TriggerConfigurationViewModel.RemoveTriggerAtIndex(index);
                ShowDialogWindow("Deleted trigger", "Trigger deleted", "Ok");
            }
        }

        private void OnPressedEditStateMachineButton(int index)
        {
            var stateMachineViewModel =
                _stateMachineContainerViewModelRegistry.StateMachineConfigurationViewModel.StateMachines[index];

            ShowStateMachineMenuView(stateMachineViewModel);
        }

        private void OnPressedDeleteStateMachineButton(int index)
        {
            var stateMachineName = _stateMachineContainerViewModelRegistry.StateMachineConfigurationViewModel.
                GetStateMachineByIndex(index).Name;
            var confirmedAction = ShowDialogWindow("Delete state machine",
                $"Are you sure you want to delete this state machine {stateMachineName}?", "Yes", "No");

            if (confirmedAction)
            {
                _stateMachineContainerViewModelRegistry.StateMachineConfigurationViewModel.
                    RemoveStateMachineAtIndex(index);
                ShowDialogWindow("State machine deleted", "State machine deleted", "Ok");
            }
        }

        private void ShowStateMachineMenuView(StateMachineDefinitionViewModel stateMachineDefinitionViewModel)
        {
            _mainMenuView.ClearCustomContent();

            var stateMachineMenuView = new StateMachineMenuView
            {
                style =
                {
                    // TODO - Do it properly
                    flexGrow = 1,
                    flexShrink = 0,
                    flexBasis = 0
                }
            };

            stateMachineMenuView.SaveButtonPressed += newData =>
            {
                _stateMachineContainerViewModelRegistry.
                    StateMachineConfigurationViewModel.UpdateState(newData);

                ShowMainMenuView();
            };

            stateMachineMenuView.ExitButtonPressed += ShowMainMenuView;

            stateMachineMenuView.Bind(stateMachineDefinitionViewModel);

            _mainMenuView.DisplayCustomContent(stateMachineMenuView);
        }

        private void ShowMainMenuView()
        {
            _mainMenuView.ClearCustomContent();
        }
    }
}