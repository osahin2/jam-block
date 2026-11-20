using UnityEngine;

namespace Game.Play
{
    [CreateAssetMenu(menuName = "Game/Play/Color Palette")]
    public sealed class ColorPalette : ScriptableObject
    {
        [SerializeField] private Material[] _colors;
        [Tooltip("One per color, in the same order as the block materials and in the same color.")]
        [SerializeField] private Material[] _doorColors;

        public int Count => _colors.Length;

        public Material ForBlock(int colorId) => _colors[colorId];

        public Material ForDoor(int colorId) => _doorColors[colorId];

        public bool Has(int colorId) =>
            colorId >= 0 && colorId < _colors.Length && _colors[colorId] != null &&
            colorId < _doorColors.Length && _doorColors[colorId] != null;
    }
}
