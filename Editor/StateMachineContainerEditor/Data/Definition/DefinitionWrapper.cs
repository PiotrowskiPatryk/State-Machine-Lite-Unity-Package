using UnityEngine;

namespace Dev.Cortez.StateMachines.Editor.StateMachineContainerEditor.Data.Definition
{
    public class DefinitionWrapper<TData> : ScriptableObject
        where TData : class, new()
    {
        [SerializeField]
        public TData Data = new();
    }
}