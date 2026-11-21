using DG.Tweening;
using UnityEngine;

namespace Game.Play
{
    [CreateAssetMenu(menuName = "Game/Play/Block Motion Settings")]
    public sealed class BlockMotionSettings : ScriptableObject
    {
        [Tooltip("Seconds for a dragged block to close half the distance to where the finger allows it.")]
        [SerializeField] private float _dragHalfLife = 0.03f;
        [Tooltip("Seconds for a block to snap onto its cell on release.")]
        [SerializeField] private float _snapDuration = 0.08f;
        [SerializeField] private Ease _snapEase = Ease.OutQuad;
        [Header("Leaving")]
        [Tooltip("How fast a leaving block speeds up from rest, in cells per second squared.")]
        [SerializeField] private float _leaveAcceleration = 21.5f;
        [Tooltip("Seconds for the door to drop into the frame as the block snaps in front of it.")]
        [SerializeField] private float _doorDropDuration = 0.05f;
        [Tooltip("Seconds the door stays down after the block is gone.")]
        [SerializeField] private float _doorRiseDelay = 0.1f;
        [SerializeField] private float _doorRiseDuration = 0.06f;

        public float DragHalfLife => _dragHalfLife;
        public float SnapDuration => _snapDuration;
        public Ease SnapEase => _snapEase;
        public float DoorDropDuration => _doorDropDuration;
        public float DoorRiseDelay => _doorRiseDelay;
        public float DoorRiseDuration => _doorRiseDuration;

        public float SlideDuration(float cells) => Mathf.Sqrt(2f * cells / _leaveAcceleration);
    }
}
