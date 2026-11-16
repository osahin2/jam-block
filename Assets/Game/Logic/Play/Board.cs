using System;
using System.Collections.Generic;
using Game.Data.Levels;

namespace Game.Logic.Play
{
    public sealed class Board
    {
        private const int Empty = -1;

        private readonly int[] _occupant;
        private readonly BoardBlock[] _blocks;
        private readonly DoorData[] _doors;

        public int Width { get; }
        public int Height { get; }
        public int RemainingBlocks { get; private set; }
        public bool IsCleared => RemainingBlocks == 0;
        public IReadOnlyList<BoardBlock> Blocks => _blocks;

        public Board(LevelData level)
        {
            Width = level.Width;
            Height = level.Height;
            _occupant = new int[Width * Height];
            Array.Fill(_occupant, Empty);
            _doors = level.Doors.ToArray();
            foreach (var door in _doors)
                CheckDoor(door);

            _blocks = new BoardBlock[level.Blocks.Count];
            for (var i = 0; i < _blocks.Length; i++)
            {
                _blocks[i] = new BoardBlock(level.Blocks[i]);
                Place(i);
            }
            RemainingBlocks = _blocks.Length;
        }

        public bool CanStep(int block, Direction direction)
        {
            var moving = _blocks[block];
            if (moving.HasLeft)
                return false;
            for (var i = 0; i < moving.CellCount; i++)
            {
                var target = moving.CellAt(i) + direction.Offset;
                if (!Contains(target))
                    return false;
                var occupant = _occupant[IndexOf(target)];
                if (occupant != Empty && occupant != block)
                    return false;
            }
            return true;
        }

        public StepResult Step(int block, Direction direction)
        {
            if (!CanStep(block, direction))
                throw new InvalidOperationException($"Block {block} cannot step by {direction.Offset}.");

            var moving = _blocks[block];
            SetOccupant(moving, Empty);
            moving.Position += direction.Offset;
            var exitDoor = FindExitDoor(moving);
            if (exitDoor == StepResult.NoDoor)
                SetOccupant(moving, block);
            else
                Remove(moving);
            return new StepResult(moving.Position, exitDoor);
        }

        private void Place(int block)
        {
            var placed = _blocks[block];
            for (var i = 0; i < placed.CellCount; i++)
            {
                var cell = placed.CellAt(i);
                if (!Contains(cell))
                    throw new ArgumentException($"Block {block} has cell {cell} outside the {Width} x {Height} board.");
                if (_occupant[IndexOf(cell)] != Empty)
                    throw new ArgumentException($"Block {block} overlaps block {_occupant[IndexOf(cell)]} at {cell}.");
                _occupant[IndexOf(cell)] = block;
            }
        }

        private void Remove(BoardBlock block)
        {
            block.HasLeft = true;
            RemainingBlocks--;
        }

        private void SetOccupant(BoardBlock block, int occupant)
        {
            for (var i = 0; i < block.CellCount; i++)
                _occupant[IndexOf(block.CellAt(i))] = occupant;
        }

        // A block leaves through a door of its color on a side it touches, when the door spans the
        // block's whole extent along that side.
        private int FindExitDoor(BoardBlock block)
        {
            for (var i = 0; i < _doors.Length; i++)
            {
                var door = _doors[i];
                if (door.ColorId == block.ColorId && Touches(block, door.Side) && Covers(door, block))
                    return i;
            }
            return StepResult.NoDoor;
        }

        private bool Touches(BoardBlock block, Side side)
        {
            var outward = Direction.OutOf(side).Offset;
            for (var i = 0; i < block.CellCount; i++)
            {
                if (!Contains(block.CellAt(i) + outward))
                    return true;
            }
            return false;
        }

        private static bool Covers(DoorData door, BoardBlock block)
        {
            var outward = Direction.OutOf(door.Side).Offset;
            var end = door.Start + door.Length;
            for (var i = 0; i < block.CellCount; i++)
            {
                var along = AlongSide(block.CellAt(i), outward);
                if (along < door.Start || along >= end)
                    return false;
            }
            return true;
        }

        // A side runs across its outward direction: top and bottom along columns, left and right along rows.
        private static int AlongSide(Cell cell, Cell outward) => outward.X == 0 ? cell.X : cell.Y;

        private void CheckDoor(DoorData door)
        {
            var sideLength = Direction.OutOf(door.Side).Offset.X == 0 ? Width : Height;
            if (door.Length < 1 || door.Start < 0 || door.Start + door.Length > sideLength)
                throw new ArgumentException($"Door on {door.Side} from {door.Start} with length {door.Length} does not fit a side of {sideLength} cells.");
        }

        private bool Contains(Cell cell) => cell.X >= 0 && cell.X < Width && cell.Y >= 0 && cell.Y < Height;

        private int IndexOf(Cell cell) => cell.Y * Width + cell.X;
    }
}
