using System;
using System.Collections.Generic;
using Game.Logic.Levels;

namespace Game.Editor.LevelEditor
{
    public static class SolverGuide
    {
        private static readonly Dictionary<Solvability, Func<LevelSolution, bool, string>> Summaries =
            new Dictionary<Solvability, Func<LevelSolution, bool, string>>
            {
                [Solvability.Solvable] = (solution, _) => solution.Rearrangements == 0
                    ? $"Solvable in {EditorText.Plural(solution.Moves.Count, "move")}: every block can leave in turn. {Searched(solution)}"
                    : $"Solvable in {EditorText.Plural(solution.Moves.Count, "move")} at the fewest, {solution.Rearrangements} of them only to make room. {Searched(solution)}",
                [Solvability.Unsolvable] = (solution, _) =>
                    $"Unsolvable: in none of the {EditorText.Plural(solution.ExploredStates, "arrangement")} the player can reach can every block leave. Without boosters no one can clear it.",
                [Solvability.Unknown] = (solution, searchedFully) => searchedFully
                    ? $"Not decided: the full check stopped at {EditorText.Plural(solution.ExploredStates, "arrangement")}, its budget in SolverSettings. Play the level to check it, or raise the budget."
                    : $"Not decided: the check stopped at {EditorText.Plural(solution.ExploredStates, "arrangement")}. Check fully searches further."
            };

        public static bool Explains(Solvability verdict) => Summaries.ContainsKey(verdict);

        public static string Summarize(LevelSolution solution, bool searchedFully) =>
            Summaries[solution.Verdict](solution, searchedFully);

        private static string Searched(LevelSolution solution) =>
            $"{EditorText.Plural(solution.ExploredStates, "arrangement")} of the blocks searched.";
    }
}
