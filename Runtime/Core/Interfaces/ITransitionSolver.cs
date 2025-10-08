using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Data;

namespace Dev.Cortez.StateMachines.Core
{
    public interface ITransitionSolver : IAsyncDisposable
    {
        event Action<TransitionRule> TransitionRuleApplied;

        UniTask<bool> PopulateRulesAsync(Dictionary<IState, IReadOnlyList<TransitionRule>> transitionRules);
        void SetupNewRules(IState state);
    }
}