using Dev.Cortez.StateMachines.Core.Interfaces;

namespace Dev.Cortez.StateMachines.Core.Data
{
    /// <summary>
    ///     Represents an empty implementation of the <see cref="IPayload" /> interface.
    ///     This class is used as a placeholder when no specific payload is required
    ///     or provided for a state machine operation.
    /// </summary>
    public sealed class EmptyPayload : IPayload
    {
        public bool IsValid()
        {
            return true;
        }
    }
}