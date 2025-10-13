using System;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Attributes
{
    /// <summary>
    /// Add this attribute to a [SerializeReference] field to render a type dropdown for any class
    /// assignable to the field's declared type (typically an interface or abstract class).
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class SerializeReferenceSelectorAttribute : PropertyAttribute
    {
        /// <summary>
        /// Optional: Provide an explicit base type constraint. If null, the field's declared type is used.
        /// </summary>
        public readonly Type BaseType;

        public SerializeReferenceSelectorAttribute(Type baseType = null)
        {
            BaseType = baseType;
        }
    }
}
