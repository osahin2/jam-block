using UnityEngine;

namespace Game.Play
{
    [CreateAssetMenu(menuName = "Game/Play/Block Outline Settings")]
    public sealed class BlockOutlineSettings : ScriptableObject
    {
        [SerializeField] private Color _color = new Color(1f, 0.981f, 0.769f);
        [Tooltip("In Quick Outline's units: the fill is pushed out by width / 1000 of the depth, so it keeps its size on " +
                 "screen.")]
        [SerializeField, Range(0f, 10f)] private float _width = 3f;
        [SerializeField] private Shader _maskShader;
        [SerializeField] private Shader _fillShader;

        public Color Color => _color;
        public float Width => _width;
        public Shader MaskShader => _maskShader;
        public Shader FillShader => _fillShader;
    }
}
