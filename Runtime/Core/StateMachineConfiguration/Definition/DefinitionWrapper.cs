using UnityEngine;

namespace Dev.Cortez.StateMachines.Core.StateMachineConfiguration.Definition
{
    public class DefinitionWrapper<TData> : ScriptableObject
        where TData : class, new()
    {
        public static string DATA_PROPERTY_NAME = nameof(Data);

        [SerializeField]
        public TData Data = new();
    }
}