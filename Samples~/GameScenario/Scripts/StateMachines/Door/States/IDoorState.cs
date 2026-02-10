using Dev.Cortez.StateMachines.Core.Interfaces;
using Samples.GameScenario.Scripts.StateMachines.Door.Context;

namespace Samples.GameScenario.Scripts.StateMachines.Door.States
{
    public interface IDoorState : IState<DoorContext>
    {
    }
}