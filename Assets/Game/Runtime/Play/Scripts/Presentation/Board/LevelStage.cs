using Game.Data.Levels;
using UnityEngine;

namespace Game.Play
{
    // Shows a level: lays the board out, builds it under the given parent from the given pieces and frames the camera
    // on it. The play session and the level editor's scene preview both show a level through this one method, so the
    // preview matches the game; each brings its own parent and pieces, and releases them itself. It makes its builder
    // and framing from the settings, so a caller outside the play feature knows only this class and the settings.
    public sealed class LevelStage
    {
        private readonly BoardArtSettings _art;
        private readonly BoardBuilder _builder;
        private readonly CameraFraming _framing;

        public LevelStage(BoardArtSettings art, ColorPalette palette, BlockMotionSettings motion, CameraFramingSettings framing)
        {
            _art = art;
            _builder = new BoardBuilder(art, palette, motion, new BlockPieceBuilder(art));
            _framing = new CameraFraming(art, framing);
        }

        public StagedLevel Show(LevelData level, Transform parent, IBoardPieceSource pieces, Camera camera)
        {
            var layout = new BoardLayout(level.Width, level.Height, _art.CellSize);
            var blocks = _builder.Build(level, layout, parent, pieces);
            _framing.Frame(camera, layout, parent);
            return new StagedLevel(layout, blocks);
        }
    }
}
