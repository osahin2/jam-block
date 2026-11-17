using System;
using System.Collections.Generic;
using Game.Data.Levels;

namespace Game.Logic.Play
{
    public sealed class Board
    {
        public const int NoBlock = -1;

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
            Array.Fill(_occupant, NoBlock);
            _doors = level.Doors.ToArray();
            _blocks = new BoardBlock[level.Blocks.Count];
            for (var i = 0; i < _blocks.Length; i++)
            {
                _blocks[i] = new BoardBlock(level.Blocks[i]);
                SetOccupant(_blocks[i], i);
            }
            RemainingBlocks = _blocks.Length;
        }

        public int BlockAt(Cell cell) => Contains(cell) ? _occupant[IndexOf(cell)] : NoBlock;

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
                if (occupant != NoBlock && occupant != block)
                    return false;
            }
            return true;
        }

        public StepResult Step(int block, Direction direction)
        {
            var moving = _blocks[block];
            if (!CanStep(block, direction))
                return new StepResult(false, moving.Position, StepResult.NoDoor);

            SetOccupant(moving, NoBlock);
            moving.Position += direction.Offset;
            var exitDoor = FindExitDoor(moving);
            if (exitDoor == StepResult.NoDoor)
                SetOccupant(moving, block);
            else
                Remove(moving);
            return new StepResult(true, moving.Position, exitDoor);
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
            for (var i = 0; i < block.CellCount; i++)
            {
                if (!door.Covers(door.Side.Along(block.CellAt(i))))
                    return false;
            }
            return true;
        }

        private bool Contains(Cell cell) => cell.X >= 0 && cell.X < Width && cell.Y >= 0 && cell.Y < Height;

        private int IndexOf(Cell cell) => cell.Y * Width + cell.X;
    }
}
