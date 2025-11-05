using System;
using Dev.Cortez.StateMachines.Core.Interfaces;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Core.Condition
{
    [Serializable]
    public class TestConditionPayload : IPayload
    {
        public GameObject instance;

        public bool IsValid()
        {
            return true;
        }
    }
}