using Samples.GameScenario.Scripts.Door;

namespace Samples.GameScenario.Scripts.StateMachines.Door.Context
{
    public sealed class DoorContext
    {
        public DoorController DoorController { get; }

        public DoorContext(DoorController doorController)
        {
            DoorController = doorController;
        }
    }
}