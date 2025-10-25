#if UNITY_EDITOR
using System;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Editor.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class TypePickerBaseTypePathAttribute : PropertyAttribute
    {
        public string Path { get; }

        public TypePickerBaseTypePathAttribute(string path)
        {
            Path = path;
        }
    }
}
#endif