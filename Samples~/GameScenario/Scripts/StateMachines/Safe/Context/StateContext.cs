using System.Collections.Generic;

namespace Samples.GameScenario.Scripts.StateMachines.Safe
{
    public sealed class StateContext
    {
        public SafeController SafeController { get; }
        public IReadOnlyCollection<SafeButtonType> UnlockSafeButtonSequence { get; }

        public StateContext(SafeController safeController, IReadOnlyCollection<SafeButtonType> unlockSafeButtonSequence)
        {
            SafeController = safeController;
            UnlockSafeButtonSequence = unlockSafeButtonSequence;
        }
    }
}