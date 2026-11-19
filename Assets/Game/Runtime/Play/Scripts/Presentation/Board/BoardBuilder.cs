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
        // Frame pieces have their visible cap facing -Y.
        private static readonly Quaternion CapUp = Quaternion.Euler(180f, 0f, 0f);
        // The door piece is long along X; on the left and right sides it is turned to run along Z.
        private static readonly Quaternion AlongZ = Quaternion.Euler(0f, 90f, 0f);

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
            Wall(new Vector3(layout.Left - half, 0f, layout.Top + half), parent, pieces);
            Wall(new Vector3(layout.Right + half, 0f, layout.Top + half), parent, pieces);
            Wall(new Vector3(layout.Left - half, 0f, layout.Bottom - half), parent, pieces);
            Wall(new Vector3(layout.Right + half, 0f, layout.Bottom - half), parent, pieces);
        }

        // Each cell along a side gets a door piece when a door covers it, else two half-cell wall pieces.
        private void BuildSide(LevelData level, BoardLayout layout, Side side, Transform parent, IBoardPieceSource pieces)
        {
            var outward = Direction.OutOf(side).Offset;
            var outwardLocal = layout.ToLocal(Direction.OutOf(side));
            var along = side.IsHorizontal() ? Vector3.right : Vector3.forward;
            var turn = side.IsHorizontal() ? Quaternion.identity : AlongZ;
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
                    var piece = Spawn(_art.Door, center + Lift(_art.FrameLift), turn * CapUp, parent, pieces);
                    piece.sharedMaterial = _palette.For(door.ColorId);
                    continue;
                }
                Wall(center - wallOffset, parent, pieces);
                Wall(center + wallOffset, parent, pieces);
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

        private void Wall(Vector3 position, Transform parent, IBoardPieceSource pieces) =>
            Spawn(_art.Wall, position + Lift(_art.FrameLift), CapUp, parent, pieces);

        private MeshRenderer Spawn(MeshRenderer prefab, Vector3 position, Quaternion rotation, Transform parent, IBoardPieceSource pieces)
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
