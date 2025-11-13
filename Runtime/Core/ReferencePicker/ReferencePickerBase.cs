using System;
using UnityEngine;

namespace Dev.Cortez.StateMachines.Core.ReferencePicker
{
    [Serializable]
    public abstract class ReferencePickerBase
    {
        public static string SELECTED_STATE_MACHINE_CONTAINER_GUID_PROPERTY_NAME =
            nameof(_selectedStateMachineContainerGUID);

        public static string SELECTED_ITEM_ID_PROPERTY_NAME = nameof(_selectedItemId);
        public static string SELECTED_ITEM_NAME_PROPERTY_NAME = nameof(_selectedItemName);

        [SerializeField]
        private string _selectedStateMachineContainerGUID;

        [SerializeField]
        private string _selectedItemId;

        [SerializeField]
        private string _selectedItemName;
    }

    [Serializable]
    public class TriggerReferencePicker : ReferencePickerBase
    {
    }
}