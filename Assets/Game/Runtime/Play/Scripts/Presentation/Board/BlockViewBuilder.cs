using System.Collections.Generic;
using Game.Data.Levels;
using Game.Logic.Play;
using UnityEngine;

namespace Game.Play
{
    public sealed class BlockViewBuilder
    {
        private readonly BoardArtSettings _art;
        private readonly ColorPalette _palette;
        private readonly BlockMotionSettings _motion;
        private readonly BlockPieceBuilder _pieces;

        public BlockViewBuilder(BoardArtSettings art, ColorPalette palette, BlockMotionSettings motion, BlockPieceBuilder pieces)
        {
            _art = art;
            _palette = palette;
            _motion = motion;
            _pieces = pieces;
        }

        public BlockView Build(int colorId, IReadOnlyList<Cell> shape, Vector3 pose, Transform parent,
            IBoardPieceSource pieces)
        {
            var view = pieces.Take(_art.Block, parent);
            BuildPieces(view, colorId, shape, pieces);
            view.SetPose(pose);
            return view;
        }

        public void Rebuild(BlockView view, BoardBlock block, PooledBoardPieces pieces)
        {
            view.ReleasePieces(pieces);
            BuildPieces(view, block.ColorId, block.Shape, pieces);
        }

        private void BuildPieces(BlockView view, int colorId, IReadOnlyList<Cell> shape, IBoardPieceSource pieces)
        {
            view.Init(_motion, shape);
            _pieces.Build(view, shape, _palette.ForBlock(colorId), pieces);
        }
    }
}
