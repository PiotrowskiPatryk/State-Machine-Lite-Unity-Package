using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Abstraction;

namespace Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Common.Mocks
{
    /// <summary>
    /// A controllable Trigger implementation for testing purposes.
    /// Extends TriggerBase to test actual base class behavior while allowing configurable responses.
    /// </summary>
    public sealed class MockTrigger : TriggerBase
    {
        /// <summary>
        /// Configures whether TriggerValueAsync should succeed.
        /// </summary>
        public bool TriggerValueShouldSucceed { get; set; } = true;

        /// <summary>
        /// Records all TriggerValueAsync call arguments.
        /// </summary>
        public List<bool> TriggerValueAsyncCalls { get; } = new();

        /// <summary>
        /// Tracks the number of times TriggerValueAsync was called.
        /// </summary>
        public int TriggerValueCallCount { get; private set; }

        /// <summary>
        /// Tracks the number of times DoDisposeAsync was called.
        /// </summary>
        public int DisposeCallCount { get; private set; }

        public MockTrigger(
            string id = "mock-trigger",
            string name = "Mock Trigger",
            string description = "A mock trigger for testing")
            : base(id, name, description)
        {
        }

        public override UniTask<bool> TriggerValueAsync(bool targetValue, CancellationToken cancellationToken)
        {
            TriggerValueCallCount++;
            TriggerValueAsyncCalls.Add(targetValue);

            if (!TriggerValueShouldSucceed)
            {
                return UniTask.FromResult(false);
            }

            IsTriggered = targetValue;
            return UniTask.FromResult(true);
        }

        protected override System.Threading.Tasks.ValueTask DoDisposeAsync()
        {
            DisposeCallCount++;
            return default;
        }

        /// <summary>
        /// Resets all tracking counters and state.
        /// </summary>
        public void Reset()
        {
            IsTriggered = false;
            TriggerValueAsyncCalls.Clear();
            TriggerValueCallCount = 0;
            DisposeCallCount = 0;
            TriggerValueShouldSucceed = true;
        }

        /// <summary>
        /// Directly sets the trigger value for test setup.
        /// </summary>
        public void SetTriggered(bool value)
        {
            IsTriggered = value;
        }
    }
}
