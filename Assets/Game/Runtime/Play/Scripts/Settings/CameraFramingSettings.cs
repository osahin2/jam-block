using UnityEngine;

namespace Game.Play
{
    [CreateAssetMenu(menuName = "Game/Play/Camera Framing Settings")]
    public sealed class CameraFramingSettings : ScriptableObject
    {
        [Tooltip("Room on the left and on the right, as a fraction of the screen's width.")]
        [SerializeField, Range(0f, 0.4f)] private float _sideMargin = 0.05f;
        [Tooltip("Room above and below, as a fraction of the screen's height.")]
        [SerializeField, Range(0f, 0.4f)] private float _topAndBottomMargin = 0.12f;

        public float SideMargin => _sideMargin;
        public float TopAndBottomMargin => _topAndBottomMargin;
    }
}
