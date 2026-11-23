using System.Collections.Generic;
using Game.Data.Levels;
using Game.Logic.Levels;
using Game.Logic.Play;

namespace Game.Editor.LevelEditor
{
    public sealed class SolutionPlayback
    {
        private readonly LevelData _level;
        private readonly IReadOnlyList<SolutionMove> _moves;
        private readonly Cell[] _positions;
        private readonly bool[] _gone;

        public int Step { get; private set; }
        public int MoveCount => _moves.Count;
        public bool IsAtStart => Step == 0;
        public bool IsAtEnd => Step == _moves.Count;
        public SolutionMove NextMove => _moves[Step];
        public LevelData Shown { get; private set; }

        public SolutionPlayback(LevelData level, LevelSolution solution)
        {
            _level = LevelCopy.Of(level);
            _moves = solution.Moves;
            _positions = new Cell[_level.Blocks.Count];
            _gone = new bool[_level.Blocks.Count];
            ShowStep(0);
        }

        public void Next()
        {
            if (!IsAtEnd)
                ShowStep(Step + 1);
        }

        public void Previous()
        {
            if (!IsAtStart)
                ShowStep(Step - 1);
        }

        public List<Cell> NextBlockCells() => CellsOf(NextMove.Block, _positions[NextMove.Block]);

        public List<Cell> NextTargetCells() => CellsOf(NextMove.Block, NextMove.Position);

        private void ShowStep(int step)
        {
            Step = step;
            for (var i = 0; i < _positions.Length; i++)
            {
                _positions[i] = _level.Blocks[i].Position;
                _gone[i] = false;
            }
            var goneCount = 0;
            for (var i = 0; i < step; i++)
            {
                var move = _moves[i];
                _positions[move.Block] = move.Position;
                _gone[move.Block] = move.Leaves;
                goneCount += move.Leaves ? 1 : 0;
            }
            Shown = LevelCopy.Of(_level);
            for (var i = _positions.Length - 1; i >= 0; i--)
            {
                if (_gone[i])
                {
                    Shown.Blocks.RemoveAt(i);
                    continue;
                }
                var block = Shown.Blocks[i];
                block.Position = _positions[i];
                block.Ice = IceMechanic.IceAfter(block.Ice, goneCount);
            }
        }

        private List<Cell> CellsOf(int block, Cell position)
        {
            var cells = new List<Cell>();
            foreach (var offset in _level.Blocks[block].Shape)
                cells.Add(position + offset);
            return cells;
        }
    }
}
