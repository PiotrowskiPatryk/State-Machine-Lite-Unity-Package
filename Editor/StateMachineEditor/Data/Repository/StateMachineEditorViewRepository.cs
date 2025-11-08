namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Data.Repository
{
    public static class StateMachineEditorViewRepository
    {
        public static string PARENT_PATH = "Packages/dev.cortez.state-machines/Editor/StateMachineEditor/";

        public static string STATE_MACHINE_GRAPH_VIEW_PATH =
            PARENT_PATH + "Views/StateMachineGraphView/StateMachineGraphView.uxml";

        public static string STATE_NODE_TEMPLATE_VIEW_PATH =
            PARENT_PATH + "Views/StateMachineGraphView/Templates/StateNodeTemplate.uxml";

        public static string STATE_MACHINE_MENU_VIEW_PATH =
            PARENT_PATH + "Views/StateMachineMenu/StateMachineMenu.uxml";

        public static string MAIN_MENU_VIEW_PATH = PARENT_PATH + "Views/MainMenu/MainMenuView.uxml";
    }
}