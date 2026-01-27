using System;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Forms;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Forms.State;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Forms.StateMachine;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Forms.TransitionRule;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Forms.Trigger;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.ViewModels;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Views.MainMenu;
using Dev.Cortez.StateMachines.Editor.StateMachineEditor.Views.StateMachineMenu;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Core
{
    internal sealed class StateMachineContainerEditor : EditorWindow
    {
        private MainMenuView _mainMenuView;
        private StateMachineContainerViewModelRegistry _stateMachineContainerViewModelRegistry;

        public static void Show(SerializedObject serializedObject)
        {
            if (StateMachineContainerSingleton.instance.TryApply(serializedObject))
            {
                var window = CreateInstance<StateMachineContainerEditor>();
                var working = StateMachineContainerSingleton.instance.WorkingSerializedObject;
                window.Initialize(working);
                window.Show();
            }
            else
            {
                Debug.LogError("Serialized object is not a StateMachineContainer");
            }
        }

        private void OnDestroy()
        {
            DisposeEventListeners();
        }

        private void CreateGUI()
        {
            CreateMainMenuView();
            InitializeEventListeners();

            // Rehydrate from singleton after domain reload or when window is created without Show(serializedObject)
            var working = StateMachineContainerSingleton.instance.WorkingSerializedObject;

            if (working != null)
            {
                Initialize(working);
            }

            // If Initialize was not called (working is null), BindData will no-op if registry is null
            if (_stateMachineContainerViewModelRegistry != null)
            {
                BindData();
            }
        }

        private void CreateMainMenuView()
        {
            var root = rootVisualElement;
            root.style.flexDirection = FlexDirection.Column;
            root.style.flexGrow = 1;
            root.style.minHeight = 0;

            _mainMenuView = new MainMenuView
            {
                style =
                {
                    // TODO - Do it properly
                    flexGrow = 1,
                    flexShrink = 1,
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

        private static bool ShowDialogWindow(string header, string content, string okText, string cancelText)
        {
            return EditorUtility.DisplayDialog(header, content, okText, cancelText);
        }

        private static void ShowDialogWindow(string header, string content, string okText)
        {
            EditorUtility.DisplayDialog(header, content, okText);
        }

        private void Initialize(SerializedObject serializedObject)
        {
            _stateMachineContainerViewModelRegistry = new StateMachineContainerViewModelRegistry(serializedObject);
        }

        private void DisplayForm<TForm, TData>(TData data, Action<TData> addedCallback)
            where TForm : FormBaseWindow<TData>, new() where TData : class
        {
            var form = new TForm();
            var formVisualElement = form.Show(data, OnSubmittedDataInternal, OnCancelledDataInternal);

            _mainMenuView.DisplayForm(formVisualElement);

            return;

            void OnCancelledDataInternal(TData newData)
            {
                _mainMenuView.HideForm();
            }

            void OnSubmittedDataInternal(TData newData)
            {
                addedCallback?.Invoke(newData);
                _mainMenuView.HideForm();
            }
        }

        private void ShowMainMenuView()
        {
            _mainMenuView.ClearCustomContent();
        }

        private void ShowStateMachineMenuView(StateMachineDefinitionViewModel stateMachineDefinitionViewModel)
        {
            _mainMenuView.ClearCustomContent();

            var stateMachineMenuView = new StateMachineMenuView();

            stateMachineMenuView.SaveButtonPressed += newData =>
            {
                _stateMachineContainerViewModelRegistry.
                    StateMachineConfigurationViewModel.UpdateState(newData);

                ShowMainMenuView();
            };

            stateMachineMenuView.ExitButtonPressed += ShowMainMenuView;
            stateMachineMenuView.AddNewStateButtonPressed += OnAddNewStateButtonPressed;
            stateMachineMenuView.EditStateButtonPressed += OnEditStateButtonPressed;
            stateMachineMenuView.DeleteStateButtonPressed += OnDeleteStateButtonPressed;
            stateMachineMenuView.AddTransitionButtonPressed += OnAddTransitionButtonPressed;
            stateMachineMenuView.RemoveTransitionButtonPressed += OnRemoveTransitionButtonPressed;
            stateMachineMenuView.EditTransitionButtonPressed += OnEditTransitionButtonPressed;
            stateMachineMenuView.CreateTransitionBetweenStatesRequested += OnCreateTransitionBetweenStatesRequested;

            stateMachineMenuView.Bind(stateMachineDefinitionViewModel);

            _mainMenuView.DisplayCustomContent(stateMachineMenuView);
        }

        private void OnPressedEditTriggerButton(int index)
        {
            var triggerDefinitionViewModel =
                _stateMachineContainerViewModelRegistry.TriggerConfigurationViewModel.Triggers[index];

            if (triggerDefinitionViewModel != null)
            {
                var copyTriggerDefinitionViewModel = new TriggerDefinitionViewModel(triggerDefinitionViewModel);

                DisplayForm<TriggerForm, TriggerDefinitionViewModel>(copyTriggerDefinitionViewModel,
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

        private void OnEditTransitionButtonPressed(StateMachineDefinitionViewModel stateMachineDefinitionViewModel,
            StateDefinitionViewModel stateDefinitionViewModel,
            TransitionRuleDefinitionViewModel transitionRuleDefinitionViewModel)
        {
            var transitionRuleFormData = new TransitionRuleFormData
            {
                StateMachineDefinitionViewModel = stateMachineDefinitionViewModel,
                TransitionRuleDefinitionViewModel = transitionRuleDefinitionViewModel,
                SourceStateDefinitionViewModel = stateDefinitionViewModel
            };

            DisplayForm<TransitionRuleForm, TransitionRuleFormData>(transitionRuleFormData,
                data => stateDefinitionViewModel.EditTransition(data.TransitionRuleDefinitionViewModel));
        }

        private void OnPressedNewStateMachineButton()
        {
            var stateMachineDefinitionViewModel = new StateMachineDefinitionViewModel();

            DisplayForm<StateMachineForm, StateMachineDefinitionViewModel>(stateMachineDefinitionViewModel,
                _stateMachineContainerViewModelRegistry.StateMachineConfigurationViewModel.AddStateMachine);
        }

        private void OnPressedNewTriggerButton()
        {
            var triggerDefinitionViewModel = new TriggerDefinitionViewModel();

            DisplayForm<TriggerForm, TriggerDefinitionViewModel>(triggerDefinitionViewModel,
                _stateMachineContainerViewModelRegistry.TriggerConfigurationViewModel.AddTrigger);
        }

        private static void OnRemoveTransitionButtonPressed(
            StateMachineDefinitionViewModel stateMachineDefinitionViewModel,
            StateDefinitionViewModel stateDefinitionViewModel,
            TransitionRuleDefinitionViewModel transitionRuleDefinitionViewModel)
        {
            stateDefinitionViewModel.RemoveTransition(transitionRuleDefinitionViewModel);
        }

        private void OnCreateTransitionBetweenStatesRequested(
            StateMachineDefinitionViewModel stateMachineDefinitionViewModel,
            StateDefinitionViewModel sourceState,
            StateDefinitionViewModel targetState)
        {
            var transitionRuleDefinitionViewModel = new TransitionRuleDefinitionViewModel();
            // Prefill target
            transitionRuleDefinitionViewModel.TargetState = targetState;

            var transitionRuleFormData = new TransitionRuleFormData
            {
                StateMachineDefinitionViewModel = stateMachineDefinitionViewModel,
                TransitionRuleDefinitionViewModel = transitionRuleDefinitionViewModel,
                SourceStateDefinitionViewModel = sourceState
            };

            DisplayForm<TransitionRuleForm, TransitionRuleFormData>(transitionRuleFormData,
                data => sourceState.AddTransition(data.TransitionRuleDefinitionViewModel));
        }

        private void OnAddTransitionButtonPressed(StateMachineDefinitionViewModel stateMachineDefinitionViewModel,
            StateDefinitionViewModel stateDefinitionViewModel)
        {
            // Todo - apply initial state
            var transitionRuleDefinitionViewModel =
                new TransitionRuleDefinitionViewModel();

            var transitionRuleFormData = new TransitionRuleFormData
            {
                StateMachineDefinitionViewModel = stateMachineDefinitionViewModel,
                TransitionRuleDefinitionViewModel = transitionRuleDefinitionViewModel,
                SourceStateDefinitionViewModel = stateDefinitionViewModel
            };

            DisplayForm<TransitionRuleForm, TransitionRuleFormData>(transitionRuleFormData,
                data => stateDefinitionViewModel.AddTransition(data.TransitionRuleDefinitionViewModel));
        }

        private static void OnDeleteStateButtonPressed(StateMachineDefinitionViewModel stateMachineDefinitionViewModel,
            int index)
        {
            stateMachineDefinitionViewModel.RemoveStateAtIndex(index);
        }

        private void OnEditStateButtonPressed(StateMachineDefinitionViewModel stateMachineDefinitionViewModel,
            StateDefinitionViewModel stateDefinitionViewModel)
        {
            DisplayForm<StateForm, StateDefinitionViewModel>(stateDefinitionViewModel,
                stateMachineDefinitionViewModel.UpdateState);
        }

        private void OnAddNewStateButtonPressed(StateMachineDefinitionViewModel stateMachineDefinitionViewModel)
        {
            var stateDefinitionViewModel = new StateDefinitionViewModel(stateMachineDefinitionViewModel.TypeName);

            DisplayForm<StateForm, StateDefinitionViewModel>(stateDefinitionViewModel,
                stateMachineDefinitionViewModel.AddState);
        }
    }
}