using Samples.GameScenario.Scripts.Key;

namespace Samples.GameScenario.Scripts.StateMachines.Key.Context
{
    public sealed class KeyContext
    {
        public KeyController KeyController { get; }

        public KeyContext(KeyController keyController)
        {
            KeyController = keyController;
        }
    }
}