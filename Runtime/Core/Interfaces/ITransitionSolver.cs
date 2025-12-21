using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Data;
using JetBrains.Annotations;

namespace Dev.Cortez.StateMachines.Core.Interfaces
{
    /// <summary>
    /// Defines the contract for a transition solver responsible for managing and applying transition rules
    /// within a state machine.
    /// </summary>
    public interface ITransitionSolver : IAsyncDisposable
    {
        /// <summary>
        /// Occurs when a transition rule has been successfully applied, indicating a state change.
        /// </summary>
        event Action<TransitionRule> TransitionRuleApplied;

        /// <summary>
        /// Asynchronously applies and populates the transition rules for the state machine.
        /// This method typically sets up the initial or updated set of rules the solver will use.
        /// </summary>
        /// <param name="transitionRules">A dictionary mapping states to their associated read-only list of transition rules.</param>
        /// <returns>A <see cref="UniTask{TResult}" /> indicating whether the rules were successfully applied and populated.</returns>
        UniTask<bool> ApplyPopulateTransitionRulesAsync([NotNull] Dictionary<IState, IReadOnlyList<TransitionRule>> transitionRules);

        /// <summary>
        /// Applies or activates the relevant transition rules for a active state.
        /// This method is typically called when the state machine switches to a new state,
        /// ensuring the solver is aware of and can enforce the rules pertinent to the current active state.
        /// </summary>
        /// <param name="state">The state that has just become active.</param>
        void ApplyRulesForActiveState([NotNull] IState state);
    }
}
