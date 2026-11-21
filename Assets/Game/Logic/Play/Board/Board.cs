using System;
using System.Collections.Generic;
using Game.Data.Levels;

namespace Game.Logic.Play
{
    public sealed class Board : IBoard
    {
        public const int NoBlock = -1;

        private readonly int[] _occupant;
        private readonly List<BoardBlock> _blocks;
        private readonly DoorData[] _doors;
        private readonly IReadOnlyList<IMoveRule> _rules;
        private readonly IReadOnlyList<IBlockGuard> _guards;
        private readonly CellGroups _parts;

        public int Width { get; }
        public int Height { get; }
        public int RemainingBlocks { get; private set; }
        public int GoneCount { get; private set; }
        public bool IsCleared => RemainingBlocks == 0;
        public IReadOnlyList<BoardBlock> Blocks => _blocks;

        public Board(LevelData level, IReadOnlyList<IMoveRule> rules, IReadOnlyList<IBlockGuard> guards)
        {
            Width = level.Width;
            Height = level.Height;
            _rules = rules;
            _guards = guards;
            _occupant = new int[Width * Height];
            Array.Fill(_occupant, NoBlock);
            _doors = level.Doors.ToArray();
            _blocks = new List<BoardBlock>(CellsOf(level));
            for (var i = 0; i < level.Blocks.Count; i++)
            {
                var data = level.Blocks[i];
                _blocks.Add(new BoardBlock(data.ColorId, data.Position, data.Shape));
                SetOccupant(_blocks[i], i);
            }
            RemainingBlocks = _blocks.Count;
            _parts = new CellGroups(LargestBlockOf(level));
        }

        public int BlockAt(Cell cell) => Contains(cell) ? _occupant[IndexOf(cell)] : NoBlock;

        internal bool Contains(Cell cell) => cell.IsInside(Width, Height);

        public bool TryLeave(int block, out int exitDoor)
        {
            exitDoor = StepResult.NoDoor;
            if (_blocks[block].IsGone)
                return false;
            exitDoor = FindExitDoor(block);
            if (exitDoor == StepResult.NoDoor)
                return false;
            Remove(block);
            return true;
        }

        public void Hit(int block)
        {
            if (!_blocks[block].IsGone && !TryAbsorbHit(block))
                Remove(block);
        }

        public void HitCell(Cell cell)
        {
            var index = BlockAt(cell);
            if (index == NoBlock || TryAbsorbHit(index))
                return;
            var block = _blocks[index];
            if (block.CellCount == 1)
            {
                Remove(index);
                return;
            }
            _occupant[IndexOf(cell)] = NoBlock;
            block.RemoveCell(cell);
            Split(index);
        }

        public bool IsGuarded(int block)
        {
            for (var i = 0; i < _guards.Count; i++)
            {
                if (_guards[i].Guards(this, block))
                    return true;
            }
            return false;
        }

        public bool IsLocked(int block) =>
            !Allows(block, Direction.Up) && !Allows(block, Direction.Down) &&
            !Allows(block, Direction.Left) && !Allows(block, Direction.Right);

        public bool CanStep(int block, Direction direction)
        {
            var moving = _blocks[block];
            if (moving.IsGone || !Allows(block, direction))
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

        private void Remove(int index)
        {
            var block = _blocks[index];
            SetOccupant(block, NoBlock);
            block.IsGone = true;
            RemainingBlocks--;
            GoneCount++;
        }

        private void Split(int index)
        {
            var block = _blocks[index];
            _parts.Find(block.Shape);
            for (var part = 1; part < _parts.Count; part++)
            {
                var appended = new BoardBlock(block.ColorId, block.Position, _parts[part]);
                _blocks.Add(appended);
                SetOccupant(appended, _blocks.Count - 1);
            }
            RemainingBlocks += _parts.Count - 1;
            block.Reshape(_parts[0]);
        }

        private bool TryAbsorbHit(int block)
        {
            for (var i = 0; i < _guards.Count; i++)
            {
                if (!_guards[i].Guards(this, block))
                    continue;
                _guards[i].TakeHit(block);
                return true;
            }
            return false;
        }

        private bool Allows(int block, Direction direction)
        {
            for (var i = 0; i < _rules.Count; i++)
            {
                if (!_rules[i].Allows(this, block, direction))
                    return false;
            }
            return true;
        }

        private void SetOccupant(BoardBlock block, int occupant)
        {
            for (var i = 0; i < block.CellCount; i++)
                _occupant[IndexOf(block.CellAt(i))] = occupant;
        }

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
            if (!Allows(index, Direction.OutOf(door.Side)))
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

        private static int CellsOf(LevelData level)
        {
            var cells = 0;
            foreach (var block in level.Blocks)
                cells += block.Shape.Count;
            return cells;
        }

        private static int LargestBlockOf(LevelData level)
        {
            var largest = 0;
            foreach (var block in level.Blocks)
                largest = Math.Max(largest, block.Shape.Count);
            return largest;
        }

        private int IndexOf(Cell cell) => cell.IndexIn(Width);
    }
}
