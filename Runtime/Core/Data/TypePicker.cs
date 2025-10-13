using System;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Core.Data
{
    [Serializable]
    public class TypePicker<BaseType> where BaseType : Type
    {
        [SerializeField]
        private Type _selectedType;
        [SerializeField]
        private string _selectedTypeName;
    }
}
