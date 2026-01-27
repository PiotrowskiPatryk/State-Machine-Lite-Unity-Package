using System;
using System.Threading;
using Dev.Cortez.StateMachines.Core.Interfaces;

namespace Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Common.Mocks
{
    /// <summary>
    /// A mock reference picker that allows direct control over reference resolution
    /// without depending on StateMachineContainerRegistry.
    /// </summary>
    public sealed class MockTriggerReferencePicker
    {
        private readonly string _selectedItemId;
        private ITrigger _reference;
        private Action<ITrigger> _onResolvedCallback;
        private Action _onUnresolvedCallback;

        public bool IsReferenceSelected => !string.IsNullOrWhiteSpace(_selectedItemId);
        public ITrigger Reference => _reference;

        public MockTriggerReferencePicker(string selectedItemId = "test-trigger")
        {
            _selectedItemId = selectedItemId;
        }

        /// <summary>
        /// Simulates the Observe method from ReferencePickerBase.
        /// </summary>
        public void Observe(Action<ITrigger> onResolved, Action onUnresolved, CancellationToken cancellationToken)
        {
            _onResolvedCallback = onResolved;
            _onUnresolvedCallback = onUnresolved;
        }

        /// <summary>
        /// Resolves the reference with the provided trigger.
        /// </summary>
        public void ResolveReference(ITrigger trigger)
        {
            _reference = trigger;
            _onResolvedCallback?.Invoke(trigger);
        }

        /// <summary>
        /// Unresolves the reference.
        /// </summary>
        public void UnresolveReference()
        {
            _reference = null;
            _onUnresolvedCallback?.Invoke();
        }
    }

    /// <summary>
    /// A mock reference picker for states that allows direct control over reference resolution
    /// without depending on StateMachineContainerRegistry.
    /// </summary>
    public sealed class MockStateReferencePicker
    {
        private readonly string _selectedItemId;
        private IState _reference;
        private Action<IState> _onResolvedCallback;
        private Action _onUnresolvedCallback;

        public bool IsReferenceSelected => !string.IsNullOrWhiteSpace(_selectedItemId);
        public IState Reference => _reference;

        public MockStateReferencePicker(string selectedItemId = "test-state")
        {
            _selectedItemId = selectedItemId;
        }

        /// <summary>
        /// Simulates the Observe method from ReferencePickerBase.
        /// </summary>
        public void Observe(Action<IState> onResolved, Action onUnresolved, CancellationToken cancellationToken)
        {
            _onResolvedCallback = onResolved;
            _onUnresolvedCallback = onUnresolved;
        }

        /// <summary>
        /// Resolves the reference with the provided state.
        /// </summary>
        public void ResolveReference(IState state)
        {
            _reference = state;
            _onResolvedCallback?.Invoke(state);
        }

        /// <summary>
        /// Unresolves the reference.
        /// </summary>
        public void UnresolveReference()
        {
            _reference = null;
            _onUnresolvedCallback?.Invoke();
        }
    }
}
