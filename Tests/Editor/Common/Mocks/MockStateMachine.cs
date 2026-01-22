using System.Threading;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Abstraction;
using Dev.Cortez.StateMachines.Core.Data;

namespace Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Common.Mocks
{
    /// <summary>
    /// A controllable StateMachine implementation for testing purposes.
    /// Exposes internal configuration and tracks lifecycle method calls.
    /// </summary>
    public sealed class MockStateMachine : StateMachineBase<MockState>
    {
        private bool _canActivate = true;

        /// <summary>
        /// Configures whether the state machine can be activated.
        /// </summary>
        public bool ConfigurableCanActivate
        {
            get => _canActivate;
            set => _canActivate = value;
        }

        protected override bool CanActivate => _canActivate;

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

        protected override System.Threading.Tasks.ValueTask DoDisposeAsync()
        {
            DoDisposeCallCount++;
            return default;
        }

        /// <summary>
        /// Resets all tracking counters.
        /// </summary>
        public void ResetTracking()
        {
            _canActivate = true;
            DoActivateShouldSucceed = true;
            DoDeactivateShouldSucceed = true;
            DoActivateCallCount = 0;
            DoDeactivateCallCount = 0;
            DoDisposeCallCount = 0;
        }
    }
}
