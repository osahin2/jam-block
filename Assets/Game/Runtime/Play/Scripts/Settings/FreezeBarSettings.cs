using DG.Tweening;
using UnityEngine;

namespace Game.Play
{
    [CreateAssetMenu(menuName = "Game/Play/Freeze Bar Settings")]
    public sealed class FreezeBarSettings : ScriptableObject
    {
        [Tooltip("Seconds the bar takes to pop in when a freeze starts.")]
        [SerializeField, Min(0f)] private float _openDuration = 0.3f;
        [Tooltip("The bar's scale when it starts to pop in; it grows to its own size.")]
        [SerializeField, Min(0f)] private float _openScale = 0.6f;
        [SerializeField] private Ease _openEase = Ease.OutBack;
        [Tooltip("Seconds the bar takes to fade out once the freeze is over.")]
        [SerializeField, Min(0f)] private float _closeDuration = 0.25f;
        [SerializeField] private Ease _closeEase = Ease.InQuad;

        public float OpenDuration => _openDuration;
        public float OpenScale => _openScale;
        public Ease OpenEase => _openEase;
        public float CloseDuration => _closeDuration;
        public Ease CloseEase => _closeEase;
    }
}
