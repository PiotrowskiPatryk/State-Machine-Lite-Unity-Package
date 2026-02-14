using UnityEngine.UIElements;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Views.StateMachineGraphView
{
    /// <summary>
    ///     Concrete graph node representing a state in the state machine graph.
    ///     Handles state-specific visuals such as initial state highlighting.
    /// </summary>
    [UxmlElement]
    public partial class StateGraphNode : GraphNodeBase
    {
        private static readonly string INITIAL_STATE_CLASS = "initial-state";

        public bool IsInitialState { get; private set; }

        /// <summary>
        ///     Toggles the initial state visual indicator on this node.
        /// </summary>
        public void SetIsInitialState(bool isInitial)
        {
            if (IsInitialState == isInitial)
            {
                return;
            }

            IsInitialState = isInitial;

            if (IsInitialState)
            {
                AddToClassList(INITIAL_STATE_CLASS);
            }
            else
            {
                RemoveFromClassList(INITIAL_STATE_CLASS);
            }
        }
    }
}