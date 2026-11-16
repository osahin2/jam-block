using UnityEngine;

namespace Game.Play.Settings
{
    [CreateAssetMenu(menuName = "Game/Play/Block Motion Settings")]
    public sealed class BlockMotionSettings : ScriptableObject
    {
        [Tooltip("How far a leaving block slides out through its door, in cells.")]
        [SerializeField] private float _leaveDistance = 3f;
        [SerializeField] private float _leaveDuration = 0.25f;

        public float LeaveDistance => _leaveDistance;
        public float LeaveDuration => _leaveDuration;
    }
}
