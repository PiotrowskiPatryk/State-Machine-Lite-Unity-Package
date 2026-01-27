using Dev.Cortez.StateMachines.Core.StateMachineConfiguration.Definition;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Editor.StateMachineEditor.ViewModels
{
    /// <summary>
    /// Concrete wrapper classes for Unity serialization.
    /// Unity cannot instantiate generic ScriptableObjects (e.g., DefinitionWrapper&lt;T&gt;) directly,
    /// so these concrete non-generic wrappers provide a workaround.
    /// These are marked internal as they are implementation details for standalone ViewModel construction.
    /// </summary>
    
    /// <summary>
    /// Concrete wrapper for ConditionDefinition serialization.
    /// </summary>
    internal sealed class ConditionDefinitionEditorWrapper : ScriptableObject
    {
        public const string DATA_PROPERTY_NAME = nameof(Data);
        
        [SerializeField]
        private ConditionDefinition Data;
    }

    /// <summary>
    /// Concrete wrapper for StateDefinition serialization.
    /// </summary>
    internal sealed class StateDefinitionEditorWrapper : ScriptableObject
    {
        public const string DATA_PROPERTY_NAME = nameof(Data);
        
        [SerializeField]
        private StateDefinition Data;
    }

    /// <summary>
    /// Concrete wrapper for StateMachineDefinition serialization.
    /// </summary>
    internal sealed class StateMachineDefinitionEditorWrapper : ScriptableObject
    {
        public const string DATA_PROPERTY_NAME = nameof(Data);
        
        [SerializeField]
        private StateMachineDefinition Data;
    }

    /// <summary>
    /// Concrete wrapper for TransitionRuleDefinition serialization.
    /// </summary>
    internal sealed class TransitionRuleDefinitionEditorWrapper : ScriptableObject
    {
        public const string DATA_PROPERTY_NAME = nameof(Data);
        
        [SerializeField]
        private TransitionRuleDefinition Data;
    }

    /// <summary>
    /// Concrete wrapper for TriggerDefinition serialization.
    /// </summary>
    internal sealed class TriggerDefinitionEditorWrapper : ScriptableObject
    {
        public const string DATA_PROPERTY_NAME = nameof(Data);
        
        [SerializeField]
        private TriggerDefinition Data;
    }
}
