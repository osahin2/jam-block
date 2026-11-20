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

        internal bool Contains(Cell cell) => cell.IsInside(Width, Height);

        // A block that leaves is taken off the board, so it never leaves twice.
        public bool TryLeave(int block, out int exitDoor)
        {
            exitDoor = StepResult.NoDoor;
            var leaving = _blocks[block];
            if (leaving.HasLeft)
                return false;
            exitDoor = FindExitDoor(block);
            if (exitDoor == StepResult.NoDoor)
                return false;
            SetOccupant(leaving, NoBlock);
            Remove(leaving);
            return true;
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
            SetOccupant(moving, block);
            TryLeave(block, out var exitDoor);
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

        // The first matching door in level order.
        private int FindExitDoor(int index)
        {
            for (var i = 0; i < _doors.Length; i++)
            {
                if (LeavesThrough(index, _doors[i]))
                    return i;
            }
            return StepResult.NoDoor;
        }

        private bool LeavesThrough(int index, DoorData door)
        {
            var block = _blocks[index];
            if (door.ColorId != block.ColorId)
                return false;
            if (!Touches(block, door.Side))
                return false;
            if (!Covers(door, block))
                return false;
            return IsWayOutFree(index, door.Side);
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

        // The block slides out straight, so every cell from each of its cells to the side must be free or its own.
        private bool IsWayOutFree(int index, Side side)
        {
            var block = _blocks[index];
            var outward = Direction.OutOf(side).Offset;
            for (var i = 0; i < block.CellCount; i++)
            {
                for (var cell = block.CellAt(i) + outward; Contains(cell); cell += outward)
                {
                    var occupant = _occupant[IndexOf(cell)];
                    if (occupant != NoBlock && occupant != index)
                        return false;
                }
            }
            return true;
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

        private int IndexOf(Cell cell) => cell.IndexIn(Width);
    }
}
