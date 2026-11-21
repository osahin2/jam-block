using System.Collections.Generic;
using Game.Data.Levels;
using Game.Logic.Play;
using UnityEngine;

namespace Game.Play
{
    public sealed class BlockViewBuilder
    {
        private static readonly ArrowData NoArrow = new ArrowData();

        private readonly BoardArtSettings _art;
        private readonly ColorPalette _palette;
        private readonly BlockMotionSettings _motion;
        private readonly IceSettings _ice;
        private readonly BlockPieceBuilder _pieces;

        public BlockViewBuilder(BoardArtSettings art, ColorPalette palette, BlockMotionSettings motion, IceSettings ice,
            BlockPieceBuilder pieces)
        {
            _art = art;
            _palette = palette;
            _motion = motion;
            _ice = ice;
            _pieces = pieces;
        }

        public BlockView Build(BlockData block, Vector3 pose, Transform parent, IBoardPieceSource pieces) =>
            Build(block.ColorId, block.Shape, block.Ice, block.Arrow, pose, parent, pieces);

        public BlockView Build(BoardBlock block, Vector3 pose, Transform parent, IBoardPieceSource pieces) =>
            Build(block.ColorId, block.Shape, 0, NoArrow, pose, parent, pieces);

        public void Rebuild(BlockView view, BoardBlock block, PooledBoardPieces pieces)
        {
            view.ReleasePieces(pieces);
            var material = BuildPieces(view, block.ColorId, block.Shape, pieces);
            view.Ice.Show(0, block.Shape, material, _art);
        }

        private BlockView Build(int colorId, IReadOnlyList<Cell> shape, int ice, ArrowData arrow, Vector3 pose,
            Transform parent, IBoardPieceSource pieces)
        {
            var view = pieces.Take(_art.Block, parent);
            var material = BuildPieces(view, colorId, shape, pieces);
            view.Ice.Show(ice, shape, material, _art);
            view.Arrow.Show(arrow, _art);
            view.SetPose(pose);
            return view;
        }

        private Material BuildPieces(BlockView view, int colorId, IReadOnlyList<Cell> shape, IBoardPieceSource pieces)
        {
            var material = _palette.ForBlock(colorId);
            view.Init(_motion, _ice, shape);
            _pieces.Build(view, shape, material, pieces);
            return material;
        }
    }
}
