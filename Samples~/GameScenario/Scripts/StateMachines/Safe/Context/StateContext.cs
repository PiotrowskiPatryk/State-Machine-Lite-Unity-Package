using System.Collections.Generic;
using Samples.GameScenario.Scripts.Safe;

namespace Samples.GameScenario.Scripts.StateMachines.Safe.Context
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