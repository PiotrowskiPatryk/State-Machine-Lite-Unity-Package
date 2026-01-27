using System;
using Dev.Cortez.StateMachines.Core.Data;

namespace Dev.Cortez.StateMachines.Core.Condition.Payload
{
    [Flags]
    public enum StateSatisfiedConditionType
    {
        None = 0,
        WhenActivating = 1,
        WhenActivated = 2,
        WhenDeactivating = 4,
        WhenDeactivated = 8
    }

    public static class StateSatisfiedConditionTypeExtensions
    {
        public static bool IsValidStatement(this StateSatisfiedConditionType stateSatisfiedConditionType,
            StateStatus stateStatus)
        {
            if ((stateSatisfiedConditionType & StateSatisfiedConditionType.WhenActivating) != 0 &&
                stateStatus == StateStatus.Activating)
            {
                return true;
            }

            if ((stateSatisfiedConditionType & StateSatisfiedConditionType.WhenActivated) != 0 &&
                stateStatus == StateStatus.Active)
            {
                return true;
            }

            if ((stateSatisfiedConditionType & StateSatisfiedConditionType.WhenDeactivated) != 0 &&
                stateStatus == StateStatus.Inactive)
            {
                return true;
            }

            if ((stateSatisfiedConditionType & StateSatisfiedConditionType.WhenDeactivating) != 0 &&
                stateStatus == StateStatus.Deactivating)
            {
                return true;
            }

            return false;
        }
    }
}