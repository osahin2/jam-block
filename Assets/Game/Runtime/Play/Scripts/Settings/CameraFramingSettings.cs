using UnityEngine;

namespace Game.Play
{
    [CreateAssetMenu(menuName = "Game/Play/Camera Framing Settings")]
    public sealed class CameraFramingSettings : ScriptableObject
    {
        [Tooltip("The UI canvases' reference resolution, which the top and bottom margins are measured in; a test keeps it equal to GameUIRoot's.")]
        [SerializeField] private Vector2 _uiReferenceResolution = new Vector2(1080f, 1920f);
        [Tooltip("Room on the left and on the right, from the safe area's edges, as a fraction of the screen's width.")]
        [SerializeField, Range(0f, 0.4f)] private float _sideMargin = 0.06f;
        [Tooltip("Room above the board for the HUD, from the safe area's top, in UI reference units: the UI is scaled by the screen's height, so the HUD takes the same share of the height on every screen.")]
        [SerializeField, Range(0f, 600f)] private float _topMargin = 360f;
        [Tooltip("Room below the board for the booster slots, from the safe area's bottom, in UI reference units.")]
        [SerializeField, Range(0f, 600f)] private float _bottomMargin = 340f;

        public Vector2 UiReferenceResolution => _uiReferenceResolution;
        public float SideMargin => _sideMargin;
        public float TopMargin => _topMargin;
        public float BottomMargin => _bottomMargin;
    }
}
