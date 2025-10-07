using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Dev.Cortez.StateMachines.Core.Data
{
    /// <summary>
    ///     Represents the default payload for a state machine, containing essential information for its initialization and
    ///     operation.
    /// </summary>
    [Serializable]
    public class DefaultStateMachinePayload : IStateMachinePayload
    {
        /// <summary>
        ///     Gets the unique identifier for the state machine.
        /// </summary>
        /// <remarks>
        ///     The <c>StateMachineId</c> property provides a unique string-based identifier
        ///     used to distinguish one state machine instance from another.
        ///     This identifier is essential for identification and tracking purposes across
        ///     different state machine instances.
        /// </remarks>
        public string StateMachineId { get; }

        /// <summary>
        ///     Represents the solver responsible for managing and applying transition rules
        ///     within the state machine. It determines how transitions between states are handled
        ///     and provides mechanisms to set up new rules and respond to transitions.
        ///     This property is required by state machine implementations to define transition behavior.
        /// </summary>
        public ITransitionSolver TransitionSolver { get; }

        /// <summary>
        ///     Gets the initial state of the state machine.
        ///     This property represents the entry point or starting state from which
        ///     the state machine begins its execution.
        /// </summary>
        /// <value>
        ///     An object implementing the <see cref="IState" /> interface, which defines
        ///     the behavior and properties of the initial state.
        /// </value>
        /// <remarks>
        ///     The initial state is a critical part of the state machine, serving as
        ///     the default state when the state machine is initialized. It must not be null
        ///     and should be provided during the construction of the payload.
        /// </remarks>
        public IState InitialState { get; }

        /// <summary>
        ///     Represents a collection of states associated with the state machine.
        ///     The states define the possible configurations or modes in which the state machine resides during its lifecycle.
        /// </summary>
        /// <remarks>
        ///     Each state in the collection must implement the <see cref="IState" /> interface.
        ///     The collection is used to facilitate transitions and manage the state machine's behavior.
        /// </remarks>
        public List<IState> States { get; }

        /// <summary>
        ///     Represents the default payload for a state machine.
        ///     It contains the essential configuration and state information
        ///     required to initialize and manage a state machine.
        /// </summary>
        public DefaultStateMachinePayload([NotNull] string stateMachineId,
            [NotNull] ITransitionSolver transitionSolver, [NotNull] IState initialState,
            [NotNull] List<IState> states)
        {
            StateMachineId = stateMachineId;
            TransitionSolver = transitionSolver;
            InitialState = initialState;
            States = states;
        }
    }
}