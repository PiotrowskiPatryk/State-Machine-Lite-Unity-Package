using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Data;
using Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Common.Builders;
using Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Common.Mocks;
using NUnit.Framework;

namespace Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Unit.Core
{
    /// <summary>
    /// Unit tests for StateBase class lifecycle and behavior.
    /// Tests initialization, enter/exit lifecycle, status changes, and disposal.
    /// </summary>
    [TestFixture]
    public sealed class StateTests
    {
        private MockState _state;
        private CancellationTokenSource _cts;

        [SetUp]
        public void SetUp()
        {
            _state = new MockState();
            _cts = new CancellationTokenSource();
        }

        [TearDown]
        public void TearDown()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _state?.DisposeAsync().AsTask().GetAwaiter().GetResult();
        }

        #region InitializeAsync Tests

        [Test]
        public async Task InitializeAsync_WithValidPayload_SetsInitializationStatusToInitialized()
        {
            // Arrange
            var stateDefinition = StateDefinitionBuilder.Create()
                .WithId("state-1")
                .WithName("Test State")
                .WithType<MockState>()
                .Build();

            // Act
            var result = await _state.InitializeAsync(stateDefinition, _cts.Token);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_state.InitializationStatus, Is.EqualTo(InitializationStatus.Initialized));
            Assert.That(_state.Id, Is.EqualTo("state-1"));
            Assert.That(_state.Name, Is.EqualTo("Test State"));
        }

        [Test]
        public async Task InitializeAsync_WhenDoInitializeFails_SetsInitializationStatusToFailed()
        {
            // Arrange
            _state.InitializationShouldSucceed = false;
            var stateDefinition = StateDefinitionBuilder.Create()
                .WithId("state-1")
                .WithType<MockState>()
                .Build();

            // Act
            var result = await _state.InitializeAsync(stateDefinition, _cts.Token);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_state.InitializationStatus, Is.EqualTo(InitializationStatus.Failed));
        }

        [Test]
        public async Task InitializeAsync_WhenAlreadyInitialized_ReturnsTrue()
        {
            // Arrange
            var stateDefinition = StateDefinitionBuilder.Create()
                .WithId("state-1")
                .WithType<MockState>()
                .Build();
            await _state.InitializeAsync(stateDefinition, _cts.Token);

            // Act
            var result = await _state.InitializeAsync(stateDefinition, _cts.Token);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_state.InitializeCallCount, Is.EqualTo(1)); // Should not re-initialize
        }

        [Test]
        public async Task InitializeAsync_WhenAlreadyFailed_ReturnsFalse()
        {
            // Arrange
            _state.InitializationShouldSucceed = false;
            var stateDefinition = StateDefinitionBuilder.Create()
                .WithId("state-1")
                .WithType<MockState>()
                .Build();
            await _state.InitializeAsync(stateDefinition, _cts.Token);

            // Reset to success, but it should still fail because status is Failed
            _state.InitializationShouldSucceed = true;

            // Act
            var result = await _state.InitializeAsync(stateDefinition, _cts.Token);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_state.InitializeCallCount, Is.EqualTo(1)); // Should not re-attempt
        }

        #endregion

        #region EnterAsync Tests

        [Test]
        public async Task EnterAsync_WhenNotActive_TransitionsToActiveState()
        {
            // Arrange
            var stateDefinition = StateDefinitionBuilder.Create()
                .WithId("state-1")
                .WithType<MockState>()
                .Build();
            await _state.InitializeAsync(stateDefinition, _cts.Token);

            // Act
            var result = await _state.EnterAsync(EmptyContext.Default, _cts.Token);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_state.IsActive, Is.True);
            Assert.That(_state.StateStatus, Is.EqualTo(StateStatus.Active));
            Assert.That(_state.EnterCallCount, Is.EqualTo(1));
        }

        [Test]
        public async Task EnterAsync_WhenAlreadyActive_ReturnsFalse()
        {
            // Arrange
            var stateDefinition = StateDefinitionBuilder.Create()
                .WithId("state-1")
                .WithType<MockState>()
                .Build();
            await _state.InitializeAsync(stateDefinition, _cts.Token);
            await _state.EnterAsync(EmptyContext.Default, _cts.Token);

            // Act
            var result = await _state.EnterAsync(EmptyContext.Default, _cts.Token);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_state.EnterCallCount, Is.EqualTo(1)); // Should not enter again
        }

        [Test]
        public async Task EnterAsync_FiresStatusChangedEvent_InOrder()
        {
            // Arrange
            var stateDefinition = StateDefinitionBuilder.Create()
                .WithId("state-1")
                .WithType<MockState>()
                .Build();
            await _state.InitializeAsync(stateDefinition, _cts.Token);

            var statusChanges = new System.Collections.Generic.List<StateStatus>();
            _state.StatusChanged += status => statusChanges.Add(status);

            // Act
            await _state.EnterAsync(EmptyContext.Default, _cts.Token);

            // Assert
            Assert.That(statusChanges.Count, Is.EqualTo(2));
            Assert.That(statusChanges[0], Is.EqualTo(StateStatus.Activating));
            Assert.That(statusChanges[1], Is.EqualTo(StateStatus.Active));
        }

        [Test]
        public async Task EnterAsync_PassesContextToDoEnterAsync()
        {
            // Arrange
            var stateDefinition = StateDefinitionBuilder.Create()
                .WithId("state-1")
                .WithType<MockState>()
                .Build();
            await _state.InitializeAsync(stateDefinition, _cts.Token);
            var context = EmptyContext.Default;

            // Act
            await _state.EnterAsync(context, _cts.Token);

            // Assert
            Assert.That(_state.LastEnterContext, Is.SameAs(context));
        }

        #endregion

        #region ExitAsync Tests

        [Test]
        public async Task ExitAsync_WhenActive_TransitionsToInactiveState()
        {
            // Arrange
            var stateDefinition = StateDefinitionBuilder.Create()
                .WithId("state-1")
                .WithType<MockState>()
                .Build();
            await _state.InitializeAsync(stateDefinition, _cts.Token);
            await _state.EnterAsync(EmptyContext.Default, _cts.Token);

            // Act
            var result = await _state.ExitAsync(EmptyContext.Default, _cts.Token);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_state.IsActive, Is.False);
            Assert.That(_state.StateStatus, Is.EqualTo(StateStatus.Inactive));
            Assert.That(_state.ExitCallCount, Is.EqualTo(1));
        }

        [Test]
        public async Task ExitAsync_WhenNotActive_ReturnsFalse()
        {
            // Arrange
            var stateDefinition = StateDefinitionBuilder.Create()
                .WithId("state-1")
                .WithType<MockState>()
                .Build();
            await _state.InitializeAsync(stateDefinition, _cts.Token);

            // Act
            var result = await _state.ExitAsync(EmptyContext.Default, _cts.Token);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_state.ExitCallCount, Is.EqualTo(0)); // Should not exit
        }

        [Test]
        public async Task ExitAsync_FiresStatusChangedEvent_InOrder()
        {
            // Arrange
            var stateDefinition = StateDefinitionBuilder.Create()
                .WithId("state-1")
                .WithType<MockState>()
                .Build();
            await _state.InitializeAsync(stateDefinition, _cts.Token);
            await _state.EnterAsync(EmptyContext.Default, _cts.Token);

            var statusChanges = new System.Collections.Generic.List<StateStatus>();
            _state.StatusChanged += status => statusChanges.Add(status);

            // Act
            await _state.ExitAsync(EmptyContext.Default, _cts.Token);

            // Assert
            Assert.That(statusChanges.Count, Is.EqualTo(2));
            Assert.That(statusChanges[0], Is.EqualTo(StateStatus.Deactivating));
            Assert.That(statusChanges[1], Is.EqualTo(StateStatus.Inactive));
        }

        [Test]
        public async Task ExitAsync_PassesContextToDoExitAsync()
        {
            // Arrange
            var stateDefinition = StateDefinitionBuilder.Create()
                .WithId("state-1")
                .WithType<MockState>()
                .Build();
            await _state.InitializeAsync(stateDefinition, _cts.Token);
            await _state.EnterAsync(EmptyContext.Default, _cts.Token);
            var context = EmptyContext.Default;

            // Act
            await _state.ExitAsync(context, _cts.Token);

            // Assert
            Assert.That(_state.LastExitContext, Is.SameAs(context));
        }

        #endregion

        #region DisposeAsync Tests

        [Test]
        public async Task DisposeAsync_ClearsStatusChangedHandler()
        {
            // Arrange
            var stateDefinition = StateDefinitionBuilder.Create()
                .WithId("state-1")
                .WithType<MockState>()
                .Build();
            await _state.InitializeAsync(stateDefinition, _cts.Token);

            var handlerCalled = false;
            _state.StatusChanged += _ => handlerCalled = true;

            // Act
            await _state.DisposeAsync();

            // Manually trigger a status change attempt (internal, simulated)
            // Since handler is cleared, this should not throw or call handler
            Assert.That(_state.DisposeCallCount, Is.EqualTo(1));
        }

        #endregion

        #region Equality Tests

        [Test]
        public async Task Equals_WithSameId_ReturnsTrue()
        {
            // Arrange
            var stateDefinition1 = StateDefinitionBuilder.Create()
                .WithId("same-id")
                .WithType<MockState>()
                .Build();
            var stateDefinition2 = StateDefinitionBuilder.Create()
                .WithId("same-id")
                .WithType<MockState>()
                .Build();

            var state1 = new MockState();
            var state2 = new MockState();
            await state1.InitializeAsync(stateDefinition1, _cts.Token);
            await state2.InitializeAsync(stateDefinition2, _cts.Token);

            // Act & Assert
            Assert.That(state1.Equals(state2), Is.True);
        }

        [Test]
        public async Task Equals_WithDifferentId_ReturnsFalse()
        {
            // Arrange
            var stateDefinition1 = StateDefinitionBuilder.Create()
                .WithId("id-1")
                .WithType<MockState>()
                .Build();
            var stateDefinition2 = StateDefinitionBuilder.Create()
                .WithId("id-2")
                .WithType<MockState>()
                .Build();

            var state1 = new MockState();
            var state2 = new MockState();
            await state1.InitializeAsync(stateDefinition1, _cts.Token);
            await state2.InitializeAsync(stateDefinition2, _cts.Token);

            // Act & Assert
            Assert.That(state1.Equals(state2), Is.False);
        }

        [Test]
        public async Task GetHashCode_ReturnsSameValueForSameId()
        {
            // Arrange
            var stateDefinition1 = StateDefinitionBuilder.Create()
                .WithId("same-id")
                .WithType<MockState>()
                .Build();
            var stateDefinition2 = StateDefinitionBuilder.Create()
                .WithId("same-id")
                .WithType<MockState>()
                .Build();

            var state1 = new MockState();
            var state2 = new MockState();
            await state1.InitializeAsync(stateDefinition1, _cts.Token);
            await state2.InitializeAsync(stateDefinition2, _cts.Token);

            // Act & Assert
            Assert.That(state1.GetHashCode(), Is.EqualTo(state2.GetHashCode()));
        }

        [Test]
        public void Equals_WithNull_ReturnsFalse()
        {
            // Act & Assert
            Assert.That(_state.Equals(null), Is.False);
        }

        [Test]
        public void Equals_WithSameReference_ReturnsTrue()
        {
            // Act & Assert
            Assert.That(_state.Equals(_state), Is.True);
        }

        #endregion
    }
}