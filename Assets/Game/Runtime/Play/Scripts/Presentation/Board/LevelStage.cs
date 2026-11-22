using System.Collections.Generic;
using Game.Data.Levels;
using Game.Logic.Play;
using UnityEngine;

namespace Game.Play
{
    public sealed class LevelStage
    {
        private readonly BoardArtSettings _art;
        private readonly BoardBuilder _builder;
        private readonly CameraFraming _framing;

        public LevelStage(BoardArtSettings art, ColorPalette palette, BlockMotionSettings motion,
            IReadOnlyList<IBlockFeature> features, CameraFramingSettings framing)
        {
            _art = art;
            var blockViews = new BlockViewBuilder(art, palette, motion, new BlockPieceBuilder(art), features);
            _builder = new BoardBuilder(art, palette, motion, blockViews);
            _framing = new CameraFraming(framing);
        }

        public StagedLevel Show(LevelData level, LevelAttempt attempt, Transform parent, IBoardPieceSource pieces,
            Camera camera)
        {
            var layout = new BoardLayout(level.Width, level.Height, _art);
            var staged = _builder.Build(level, attempt, layout, parent, pieces);
            BoardClip.Set(layout, parent);
            _framing.Frame(camera, layout, parent, ViewportSafeArea());
            return staged;
        }

        private static Rect ViewportSafeArea()
        {
            var screen = new Vector2(Screen.width, Screen.height);
            var safeArea = Screen.safeArea;
            return new Rect(safeArea.position / screen, safeArea.size / screen);
        }
    }
}
