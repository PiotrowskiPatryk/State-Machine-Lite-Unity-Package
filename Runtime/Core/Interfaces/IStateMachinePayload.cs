using System.Collections.Generic;
using JetBrains.Annotations;

namespace Dev.Cortez.StateMachines.Core
{
    public interface IStateMachinePayload : IPayload
    {
        [NotNull]
        string StateMachineId { get; }

        [NotNull]
        ITransitionSolver TransitionSolver { get; }

        [NotNull]
        IState InitialState { get; }

        [NotNull]
        IEnumerable<IState> States { get; }
    }
}