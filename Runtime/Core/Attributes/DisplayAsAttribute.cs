#region

using System;

#endregion

namespace Dev.Cortez.StateMachines.Core.Attributes
{
    /// <summary>
    ///     Apply this attribute to a class to override display type name in the TypePickerDropdownField.
    ///     Useful for managing and grouping multiple types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class DisplayAsAttribute : Attribute
    {
        public string CustomName { get; }

        public DisplayAsAttribute(string customName)
        {
            CustomName = customName;
        }
    }
}