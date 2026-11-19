using System;
using Game.Data.Levels;
using Game.Logic.Play;
using UnityEngine;

namespace Game.Play
{
    // Builds a level's ground, frame, doors and blocks under the parent it is given, from the pieces of the source it
    // is given. It owns neither: the caller creates the parent and releases the pieces.
    public sealed class BoardBuilder
    {
        private static readonly Side[] Sides = (Side[])Enum.GetValues(typeof(Side));

        private readonly BoardArtSettings _art;
        private readonly ColorPalette _palette;
        private readonly BlockMotionSettings _motion;
        private readonly BlockPieceBuilder _blockPieces;

        public BoardBuilder(BoardArtSettings art, ColorPalette palette, BlockMotionSettings motion, BlockPieceBuilder blockPieces)
        {
            _art = art;
            _palette = palette;
            _motion = motion;
            _blockPieces = blockPieces;
        }

        // Every piece is placed in the parent's local space, so the board follows the parent it is built under.
        public BlockView[] Build(LevelData level, BoardLayout layout, Transform parent, IBoardPieceSource pieces)
        {
            BuildGround(level, layout, parent, pieces);
            BuildFrame(level, layout, parent, pieces);
            return BuildBlocks(level, layout, parent, pieces);
        }

        private void BuildGround(LevelData level, BoardLayout layout, Transform parent, IBoardPieceSource pieces)
        {
            for (var y = 0; y < level.Height; y++)
            {
                for (var x = 0; x < level.Width; x++)
                    Spawn(_art.Ground, layout.CellCenter(new Cell(x, y)), Quaternion.identity, parent, pieces);
            }
        }

        private void BuildFrame(LevelData level, BoardLayout layout, Transform parent, IBoardPieceSource pieces)
        {
            foreach (var side in Sides)
                BuildSide(level, layout, side, parent, pieces);

            var half = _art.FrameWidth * 0.5f;
            Wall(new Vector3(layout.Left - half, 0f, layout.Top + half), Quaternion.identity, parent, pieces);
            Wall(new Vector3(layout.Right + half, 0f, layout.Top + half), Quaternion.identity, parent, pieces);
            Wall(new Vector3(layout.Left - half, 0f, layout.Bottom - half), Quaternion.identity, parent, pieces);
            Wall(new Vector3(layout.Right + half, 0f, layout.Bottom - half), Quaternion.identity, parent, pieces);
        }

        // Each cell along a side gets a door piece when a door covers it, else two half-cell wall pieces. Frame pieces
        // face out along their +Z, so each is turned toward the side's outward direction.
        private void BuildSide(LevelData level, BoardLayout layout, Side side, Transform parent, IBoardPieceSource pieces)
        {
            var outward = Direction.OutOf(side).Offset;
            var outwardLocal = layout.ToLocal(Direction.OutOf(side));
            var along = side.IsHorizontal() ? Vector3.right : Vector3.forward;
            var turn = Quaternion.LookRotation(outwardLocal, Vector3.up);
            var frameOffset = outwardLocal * (layout.CellSize * 0.5f + _art.FrameWidth * 0.5f);
            var wallOffset = along * (_art.FrameWidth * 0.5f);
            for (var i = 0; i < side.Length(level.Width, level.Height); i++)
            {
                var edgeCell = side.IsHorizontal()
                    ? new Cell(i, outward.Y < 0 ? 0 : level.Height - 1)
                    : new Cell(outward.X < 0 ? 0 : level.Width - 1, i);
                var center = layout.CellCenter(edgeCell) + frameOffset;
                var door = DoorAt(level, side, i);
                if (door != null)
                {
                    var piece = Spawn(_art.Door, center + Lift(_art.FrameLift), turn, parent, pieces);
                    piece.SetMaterial(_palette.For(door.ColorId));
                    continue;
                }
                Wall(center - wallOffset, turn, parent, pieces);
                Wall(center + wallOffset, turn, parent, pieces);
            }
        }

        private BlockView[] BuildBlocks(LevelData level, BoardLayout layout, Transform parent, IBoardPieceSource pieces)
        {
            var views = new BlockView[level.Blocks.Count];
            for (var i = 0; i < views.Length; i++)
            {
                var block = level.Blocks[i];
                var view = pieces.Take(_art.Block, parent);
                view.Init(_motion);
                _blockPieces.Build(view.transform, block.Shape, _palette.For(block.ColorId), pieces);
                view.SetPose(layout.CellCenter(block.Position));
                views[i] = view;
            }
            return views;
        }

        private void Wall(Vector3 position, Quaternion rotation, Transform parent, IBoardPieceSource pieces) =>
            Spawn(_art.Wall, position + Lift(_art.FrameLift), rotation, parent, pieces);

        private BoardPiece Spawn(BoardPiece prefab, Vector3 position, Quaternion rotation, Transform parent, IBoardPieceSource pieces)
        {
            var piece = pieces.Take(prefab, parent);
            var transform = piece.transform;
            transform.SetLocalPositionAndRotation(position, rotation);
            transform.localScale = Vector3.one * _art.Scale;
            return piece;
        }

        private static DoorData DoorAt(LevelData level, Side side, int index)
        {
            foreach (var door in level.Doors)
            {
                if (door.Side == side && door.Covers(index))
                    return door;
            }
            return null;
        }

        private static Vector3 Lift(float height) => new Vector3(0f, height, 0f);
    }
}
