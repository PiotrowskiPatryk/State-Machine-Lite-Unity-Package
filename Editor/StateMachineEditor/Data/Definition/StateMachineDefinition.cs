using System;
using System.Collections.Generic;
using Dev.Cortez.StateMachines.Core.Interfaces;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.Data.Definition
{
    [Serializable]
    public sealed class StateMachineDefinition
    {
        [SerializeField]
        private string _id;
        [SerializeField]
        private string _name;
        [SerializeField]
        private string _description;
        [SerializeField]
        private string _stateMachineTypeName;
        [SerializeField] 
        private string _transitionSolverTypeName;
        [SerializeReference]
        private List<StateDefinition> _states;
        [SerializeReference]
        private IPayload _payload;

        public string Id => _id;
        public string Name => _name;
        public string Description => _description;
        public string StateMachineTypeName => _stateMachineTypeName;
        public string TransitionSolverTypeName => _transitionSolverTypeName;
        public List<StateDefinition> States => _states;
        public IPayload Payload => _payload;
    }
}
