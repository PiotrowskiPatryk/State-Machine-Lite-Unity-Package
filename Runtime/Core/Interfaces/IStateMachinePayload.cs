using System.Collections.Generic;
using Dev.Cortez.StateMachines.Core.Data;
using JetBrains.Annotations;

namespace Dev.Cortez.StateMachines.Core
{
    /// <summary>
    ///     Represents a payload used to configure and initialize state machines.
    ///     Provides all necessary data including the state machine ID, transition solver,
    ///     initial state, states, and transition rules required for state machine operation.
    /// </summary>
    public interface IStateMachinePayload : IPayload
    {
        /// Represents the unique identifier for a state machine.
        /// This property is used to distinguish between different state machines in scenarios
        /// where multiple instances or types of state machines are utilized.
        /// It is a required property and must not be null or empty.
        [NotNull]
        string StateMachineId { get; }

        /// Represents the property responsible for handling transition logic and rule application
        /// within a state machine. The `TransitionSolver` is an instance of the `ITransitionSolver`
        /// interface, which provides the mechanisms to set up and apply transition rules asynchronously.
        [NotNull]
        ITransitionSolver TransitionSolver { get; }

        /// <summary>
        ///     Gets the initial state of the state machine, which represents the starting point
        ///     or entry state when the state machine is initialized.
        /// </summary>
        /// <remarks>
        ///     The initial state serves as the first active state of the state machine upon setup.
        ///     It is expected to implement the <see cref="IState" /> interface, ensuring it can
        ///     participate in state machine operations and transitions.
        /// </remarks>
        [NotNull]
        IState InitialState { get; }

        /// <summary>
        ///     Represents the collection of states available in a state machine.
        /// </summary>
        /// <remarks>
        ///     Each state in this collection implements the <see cref="IState" /> interface, encapsulating
        ///     relevant behaviors and properties for individual state instances. The collection can be
        ///     accessed to retrieve the states managed by the state machine, enabling operations such as
        ///     iteration, state look-up, and state registration.
        /// </remarks>
        [NotNull]
        List<IState> States { get; }

        /// <summary>
        ///     Gets the collection of transition rules that define valid transitions
        ///     for each state in the state machine. Each key in the dictionary represents
        ///     a state, and its associated value is a <see cref="TransitionRule" /> that specifies
        ///     the conditions and target state for transitioning from the key state.
        /// </summary>
        [NotNull]
        IReadOnlyDictionary<IState, IReadOnlyList<TransitionRule>> TransitionRules { get; }
    }
}