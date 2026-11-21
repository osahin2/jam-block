using DG.Tweening;
using UnityEngine;

namespace Game.Play
{
    [CreateAssetMenu(menuName = "Game/Play/Ice Settings")]
    public sealed class IceSettings : ScriptableObject
    {
        [Header("Look")]
        [Tooltip("Laid over an iced block's top in the board's space, so it runs on across the block's cells.")]
        [SerializeField] private Texture2D _surface;
        [Tooltip("Repeats of the surface texture per board unit.")]
        [SerializeField] private float _surfaceScale = 0.25f;
        [Tooltip("An iced block's color toward the light, multiplied by the surface texture.")]
        [SerializeField] private Color _color = Color.white;
        [Tooltip("An iced block's color away from the light, multiplied by the surface texture.")]
        [SerializeField] private Color _shadowColor = new Color(0.7f, 0.8f, 0.95f);
        [Tooltip("Height of the ice count above the block's top, in board units; above an arrow block's arrow.")]
        [SerializeField] private float _countLift = 0.45f;
        [Header("Crack")]
        [Tooltip("How much the count swells, as a share of its size, when the ice drops.")]
        [SerializeField] private float _crackPunch = 0.35f;
        [SerializeField] private float _crackDuration = 0.3f;
        [SerializeField] private int _crackVibrato = 6;
        [Tooltip("Shards thrown from each cell when the ice drops.")]
        [SerializeField] private int _crackShards = 2;
        [Header("Melt")]
        [Tooltip("Seconds for the block to turn from ice back to its color when the ice is gone.")]
        [SerializeField] private float _meltDuration = 0.35f;
        [SerializeField] private Ease _meltEase = Ease.OutQuad;
        [Tooltip("Seconds for the count to shrink away when the ice is gone.")]
        [SerializeField] private float _countShrinkDuration = 0.15f;
        [Tooltip("Shards thrown from each cell when the ice is gone.")]
        [SerializeField] private int _meltShards = 6;

        public Texture2D Surface => _surface;
        public float SurfaceScale => _surfaceScale;
        public Color Color => _color;
        public Color ShadowColor => _shadowColor;
        public float CountLift => _countLift;
        public float CrackPunch => _crackPunch;
        public float CrackDuration => _crackDuration;
        public int CrackVibrato => _crackVibrato;
        public int CrackShards => _crackShards;
        public float MeltDuration => _meltDuration;
        public Ease MeltEase => _meltEase;
        public float CountShrinkDuration => _countShrinkDuration;
        public int MeltShards => _meltShards;
    }
}
