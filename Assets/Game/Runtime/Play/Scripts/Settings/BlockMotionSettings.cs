using DG.Tweening;
using UnityEngine;

namespace Game.Play.Settings
{
    [CreateAssetMenu(menuName = "Game/Play/Block Motion Settings")]
    public sealed class BlockMotionSettings : ScriptableObject
    {
        [Tooltip("Seconds for a dragged block to close half the distance to where the finger allows it.")]
        [SerializeField] private float _dragHalfLife = 0.03f;
        [Tooltip("Seconds for a block to snap onto its cell on release, or onto the cell in front of its door.")]
        [SerializeField] private float _snapDuration = 0.08f;
        [SerializeField] private Ease _snapEase = Ease.OutQuad;
        [Tooltip("Seconds a leaving block waits in front of its door before it slides out.")]
        [SerializeField] private float _leaveHoldDuration = 0.12f;
        [Tooltip("How far a leaving block slides out through its door, in cells.")]
        [SerializeField] private float _leaveDistance = 3f;
        [SerializeField] private float _leaveDuration = 0.25f;
        [SerializeField] private Ease _leaveEase = Ease.InQuad;

        public float DragHalfLife => _dragHalfLife;
        public float SnapDuration => _snapDuration;
        public Ease SnapEase => _snapEase;
        public float LeaveHoldDuration => _leaveHoldDuration;
        public float LeaveDistance => _leaveDistance;
        public float LeaveDuration => _leaveDuration;
        public Ease LeaveEase => _leaveEase;
    }
}
