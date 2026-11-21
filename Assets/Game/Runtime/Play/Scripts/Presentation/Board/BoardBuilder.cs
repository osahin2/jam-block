using System;
using Game.Data.Levels;
using Game.Logic.Play;
using UnityEngine;

namespace Game.Play
{
    public sealed class BoardBuilder
    {
        private static readonly Side[] Sides = (Side[])Enum.GetValues(typeof(Side));
        private static readonly Vector2Int[] Corners =
        {
            new Vector2Int(1, 1), new Vector2Int(-1, 1), new Vector2Int(-1, -1), new Vector2Int(1, -1)
        };

        private readonly BoardArtSettings _art;
        private readonly ColorPalette _palette;
        private readonly BlockMotionSettings _motion;
        private readonly BlockViewBuilder _blockViews;

        public BoardBuilder(BoardArtSettings art, ColorPalette palette, BlockMotionSettings motion, BlockViewBuilder blockViews)
        {
            _art = art;
            _palette = palette;
            _motion = motion;
            _blockViews = blockViews;
        }

        public StagedLevel Build(LevelData level, BoardLayout layout, Transform parent, IBoardPieceSource pieces)
        {
            BuildGround(level, layout, parent, pieces);
            var doors = BuildFrame(level, layout, parent, pieces);
            return new StagedLevel(layout, BuildBlocks(level, layout, parent, pieces), doors);
        }

        private void BuildGround(LevelData level, BoardLayout layout, Transform parent, IBoardPieceSource pieces)
        {
            for (var y = 0; y < level.Height; y++)
            {
                for (var x = 0; x < level.Width; x++)
                    Spawn(_art.Ground, layout.CellCenter(new Cell(x, y)), Quaternion.identity, Vector3.one * _art.Scale, parent, pieces);
            }
            var baseSize = new Vector2(layout.Right * 2f + _art.WallWidth, layout.Top * 2f + _art.WallWidth) / _art.GroundTileSize;
            Spawn(_art.GroundBase, Vector3.zero, Quaternion.identity, new Vector3(baseSize.x, 1f, baseSize.y) * _art.Scale,
                parent, pieces);
        }

        private DoorView[] BuildFrame(LevelData level, BoardLayout layout, Transform parent, IBoardPieceSource pieces)
        {
            var doors = new DoorView[level.Doors.Count];
            foreach (var side in Sides)
                BuildSide(level, layout, side, parent, pieces, doors);

            foreach (var corner in Corners)
                BuildCorner(layout, corner, parent, pieces);
            return doors;
        }

        private void BuildCorner(BoardLayout layout, Vector2Int corner, Transform parent, IBoardPieceSource pieces)
        {
            var half = _art.WallWidth * 0.5f;
            var position = new Vector3(
                corner.x * (layout.Right + half), _art.FrameLift, corner.y * (layout.Top + half));
            var turn = Quaternion.Euler(0f, Mathf.Atan2(corner.x, corner.y) * Mathf.Rad2Deg - 45f, 0f);
            var scale = new Vector3(_art.WallWidth, _art.Scale, _art.WallWidth);
            Spawn(_art.FrameCorner, position, turn, scale, parent, pieces);
        }

        private void BuildSide(LevelData level, BoardLayout layout, Side side, Transform parent, IBoardPieceSource pieces,
            DoorView[] doors)
        {
            var outward = layout.ToLocal(Direction.OutOf(side));
            var turn = Quaternion.LookRotation(outward, Vector3.up);
            var wallOut = outward * (layout.CellSize + _art.WallWidth) * 0.5f;
            var doorOut = outward * (layout.CellSize + _art.DoorWidth) * 0.5f;
            var length = side.Length(level.Width, level.Height);
            var start = 0;
            while (start < length)
            {
                var door = DoorAt(level, side, start);
                var end = door?.End ?? NextDoorStart(level, side, start, length);
                var middle = RunMiddle(level, layout, side, start, end);
                var cells = end - start;
                if (door != null)
                {
                    var doorScale = new Vector3(cells * _art.DoorScalePerCell, _art.Scale, _art.DoorWidth);
                    var view = pieces.Take(_art.Door, parent);
                    Place(view.transform, middle + doorOut, turn, doorScale);
                    var arrowPosition = middle + doorOut;
                    arrowPosition.y = _art.FrameTop;
                    var arrow = Spawn(_art.DoorArrow, arrowPosition, turn, Vector3.one * _art.Scale, parent, pieces);
                    view.Init(_palette.ForDoor(door.ColorId), arrow.transform, _art.DoorDrop, _art.TouchHeight * 0.5f, _motion);
                    doors[level.Doors.IndexOf(door)] = view;
                }
                else
                {
                    var wallScale = new Vector3(cells * _art.WallScalePerCell, _art.Scale, _art.WallWidth);
                    Spawn(_art.Wall, middle + wallOut, turn, wallScale, parent, pieces);
                }
                start = end;
            }
        }

        private Vector3 RunMiddle(LevelData level, BoardLayout layout, Side side, int start, int end)
        {
            var first = layout.CellCenter(EdgeCell(level, side, start));
            var last = layout.CellCenter(EdgeCell(level, side, end - 1));
            return (first + last) * 0.5f + Lift(_art.FrameLift);
        }

        private BlockView[] BuildBlocks(LevelData level, BoardLayout layout, Transform parent, IBoardPieceSource pieces)
        {
            var views = new BlockView[level.Blocks.Count];
            for (var i = 0; i < views.Length; i++)
            {
                var block = level.Blocks[i];
                views[i] = _blockViews.Build(block, layout.CellCenter(block.Position), parent, pieces);
            }
            return views;
        }

        private static BoardPiece Spawn(BoardPiece prefab, Vector3 position, Quaternion rotation, Vector3 scale,
            Transform parent, IBoardPieceSource pieces)
        {
            var piece = pieces.Take(prefab, parent);
            Place(piece.transform, position, rotation, scale);
            return piece;
        }

        private static void Place(Transform transform, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            transform.SetLocalPositionAndRotation(position, rotation);
            transform.localScale = scale;
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

        private static int NextDoorStart(LevelData level, Side side, int from, int length)
        {
            var next = length;
            foreach (var door in level.Doors)
            {
                if (door.Side == side && door.Start >= from && door.Start < next)
                    next = door.Start;
            }
            return next;
        }

        private static Cell EdgeCell(LevelData level, Side side, int along)
        {
            var outward = Direction.OutOf(side).Offset;
            return side.IsHorizontal()
                ? new Cell(along, outward.Y < 0 ? 0 : level.Height - 1)
                : new Cell(outward.X < 0 ? 0 : level.Width - 1, along);
        }

        private static Vector3 Lift(float height) => new Vector3(0f, height, 0f);
    }
}
