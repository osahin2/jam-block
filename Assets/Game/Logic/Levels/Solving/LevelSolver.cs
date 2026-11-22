using System;
using System.Collections.Generic;
using Game.Data.Levels;
using Game.Logic.Play;

namespace Game.Logic.Levels
{
    public sealed class LevelSolver
    {
        private const int NoParent = -1;

        private readonly Board _board;
        private readonly Func<int, bool> _keepSearching;
        private readonly InterchangeableBlocks _alike;
        private readonly ArrangementKey _probe;
        private readonly BlockReach _reach = new BlockReach();
        private readonly List<SearchNode> _nodes = new List<SearchNode>();
        private readonly HashSet<ArrangementKey> _seen = new HashSet<ArrangementKey>();
        private readonly Queue<int> _frontier = new Queue<int>();
        private readonly List<SolutionMove> _moves = new List<SolutionMove>();
        private readonly List<Cell> _places = new List<Cell>();

        private LevelSolver(LevelData level, Func<int, bool> keepSearching)
        {
            var mechanics = LevelMechanics.For(level);
            _board = new Board(level, mechanics.Rules, mechanics.Guards);
            _keepSearching = keepSearching;
            _alike = new InterchangeableBlocks(level);
            _probe = new ArrangementKey(_alike.BlockCount);
        }

        public static LevelSolution Solve(LevelData level, int stateBudget) =>
            Solve(level, searched => searched < stateBudget);

        public static LevelSolution Solve(LevelData level, Func<int, bool> keepSearching) =>
            BlocksCanLeaveCheck.EveryBlockFitsADoor(level)
                ? new LevelSolver(level, keepSearching).Search()
                : LevelSolution.Unsolvable(0);

        private LevelSolution Search()
        {
            LeaveAll();
            TryAdd(NoParent);
            if (_board.IsCleared)
                return Solved();
            while (_frontier.Count > 0)
            {
                if (TryExpand(_frontier.Dequeue(), out var solution))
                    return solution;
            }
            return LevelSolution.Unsolvable(_seen.Count);
        }

        private bool TryExpand(int parent, out LevelSolution solution)
        {
            solution = null;
            var arrangement = _nodes[parent].Arrangement;
            for (var block = 0; block < _board.Blocks.Count; block++)
            {
                if (TryRearrange(parent, arrangement, block, out solution))
                    return true;
            }
            return false;
        }

        private bool TryRearrange(int parent, BoardArrangement arrangement, int block, out LevelSolution solution)
        {
            solution = null;
            _board.Arrange(arrangement);
            if (_board.Blocks[block].IsGone)
                return false;
            _reach.Find(_board, block);
            _places.Clear();
            _places.AddRange(_reach.Places);
            foreach (var place in _places)
            {
                _board.Arrange(arrangement);
                _board.Place(block, place);
                _moves.Add(SolutionMove.To(block, place));
                LeaveAllBut(block);
                if (TryAdd(parent) && _board.IsCleared)
                    solution = Solved();
                else if (!_keepSearching(_seen.Count))
                    solution = LevelSolution.Unknown(_seen.Count);
                if (solution != null)
                    return true;
            }
            return false;
        }

        private void LeaveAllBut(int moved)
        {
            for (var block = 0; block < _board.Blocks.Count; block++)
            {
                if (block == moved || !TryLeave(block))
                    continue;
                LeaveAll();
                return;
            }
        }

        private void LeaveAll()
        {
            var anyLeft = true;
            while (anyLeft)
            {
                anyLeft = false;
                for (var block = 0; block < _board.Blocks.Count; block++)
                    anyLeft |= TryLeave(block);
            }
        }

        private bool TryLeave(int block)
        {
            if (_board.Blocks[block].IsGone)
                return false;
            _reach.Find(_board, block);
            if (!_reach.CanLeave)
                return false;
            _board.Place(block, _reach.ExitPlace);
            _board.TryLeave(block, out _);
            _moves.Add(SolutionMove.LeaveFrom(block, _reach.ExitPlace));
            return true;
        }

        private bool TryAdd(int parent)
        {
            _probe.Read(_alike, _board.Blocks);
            var isNew = !_seen.Contains(_probe);
            if (isNew)
            {
                _seen.Add(_probe.Copy());
                _nodes.Add(new SearchNode(_board.CurrentArrangement(), parent, _moves.ToArray()));
                _frontier.Enqueue(_nodes.Count - 1);
            }
            _moves.Clear();
            return isNew;
        }

        private LevelSolution Solved()
        {
            var path = new List<IReadOnlyList<SolutionMove>>();
            for (var node = _nodes.Count - 1; node != NoParent; node = _nodes[node].Parent)
                path.Add(_nodes[node].Moves);
            var moves = new List<SolutionMove>();
            for (var i = path.Count - 1; i >= 0; i--)
                moves.AddRange(path[i]);
            return LevelSolution.Solved(moves, _seen.Count);
        }

        private readonly struct SearchNode
        {
            public BoardArrangement Arrangement { get; }
            public int Parent { get; }
            public IReadOnlyList<SolutionMove> Moves { get; }

            public SearchNode(BoardArrangement arrangement, int parent, IReadOnlyList<SolutionMove> moves)
            {
                Arrangement = arrangement;
                Parent = parent;
                Moves = moves;
            }
        }
    }
}
