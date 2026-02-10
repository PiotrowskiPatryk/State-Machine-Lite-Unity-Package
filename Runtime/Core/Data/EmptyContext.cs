using JetBrains.Annotations;

namespace Dev.Cortez.StateMachines.Core.Data
{
    [UsedImplicitly]
    public sealed class EmptyContext
    {
        public static EmptyContext Default => new();
    }
}