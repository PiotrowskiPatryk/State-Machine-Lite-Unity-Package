using Dev.Cortez.StateMachines.Core.Data;

namespace Dev.Cortez.StateMachines.Extensions
{
    /// <summary>
    /// Provides extension methods for the <see cref="InitializationStatus"/> enum.
    /// </summary>
    public static class InitializationStatusExtensions
    {
        /// <summary>
        /// Determines whether a component with the given <see cref="InitializationStatus"/> can be initialized.
        /// </summary>
        /// <param name="status">The current <see cref="InitializationStatus"/> of the component.</param>
        /// <returns><c>true</c> if the component can be initialized (i.e., its status is <see cref="InitializationStatus.NotInitialized"/> or <see cref="InitializationStatus.Failed"/>); otherwise, <c>false</c>.</returns>
        public static bool CanInitialize(this InitializationStatus status)
        {
            return status is InitializationStatus.NotInitialized or InitializationStatus.Failed;
        }
    }
}
