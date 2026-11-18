using UnityEngine;

namespace Game.Play
{
    // Block and door material per color id of the level data.
    [CreateAssetMenu(menuName = "Game/Play/Color Palette")]
    public sealed class ColorPalette : ScriptableObject
    {
        [SerializeField] private Material[] _colors;

        public int Count => _colors.Length;

        public Material For(int colorId) => _colors[colorId];

        // Whether the color id has a material, so it can be drawn and built.
        public bool Has(int colorId) => colorId >= 0 && colorId < _colors.Length && _colors[colorId] != null;
    }
}
