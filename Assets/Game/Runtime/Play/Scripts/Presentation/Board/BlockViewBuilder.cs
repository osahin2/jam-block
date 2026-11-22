using System.Collections.Generic;
using UnityEngine;

namespace Game.Play
{
    public sealed class BlockViewBuilder
    {
        private readonly BoardArtSettings _art;
        private readonly ColorPalette _palette;
        private readonly BlockMotionSettings _motion;
        private readonly BlockPieceBuilder _pieces;
        private readonly IReadOnlyList<IBlockFeature> _features;

        public BlockViewBuilder(BoardArtSettings art, ColorPalette palette, BlockMotionSettings motion,
            BlockPieceBuilder pieces, IReadOnlyList<IBlockFeature> features)
        {
            _art = art;
            _palette = palette;
            _motion = motion;
            _pieces = pieces;
            _features = features;
        }

        public BlockView Build(BlockOnBoard block, Vector3 pose, Transform parent, IBoardPieceSource pieces)
        {
            var view = pieces.Take(_art.Block, parent);
            Dress(view, block, pieces);
            view.SetPose(pose);
            return view;
        }

        public void Rebuild(BlockView view, BlockOnBoard block, PooledBoardPieces pieces)
        {
            view.ReleasePieces(pieces);
            Dress(view, block, pieces);
        }

        public void CatchUpFeatures(BlockView view, BlockOnBoard block)
        {
            for (var i = 0; i < _features.Count; i++)
                _features[i].CatchUp(view, block);
        }

        private void Dress(BlockView view, BlockOnBoard block, IBoardPieceSource pieces)
        {
            BuildPieces(view, block, pieces);
            ShowFeatures(view, block);
        }

        private void BuildPieces(BlockView view, BlockOnBoard block, IBoardPieceSource pieces)
        {
            var shape = block.Block.Shape;
            view.Init(_motion, shape);
            _pieces.Build(view, shape, _palette.ForBlock(block.Block.ColorId), pieces);
        }

        private void ShowFeatures(BlockView view, BlockOnBoard block)
        {
            for (var i = 0; i < _features.Count; i++)
                _features[i].Show(view, block);
        }
    }
}
