using System;
using System.Collections.Generic;

namespace Game.Logic.Levels
{
    public sealed class LevelSolution
    {
        public Solvability Verdict { get; }
        public IReadOnlyList<SolutionMove> Moves { get; }
        public int ExploredStates { get; }
        public int Rearrangements { get; }

        private LevelSolution(Solvability verdict, IReadOnlyList<SolutionMove> moves, int exploredStates)
        {
            Verdict = verdict;
            Moves = moves;
            ExploredStates = exploredStates;
            foreach (var move in moves)
            {
                if (!move.Leaves)
                    Rearrangements++;
            }
        }

        internal static LevelSolution Solved(IReadOnlyList<SolutionMove> moves, int exploredStates) =>
            new LevelSolution(Solvability.Solvable, moves, exploredStates);

        internal static LevelSolution Unsolvable(int exploredStates) =>
            new LevelSolution(Solvability.Unsolvable, Array.Empty<SolutionMove>(), exploredStates);

        internal static LevelSolution Unknown(int exploredStates) =>
            new LevelSolution(Solvability.Unknown, Array.Empty<SolutionMove>(), exploredStates);
    }
}
