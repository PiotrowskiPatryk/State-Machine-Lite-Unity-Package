using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Abstraction;
using Dev.Cortez.StateMachines.Core.Attributes;

namespace Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Common.Mocks
{
    /// <summary>
    /// A controllable StateMachine implementation for testing purposes.
    /// Exposes internal configuration and tracks lifecycle method calls.
    /// </summary>
    [ExcludeFromTypePicker]
    public sealed class MockStateMachine : StateMachineBase<MockState>
    {
        /// <summary>
        /// Configures whether the state machine can be activated.
        /// </summary>
        public bool ConfigurableCanActivate { get; set; } = true;

        /// <summary>
        /// Configures whether DoActivateAsync should succeed.
        /// </summary>
        public bool DoActivateShouldSucceed { get; set; } = true;

        /// <summary>
        /// Configures whether DoDeactivateAsync should succeed.
        /// </summary>
        public bool DoDeactivateShouldSucceed { get; set; } = true;

        /// <summary>
        /// Tracks the number of times DoActivateAsync was called.
        /// </summary>
        public int DoActivateCallCount { get; private set; }

        /// <summary>
        /// Tracks the number of times DoDeactivateAsync was called.
        /// </summary>
        public int DoDeactivateCallCount { get; private set; }

        /// <summary>
        /// Tracks the number of times DoDisposeAsync was called.
        /// </summary>
        public int DoDisposeCallCount { get; private set; }

        protected override bool CanActivate => ConfigurableCanActivate;

        /// <summary>
        /// Resets all tracking counters.
        /// </summary>
        public void ResetTracking()
        {
            ConfigurableCanActivate = true;
            DoActivateShouldSucceed = true;
            DoDeactivateShouldSucceed = true;
            DoActivateCallCount = 0;
            DoDeactivateCallCount = 0;
            DoDisposeCallCount = 0;
        }

        protected override UniTask<bool> DoActivateAsync(CancellationToken cancellationToken)
        {
            DoActivateCallCount++;

            return UniTask.FromResult(DoActivateShouldSucceed);
        }

        protected override UniTask<bool> DoDeactivateAsync(CancellationToken cancellationToken)
        {
            DoDeactivateCallCount++;

            return UniTask.FromResult(DoDeactivateShouldSucceed);
        }

        protected override ValueTask DoDisposeAsync()
        {
            DoDisposeCallCount++;

            return default;
        }
    }
}