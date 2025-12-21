namespace Dev.Cortez.StateMachines.Core.Interfaces
{
    /// <summary>
    /// Represents an object that can be validated.
    /// </summary>
    public interface IValidatable
    {
        /// <summary>
        /// Determines whether the implementing object is in a valid state.
        /// </summary>
        /// <returns><c>true</c> if the object is valid; otherwise, <c>false</c>.</returns>
        bool IsValid();
    }
}