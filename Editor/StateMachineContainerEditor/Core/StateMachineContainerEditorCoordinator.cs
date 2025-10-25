using Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.Data;
using Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.Views.MainMenu;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.Core
{
    public class StateMachineContainerEditorCoordinator : EditorWindow
    {
        private MainMenuView _mainMenuView;
        private StateMachineContainerViewModelRegistry _stateMachineContainerViewModelRegistry;

        public static void Show(SerializedObject serializedObject)
        {
            if (serializedObject.targetObject.GetType() != typeof(StateMachineContainer))
            {
                Debug.LogError("Serialized object is not a StateMachineContainer");
            }

            var window = CreateInstance<StateMachineContainerEditorCoordinator>();
            window.Initialize(serializedObject);
            window.Show();
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
        }

        private void Initialize(SerializedObject serializedObject)
        {
            _stateMachineContainerViewModelRegistry = new StateMachineContainerViewModelRegistry(serializedObject);
        }

        private void OnPressedNewStateMachineButton()
        {
            _stateMachineContainerViewModelRegistry.StateMachineConfigurationViewModel.AddStateMachine();
        }

        private void OnPressedNewTriggerButton()
        {
            _stateMachineContainerViewModelRegistry.TriggerConfigurationViewModel.AddTrigger();
        }
    }
}