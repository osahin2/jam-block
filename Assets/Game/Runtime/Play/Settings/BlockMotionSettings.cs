using UnityEngine;

namespace Game.Play.Settings
{
    [CreateAssetMenu(menuName = "Game/Play/Block Motion Settings")]
    public sealed class BlockMotionSettings : ScriptableObject
    {
        [Tooltip("Seconds for a dragged block to close half the distance to where the finger allows it.")]
        [SerializeField] private float _dragHalfLife = 0.03f;
        [Tooltip("Seconds for a released block to close half the distance to its cell.")]
        [SerializeField] private float _settleHalfLife = 0.06f;
        [Tooltip("How far a leaving block slides out through its door, in cells.")]
        [SerializeField] private float _leaveDistance = 3f;
        [SerializeField] private float _leaveDuration = 0.25f;

        public float DragHalfLife => _dragHalfLife;
        public float SettleHalfLife => _settleHalfLife;
        public float LeaveDistance => _leaveDistance;
        public float LeaveDuration => _leaveDuration;
    }
}
