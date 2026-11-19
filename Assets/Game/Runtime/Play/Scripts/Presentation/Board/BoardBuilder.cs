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
        // The board's corners as signs on X and Z; +Z is the far edge.
        private static readonly Vector2Int[] Corners =
        {
            new Vector2Int(1, 1), new Vector2Int(-1, 1), new Vector2Int(-1, -1), new Vector2Int(1, -1)
        };

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
                    Spawn(_art.Ground, layout.CellCenter(new Cell(x, y)), Quaternion.identity, Vector3.one * _art.Scale, parent, pieces);
            }
        }

        private void BuildFrame(LevelData level, BoardLayout layout, Transform parent, IBoardPieceSource pieces)
        {
            foreach (var side in Sides)
                BuildSide(level, layout, side, parent, pieces);

            foreach (var corner in Corners)
                BuildCorner(layout, corner, parent, pieces);
        }

        // A corner piece is a square of the wall's thickness outside the board's corner, turned so its rounded corner
        // points away from the board.
        private void BuildCorner(BoardLayout layout, Vector2Int corner, Transform parent, IBoardPieceSource pieces)
        {
            var half = _art.WallWidth * 0.5f;
            var position = new Vector3(
                corner.x * (layout.Right + half), _art.FrameLift, corner.y * (layout.Top + half));
            var turn = Quaternion.Euler(0f, Mathf.Atan2(corner.x, corner.y) * Mathf.Rad2Deg - 45f, 0f);
            var scale = new Vector3(_art.WallWidth, _art.Scale, _art.WallWidth);
            Spawn(_art.FrameCorner, position, turn, scale, parent, pieces);
        }

        // Each cell along a side gets a door piece when a door covers it, else two half-cell wall pieces. Frame pieces
        // face out along their +Z, so each is turned toward the side's outward direction and its Z scale is its
        // thickness; its inner face lies on the board's edge.
        private void BuildSide(LevelData level, BoardLayout layout, Side side, Transform parent, IBoardPieceSource pieces)
        {
            var outward = Direction.OutOf(side).Offset;
            var outwardLocal = layout.ToLocal(Direction.OutOf(side));
            var along = side.IsHorizontal() ? Vector3.right : Vector3.forward;
            var turn = Quaternion.LookRotation(outwardLocal, Vector3.up);
            var wallOut = outwardLocal * (layout.CellSize + _art.WallWidth) * 0.5f;
            var doorOut = outwardLocal * (layout.CellSize + _art.DoorWidth) * 0.5f;
            var wallAlong = along * (layout.CellSize * 0.25f);
            var wallScale = new Vector3(_art.Scale, _art.Scale, _art.WallWidth);
            var doorScale = new Vector3(_art.Scale, _art.Scale, _art.DoorWidth);
            for (var i = 0; i < side.Length(level.Width, level.Height); i++)
            {
                var edgeCell = side.IsHorizontal()
                    ? new Cell(i, outward.Y < 0 ? 0 : level.Height - 1)
                    : new Cell(outward.X < 0 ? 0 : level.Width - 1, i);
                var edge = layout.CellCenter(edgeCell) + Lift(_art.FrameLift);
                var door = DoorAt(level, side, i);
                if (door != null)
                {
                    var piece = Spawn(_art.Door, edge + doorOut, turn, doorScale, parent, pieces);
                    piece.SetMaterial(_palette.For(door.ColorId));
                    continue;
                }
                Spawn(_art.Wall, edge + wallOut - wallAlong, turn, wallScale, parent, pieces);
                Spawn(_art.Wall, edge + wallOut + wallAlong, turn, wallScale, parent, pieces);
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

        private static BoardPiece Spawn(BoardPiece prefab, Vector3 position, Quaternion rotation, Vector3 scale,
            Transform parent, IBoardPieceSource pieces)
        {
            var piece = pieces.Take(prefab, parent);
            var transform = piece.transform;
            transform.SetLocalPositionAndRotation(position, rotation);
            transform.localScale = scale;
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
