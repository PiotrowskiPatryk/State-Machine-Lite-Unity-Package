using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.Abstraction;

namespace StateMachineExamples.Scenario.Scripts.Trigger
{
    public class ScenarioTrigger : TriggerBase<ScenarioTriggerPayload>
    {
        private ScenarioTriggerPayload _payload;

        public ScenarioTrigger(string id, string name, string description) : base(id, name, description)
        {
        }

        protected override UniTask<bool> InitializeAsync(ScenarioTriggerPayload payload, CancellationToken cancellationToken)
        {
            _payload = payload;
            ScenarioEventBus.OnEvent += HandleEvent;
            return UniTask.FromResult(true);
        }

        protected override ValueTask DoDisposeAsync()
        {
            ScenarioEventBus.OnEvent -= HandleEvent;
            return base.DoDisposeAsync();
        }

        private void HandleEvent(ScenarioEventType type, string id)
        {
            if (_payload != null && _payload.EventType == type && _payload.TargetId == id)
            {
                if (!IsTriggered)
                {
                    IsTriggered = true;
                }
            }
        }

        public override UniTask<bool> TriggerValueAsync(bool targetValue, CancellationToken cancellationToken)
        {
            IsTriggered = targetValue;
            return UniTask.FromResult(true);
        }
    }
}
