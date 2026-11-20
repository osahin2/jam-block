using DG.Tweening;
using UnityEngine;

namespace Game.Preferences
{
    [CreateAssetMenu(menuName = "Game/Preferences/Switch Settings")]
    public sealed class SwitchSettings : ScriptableObject
    {
        [Tooltip("Seconds for the knob to slide to the other end when the switch is pressed.")]
        [SerializeField, Min(0f)] private float _duration = 0.15f;
        [SerializeField] private Ease _ease = Ease.OutQuad;
        [SerializeField] private Color _offTrack = new Color(0.14f, 0.2f, 0.42f, 1f);
        [SerializeField] private Color _onTrack = new Color(0.02f, 0.3f, 0.08f, 1f);

        public float Duration => _duration;
        public Ease Ease => _ease;
        public Color OffTrack => _offTrack;
        public Color OnTrack => _onTrack;
    }
}
