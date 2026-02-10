using Dev.Cortez.StateMachines.Core.Interfaces;

namespace Samples.GameScenario.Scripts.StateMachines.Safe
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