using DG.Tweening;
using UnityEngine;

namespace Game.Popups
{
    [CreateAssetMenu(menuName = "Game/Popups/Popup Motion Settings")]
    public sealed class PopupMotionSettings : ScriptableObject
    {
        [Tooltip("Seconds for the dimmer to fade in to its alpha in the prefab.")]
        [SerializeField, Min(0f)] private float _dimDuration = 0.15f;
        [Tooltip("A part's size when it starts to grow, as a fraction of its size in the prefab.")]
        [SerializeField, Range(0f, 1f)] private float _startScale = 0.5f;
        [Tooltip("Seconds for one part to grow to its size.")]
        [SerializeField, Min(0f)] private float _popDuration = 0.3f;
        [SerializeField] private Ease _popEase = Ease.OutBack;
        [Tooltip("Seconds between the starts of two parts that pop one after the other.")]
        [SerializeField, Min(0f)] private float _stagger = 0.15f;

        public float DimDuration => _dimDuration;
        public float StartScale => _startScale;
        public float PopDuration => _popDuration;
        public Ease PopEase => _popEase;
        public float Stagger => _stagger;
    }
}
