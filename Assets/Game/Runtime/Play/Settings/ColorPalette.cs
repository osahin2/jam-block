using UnityEngine;

namespace Game.Play.Settings
{
    // Block and door material per color id of the level data.
    [CreateAssetMenu(menuName = "Game/Play/Color Palette")]
    public sealed class ColorPalette : ScriptableObject
    {
        [SerializeField] private Material[] _colors;

        public Material For(int colorId) => _colors[colorId];
    }
}
