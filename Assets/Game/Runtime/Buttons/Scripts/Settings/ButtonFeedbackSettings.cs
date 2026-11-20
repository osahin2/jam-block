using DG.Tweening;
using UnityEngine;

namespace Game.Buttons
{
    [CreateAssetMenu(menuName = "Game/Buttons/Button Feedback Settings")]
    public sealed class ButtonFeedbackSettings : ScriptableObject
    {
        [Tooltip("The button's size while pressed, as a fraction of its size at rest.")]
        [SerializeField, Range(0.5f, 1f)] private float _pressedScale = 0.9f;
        [Tooltip("Seconds to shrink when pressed.")]
        [SerializeField, Min(0f)] private float _pressDuration = 0.08f;
        [SerializeField] private Ease _pressEase = Ease.OutQuad;
        [Tooltip("Seconds to spring back when released.")]
        [SerializeField, Min(0f)] private float _releaseDuration = 0.2f;
        [SerializeField] private Ease _releaseEase = Ease.OutBack;

        public float PressedScale => _pressedScale;
        public float PressDuration => _pressDuration;
        public Ease PressEase => _pressEase;
        public float ReleaseDuration => _releaseDuration;
        public Ease ReleaseEase => _releaseEase;
    }
}
