using UnityEngine;

namespace Game.Play
{
    [CreateAssetMenu(menuName = "Game/Play/Time Warning Settings")]
    public sealed class TimeWarningSettings : ScriptableObject
    {
        [Tooltip("The time box flashes once a second while this many seconds or fewer are left.")]
        [SerializeField, Min(0)] private int _lastSeconds = 30;
        [Tooltip("The overlay's color at the height of a flash; its alpha is how strongly it covers the time box.")]
        [SerializeField] private Color _flashColor = new Color(0.95f, 0.2f, 0.2f, 0.85f);
        [Tooltip("Seconds for one flash, to the color and back; under a second, so flashes stay apart.")]
        [SerializeField, Range(0.1f, 1f)] private float _flashDuration = 0.5f;

        public int LastSeconds => _lastSeconds;
        public Color FlashColor => _flashColor;
        public float FlashDuration => _flashDuration;
    }
}
