using System;
using Dev.Cortez.StateMachines.Core.StateMachineConfiguration;
using Dev.Cortez.StateMachines.Core.StateMachineConfiguration.Definition;
using Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Common.Data;
using UnityEngine;

namespace Dev.Cortez.StateMachines.EditorTests.Tests.Editor.Common.Builders
{
    /// <summary>
    /// Base class for test wrappers.
    /// Unity cannot instantiate generic ScriptableObjects via ScriptableObject.CreateInstance,
    /// so we need concrete wrapper classes for each data type.
    /// </summary>
    public abstract class TestWrapperBase : ScriptableObject { }

    /// <summary>
    /// Wrapper for ConditionDefinition testing.
    /// </summary>
    public sealed class ConditionDefinitionWrapper : TestWrapperBase
    {
        public const string DATA_PROPERTY_NAME = nameof(Data);

        [SerializeField]
        public ConditionDefinition Data = new();
    }

    /// <summary>
    /// Wrapper for StateDefinition testing.
    /// </summary>
    public sealed class StateDefinitionWrapper : TestWrapperBase
    {
        public const string DATA_PROPERTY_NAME = nameof(Data);

        [SerializeField]
        public StateDefinition Data = new();
    }

    /// <summary>
    /// Wrapper for TransitionRuleDefinition testing.
    /// </summary>
    public sealed class TransitionRuleDefinitionWrapper : TestWrapperBase
    {
        public const string DATA_PROPERTY_NAME = nameof(Data);

        [SerializeField]
        public TransitionRuleDefinition Data = new();
    }

    /// <summary>
    /// Wrapper for TriggerDefinition testing.
    /// </summary>
    public sealed class TriggerDefinitionWrapper : TestWrapperBase
    {
        public const string DATA_PROPERTY_NAME = nameof(Data);

        [SerializeField]
        public TriggerDefinition Data = new();
    }

    /// <summary>
    /// Wrapper for StateMachineDefinition testing.
    /// </summary>
    public sealed class StateMachineDefinitionWrapper : TestWrapperBase
    {
        public const string DATA_PROPERTY_NAME = nameof(Data);

        [SerializeField]
        public StateMachineDefinition Data = new();
    }

    /// <summary>
    /// Wrapper for TriggerConfiguration testing.
    /// </summary>
    public sealed class TriggerConfigurationWrapper : TestWrapperBase
    {
        public const string DATA_PROPERTY_NAME = nameof(Data);

        [SerializeField]
        public TriggerConfiguration Data = new();
    }

    /// <summary>
    /// Wrapper for StateMachineConfiguration testing.
    /// </summary>
    public sealed class StateMachineConfigurationWrapper : TestWrapperBase
    {
        public const string DATA_PROPERTY_NAME = nameof(Data);

        [SerializeField]
        public StateMachineConfiguration Data = new();
    }

    /// <summary>
    /// Wrapper for AllPropertyTypesData testing.
    /// </summary>
    public sealed class AllPropertyTypesDataWrapper : TestWrapperBase
    {
        public const string DATA_PROPERTY_NAME = nameof(Data);

        [SerializeField]
        public AllPropertyTypesData Data = new();
    }
}
