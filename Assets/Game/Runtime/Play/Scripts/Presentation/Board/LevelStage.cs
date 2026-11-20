using Game.Data.Levels;
using UnityEngine;

namespace Game.Play
{
    public sealed class LevelStage
    {
        private readonly BoardArtSettings _art;
        private readonly BoardBuilder _builder;
        private readonly CameraFraming _framing;

        public LevelStage(BoardArtSettings art, ColorPalette palette, BlockMotionSettings motion, CameraFramingSettings framing)
        {
            _art = art;
            _builder = new BoardBuilder(art, palette, motion, new BlockPieceBuilder(art));
            _framing = new CameraFraming(framing);
        }

        public StagedLevel Show(LevelData level, Transform parent, IBoardPieceSource pieces, Camera camera)
        {
            var layout = new BoardLayout(level.Width, level.Height, _art);
            var staged = _builder.Build(level, layout, parent, pieces);
            BoardClip.Set(layout, parent);
            _framing.Frame(camera, layout, parent);
            return staged;
        }
    }
}
