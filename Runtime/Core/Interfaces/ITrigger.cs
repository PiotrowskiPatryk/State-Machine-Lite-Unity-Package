using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Dev.Cortez.StateMachines.Core.Interfaces
{
    /// <summary>
    /// Represents a trigger that can be activated or deactivated, often used to initiate state transitions.
    /// </summary>
    public interface ITrigger : IIdentifiable, IAsyncDisposable
    {
        /// <summary>
        /// Occurs when the <see cref="IsTriggered"/> value changes.
        /// </summary>
        event Action<ITrigger, bool> TriggeredValueChanged;

        /// <summary>
        /// Gets a value indicating whether this trigger is currently active or "triggered".
        /// </summary>
        bool IsTriggered { get; }

        /// <summary>
        /// Asynchronously sets the trigger's value to the specified target value.
        /// </summary>
        /// <param name="targetValue">The desired value for the trigger (true for triggered, false for untriggered).</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref="UniTask{TResult}"/> representing the asynchronous operation, returning <c>true</c> if the value was successfully set; otherwise, <c>false</c>.</returns>
        UniTask<bool> TriggerValueAsync(bool targetValue, CancellationToken cancellationToken);
    }
}