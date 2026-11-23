using Game.Data.Levels;
using Game.Play;
using UnityEngine;

namespace Game.Editor.LevelEditor
{
    public sealed class EditorPalette
    {
        private static readonly Color MissingColor = Color.magenta;

        private readonly ColorPalette _palette;

        public bool Exists => _palette != null;
        public int Count => _palette != null ? _palette.Count : 0;

        public EditorPalette(ColorPalette palette)
        {
            _palette = palette;
        }

        public bool Has(int colorId) => _palette != null && _palette.Has(colorId);

        public Color ColorOf(int colorId)
        {
            var material = MaterialOf(colorId);
            return material != null ? material.color : MissingColor;
        }

        public string ColorName(int colorId)
        {
            var material = MaterialOf(colorId);
            return material != null ? material.name.Substring(material.name.LastIndexOf('_') + 1) : $"Missing color {colorId}";
        }

        public string NameOf(ILevelSelection piece, LevelData level) => $"{ColorName(piece.ColorId(level))} {piece.Describe(level)}";

        public string BlockName(LevelData level, int block) => NameOf(new BlockSelection(level.Blocks[block]), level);

        private Material MaterialOf(int colorId) => Has(colorId) ? _palette.ForBlock(colorId) : null;
    }
}
