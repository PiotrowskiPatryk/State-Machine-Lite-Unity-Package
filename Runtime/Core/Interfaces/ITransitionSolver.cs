using System;
using System.Collections.Generic;
using Dev.Cortez.StateMachines.Core.Data;

namespace Dev.Cortez.StateMachines.Core
{
    public interface ITransitionSolver : IAsyncDisposable
    {
        event Action<TransitionRule> TransitionRuleApplied;

        void SetupNewRules(IReadOnlyCollection<TransitionRule> transitionRules);
    }
}