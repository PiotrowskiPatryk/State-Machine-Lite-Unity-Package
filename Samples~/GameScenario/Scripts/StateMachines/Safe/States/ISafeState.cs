using Dev.Cortez.StateMachines.Core.Interfaces;
using Samples.GameScenario.Scripts.StateMachines.Safe.Context;

namespace Samples.GameScenario.Scripts.StateMachines.Safe.States
{
    /// <summary>
    /// Marker interface for all Safe state machine states.
    /// This allows the state machine to accept any state that implements this interface,
    /// regardless of the specific payload type.
    /// </summary>
    public interface ISafeState : IState<StateContext>
    {
    }
}