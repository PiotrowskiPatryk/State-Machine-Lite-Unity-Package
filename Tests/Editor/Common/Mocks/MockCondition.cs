using System.Threading;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Abstraction;
using Dev.Cortez.StateMachines.Core.Data;

namespace Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Common.Mocks
{
    /// <summary>
    /// A controllable Condition implementation for testing purposes.
    /// Extends ConditionBase to test actual base class behavior while allowing configurable responses.
    /// </summary>
    public sealed class MockCondition : ConditionBase
    {
        private bool _isSatisfied;

        /// <summary>
        /// Gets or sets whether the condition is satisfied.
        /// Use SetSatisfied() to change the value and fire events.
        /// </summary>
        public override bool IsSatisfied => _isSatisfied;

        /// <summary>
        /// Tracks the number of times SetSatisfied was called.
        /// </summary>
        public int SetSatisfiedCallCount { get; private set; }

        /// <summary>
        /// Sets the IsSatisfied value and publishes the SatisfiedChanged event.
        /// </summary>
        public void SetSatisfied(bool value)
        {
            SetSatisfiedCallCount++;
            
            if (_isSatisfied == value)
            {
                return;
            }

            _isSatisfied = value;
            PublishSatisfiedChangedEvent(value);
        }

        /// <summary>
        /// Forces event firing regardless of current state (for edge case testing).
        /// </summary>
        public void ForceSatisfiedChangedEvent()
        {
            PublishSatisfiedChangedEvent(_isSatisfied);
        }

        /// <summary>
        /// Resets all tracking counters and state.
        /// </summary>
        public void Reset()
        {
            _isSatisfied = false;
            SetSatisfiedCallCount = 0;
        }
    }
}
