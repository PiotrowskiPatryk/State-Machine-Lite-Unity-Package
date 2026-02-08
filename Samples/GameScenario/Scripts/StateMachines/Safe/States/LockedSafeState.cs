using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Dev.Cortez.StateMachines.Core.ReferencePicker.Trigger;
using JetBrains.Annotations;

namespace Samples.GameScenario.Scripts.StateMachines.Safe
{
    [UsedImplicitly]
    public sealed class LockedSafeState : SafeStateBase<LockedSafeStatePayload>
    {
        private Queue<SafeButtonType> _originalSafeButtonSequence;
        private Queue<SafeButtonType> _currentSafeButtonSequence;
        private LockedSafeStatePayload _lockedSafeStatePayload;

        protected override UniTask<bool> DoInitializeAsync(LockedSafeStatePayload payload,
            CancellationToken cancellationToken)
        {
            _lockedSafeStatePayload = payload;

            return UniTask.FromResult(true);
        }

        protected override UniTask DoEnterAsync(StateContext context, CancellationToken cancellationToken)
        {
            SubscribeToEvents(context);
            InitializeSafeButtonsQueue(context.UnlockSafeButtonSequence);

            return UniTask.CompletedTask;
        }

        protected override UniTask DoExitAsync(StateContext context, CancellationToken cancellationToken)
        {
            UnsubscribeFromEvents(context);

            return UniTask.CompletedTask;
        }

        private void InitializeSafeButtonsQueue(IReadOnlyCollection<SafeButtonType> contextUnlockSafeButtonSequence)
        {
            _originalSafeButtonSequence = new Queue<SafeButtonType>(contextUnlockSafeButtonSequence);
            RestartButtonSequence();
        }

        private void SubscribeToEvents(StateContext context)
        {
            context.SafeController.PressedSafeButton += OnPressedSafeButton;
        }

        private void UnsubscribeFromEvents(StateContext context)
        {
            context.SafeController.PressedSafeButton -= OnPressedSafeButton;
        }

        private bool PressedValidButton(SafeButtonType safeButtonType)
        {
            return safeButtonType == _currentSafeButtonSequence.Dequeue();
        }

        private void RestartButtonSequence()
        {
            _currentSafeButtonSequence = new Queue<SafeButtonType>(_originalSafeButtonSequence);
        }

        private void OnPressedSafeButton(SafeButtonType safeButtonType)
        {
            if (!PressedValidButton(safeButtonType))
            {
                RestartButtonSequence();
                TriggerInvalidButtonSequencePressed();

                return;
            }

            if (_currentSafeButtonSequence.Count == 0)
            {
                TriggerValidButtonSequencePressedAsync().Forget();
            }
        }

        private void TriggerInvalidButtonSequencePressed()
        {
            _ = _lockedSafeStatePayload.TriggeredInvalidSafeButtonSequence.
                ResolveReferenceAsync(CancellationToken.None).ContinueWith(trigger =>
                    trigger.TriggerValueAsync(true, CancellationToken.None).Forget());
        }

        private async UniTaskVoid TriggerValidButtonSequencePressedAsync()
        {
            _ = _lockedSafeStatePayload.TriggeredValidSafeButtonSequence.ResolveReferenceAsync(CancellationToken.None).
                ContinueWith(trigger => trigger.TriggerValueAsync(true, CancellationToken.None).Forget());
        }
    }
}