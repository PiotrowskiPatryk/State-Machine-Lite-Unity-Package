using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace Samples.GameScenario.Scripts
{
    public sealed class SafeController : MonoBehaviour
    {
        private static readonly int OPEN_SAFE = Animator.StringToHash("OpenSafe");
        private static readonly int CLOSE_SAFE = Animator.StringToHash("CloseSafe");

        public event Action<SafeButtonType> PressedSafeButton;

        [SerializeField]
        private SafeButton[] _safeButtons;

        [SerializeField]
        private Animator _animator;

        [SerializeField]
        private float _showHideDoorDurationInSeconds = 3.0f;

        public static SafeController Instance { get; private set; }

        public UniTask OpenSafeAsync(CancellationToken token)
        {
            _animator.SetTrigger(OPEN_SAFE);

            return UniTask.Delay(TimeSpan.FromSeconds(_showHideDoorDurationInSeconds), cancellationToken: token);
        }

        public UniTask CloseSafeAsync(CancellationToken token)
        {
            _animator.SetTrigger(CLOSE_SAFE);

            return UniTask.Delay(TimeSpan.FromSeconds(_showHideDoorDurationInSeconds), cancellationToken: token);
        }

        private void Awake()
        {
            Instance = this;

            foreach (var safeButton in _safeButtons)
            {
                safeButton.PressedButton += OnPressedSafeButton;
            }
        }

        private void OnDestroy()
        {
            foreach (var safeButton in _safeButtons)
            {
                safeButton.PressedButton -= OnPressedSafeButton;
            }
        }

        private void OnPressedSafeButton(SafeButtonType safeButtonType)
        {
            PressedSafeButton?.Invoke(safeButtonType);
        }
    }
}