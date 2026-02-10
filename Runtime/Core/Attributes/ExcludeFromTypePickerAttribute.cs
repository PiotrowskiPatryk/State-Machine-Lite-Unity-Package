using System;

namespace Dev.Cortez.StateMachines.Core.Attributes
{
    /// <summary>
    /// Apply this attribute to a class to exclude it from appearing in TypePickerDropdownField.
    /// Useful for hiding mock, test, or internal implementation classes from the editor.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class ExcludeFromTypePickerAttribute : Attribute
    {
    }
}