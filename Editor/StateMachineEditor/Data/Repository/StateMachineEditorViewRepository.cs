namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Data.Repository
{
    public static class StateMachineEditorViewRepository
    {
        public const string PARENT_PATH = "Packages/dev.cortez.state-machines/Editor/StateMachineEditor/";

        public const string REFERENCE_PICKER_PROPERTY_DRAWER_PATH =
            PARENT_PATH + "Views/ReferencePicker/ReferencePickerView.uxml";

        public static string STATE_MACHINE_GRAPH_VIEW_PATH =>
            PARENT_PATH + "Views/StateMachineGraphView/StateMachineGraphView.uxml";

        public static string STATE_NODE_TEMPLATE_VIEW_PATH =>
            PARENT_PATH + "Views/StateMachineGraphView/Templates/StateNodeTemplate.uxml";

        public static string STATE_MACHINE_MENU_VIEW_PATH =>
            PARENT_PATH + "Views/StateMachineMenu/StateMachineMenu.uxml";

        public static string MAIN_MENU_VIEW_PATH => PARENT_PATH + "Views/MainMenu/MainMenuView.uxml";
        public static string TRIGGER_FORM_PATH => PARENT_PATH + "Forms/Trigger/TriggerForm.uxml";
        public static string STATE_FORM_PATH => PARENT_PATH + "Forms/State/StateForm.uxml";
        public static string CONDITION_FORM_PATH => PARENT_PATH + "Forms/Condition/ConditionForm.uxml";
        public static string TRANSITION_RULE_FORM_PATH => PARENT_PATH + "Forms/TransitionRule/TransitionRuleForm.uxml";
        public static string STATE_MACHINE_FORM_PATH => PARENT_PATH + "Forms/StateMachine/StateMachineForm.uxml";
    }
}