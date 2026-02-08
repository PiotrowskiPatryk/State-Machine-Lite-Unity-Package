using System;
using System.Collections.Generic;
using Dev.Cortez.StateMachines.Core.Interfaces;
using UnityEngine;

namespace Samples.GameScenario.Scripts.StateMachines.Safe
{
    [Serializable]
    public sealed class SafePayload : IPayload
    {
        [SerializeField]
        private List<SafeButtonType> _openSafeSequence = new();

        public IReadOnlyCollection<SafeButtonType> OpenSafeSequence =>
            _openSafeSequence.AsReadOnly();

        public bool IsValid()
        {
            return _openSafeSequence?.Count > 0;
        }
    }
}