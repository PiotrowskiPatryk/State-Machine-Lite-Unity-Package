using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Abstraction;
using Dev.Cortez.StateMachines.Core.Attributes;
using Dev.Cortez.StateMachines.Core.Data;

namespace Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Common.Mocks
{
    /// <summary>
    /// A controllable State implementation for testing purposes.
    /// Extends StateBase to test actual base class behavior while allowing configurable responses.
    /// </summary>
    [ExcludeFromTypePicker]
    public sealed class MockState : StateBase
    {
        /// <summary>
        /// Configures whether DoInitializeAsync should succeed.
        /// </summary>
        public bool InitializationShouldSucceed { get; set; } = true;

        /// <summary>
        /// Configures whether DoEnterAsync should succeed.
        /// </summary>
        public bool EnterShouldSucceed { get; set; } = true;

        /// <summary>
        /// Configures whether DoExitAsync should succeed.
        /// </summary>
        public bool ExitShouldSucceed { get; set; } = true;

        /// <summary>
        /// Tracks the number of times DoEnterAsync was called.
        /// </summary>
        public int EnterCallCount { get; private set; }

        /// <summary>
        /// Tracks the number of times DoExitAsync was called.
        /// </summary>
        public int ExitCallCount { get; private set; }

        /// <summary>
        /// Tracks the number of times DoInitializeAsync was called.
        /// </summary>
        public int InitializeCallCount { get; private set; }

        /// <summary>
        /// Tracks the number of times DoDisposeAsync was called.
        /// </summary>
        public int DisposeCallCount { get; private set; }

        /// <summary>
        /// Records the last context passed to DoEnterAsync.
        /// </summary>
        public EmptyContext LastEnterContext { get; private set; }

        /// <summary>
        /// Records the last context passed to DoExitAsync.
        /// </summary>
        public EmptyContext LastExitContext { get; private set; }

        /// <summary>
        /// Optional delay for DoEnterAsync to simulate async work (in milliseconds).
        /// </summary>
        public int EnterDelayMs { get; set; }

        /// <summary>
        /// Optional delay for DoExitAsync to simulate async work (in milliseconds).
        /// </summary>
        public int ExitDelayMs { get; set; }

        /// <summary>
        /// Resets all tracking counters and configuration.
        /// </summary>
        public void Reset()
        {
            EnterCallCount = 0;
            ExitCallCount = 0;
            InitializeCallCount = 0;
            DisposeCallCount = 0;
            LastEnterContext = null;
            LastExitContext = null;
            InitializationShouldSucceed = true;
            EnterShouldSucceed = true;
            ExitShouldSucceed = true;
            EnterDelayMs = 0;
            ExitDelayMs = 0;
        }

        protected override async UniTask DoEnterAsync(EmptyContext context, CancellationToken cancellationToken)
        {
            EnterCallCount++;
            LastEnterContext = context;

            if (EnterDelayMs > 0)
            {
                await UniTask.Delay(EnterDelayMs, cancellationToken: cancellationToken);
            }
        }

        protected override async UniTask DoExitAsync(EmptyContext context, CancellationToken cancellationToken)
        {
            ExitCallCount++;
            LastExitContext = context;

            if (ExitDelayMs > 0)
            {
                await UniTask.Delay(ExitDelayMs, cancellationToken: cancellationToken);
            }
        }

        protected override UniTask<bool> DoInitializeAsync(EmptyPayload uiStatePayload,
            CancellationToken cancellationToken)
        {
            InitializeCallCount++;

            return UniTask.FromResult(InitializationShouldSucceed);
        }

        protected override ValueTask DoDisposeAsync()
        {
            DisposeCallCount++;

            return default;
        }
    }
}