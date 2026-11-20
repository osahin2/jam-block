using UnityEngine;

namespace Game.Play
{
    [CreateAssetMenu(menuName = "Game/Play/Camera Framing Settings")]
    public sealed class CameraFramingSettings : ScriptableObject
    {
        [Tooltip("Room on the left and on the right, from the safe area's edges, as a fraction of the screen's width.")]
        [SerializeField, Range(0f, 0.4f)] private float _sideMargin = 0.05f;
        [Tooltip("Room above the board for the HUD, from the safe area's top, as a fraction of the screen's width: the UI scales with the width, so the HUD takes the same share of it on every phone.")]
        [SerializeField, Range(0f, 1f)] private float _topMargin = 0.44f;
        [Tooltip("Room below the board for the booster slots, from the safe area's bottom, as a fraction of the screen's width.")]
        [SerializeField, Range(0f, 1f)] private float _bottomMargin = 0.37f;

        public float SideMargin => _sideMargin;
        public float TopMargin => _topMargin;
        public float BottomMargin => _bottomMargin;
    }
}
