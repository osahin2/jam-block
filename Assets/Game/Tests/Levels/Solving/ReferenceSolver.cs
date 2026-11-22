using System.Collections.Generic;
using System.Text;
using Game.Data.Levels;
using Game.Logic.Levels;
using Game.Logic.Play;
using Game.Tests.Play;

namespace Game.Tests.Levels
{
    public static class ReferenceSolver
    {
        public static (Solvability Verdict, int Rearrangements) Solve(LevelData level, int stateBudget)
        {
            var board = Boards.Of(level);
            var rearrangementsTo = new Dictionary<string, int> { [KeyOf(board)] = 0 };
            var frontier = new LinkedList<(BoardArrangement Arrangement, int Rearrangements)>();
            frontier.AddFirst((board.CurrentArrangement(), 0));
            while (frontier.Count > 0)
            {
                var (arrangement, rearrangements) = frontier.First.Value;
                frontier.RemoveFirst();
                board.Arrange(arrangement);
                if (board.IsCleared)
                    return (Solvability.Solvable, rearrangements);
                if (rearrangementsTo[KeyOf(board)] < rearrangements)
                    continue;
                for (var block = 0; block < board.Blocks.Count; block++)
                {
                    board.Arrange(arrangement);
                    if (board.Blocks[block].IsGone)
                        continue;
                    var places = EveryPlace(board, block, out var exitPlace);
                    if (exitPlace.HasValue)
                    {
                        board.Place(block, exitPlace.Value);
                        board.TryLeave(block, out _);
                        if (!TryVisit(board, rearrangements, rearrangementsTo, frontier, true, stateBudget))
                            return (Solvability.Unknown, 0);
                    }
                    foreach (var place in places)
                    {
                        board.Arrange(arrangement);
                        board.Place(block, place);
                        if (!TryVisit(board, rearrangements + 1, rearrangementsTo, frontier, false, stateBudget))
                            return (Solvability.Unknown, 0);
                    }
                }
            }
            return (Solvability.Unsolvable, 0);
        }

        private static List<Cell> EveryPlace(Board board, int block, out Cell? exitPlace)
        {
            exitPlace = null;
            var places = new List<Cell>();
            var start = board.Blocks[block].Position;
            var seen = new HashSet<Cell> { start };
            var open = new Queue<Cell>();
            open.Enqueue(start);
            while (open.Count > 0)
            {
                var place = open.Dequeue();
                board.Place(block, place);
                if (board.CanLeave(block))
                {
                    exitPlace ??= place;
                    continue;
                }
                if (place != start)
                    places.Add(place);
                foreach (var direction in Direction.All)
                {
                    if (board.CanStep(block, direction) && seen.Add(place + direction.Offset))
                        open.Enqueue(place + direction.Offset);
                }
            }
            board.Place(block, start);
            return places;
        }

        private static bool TryVisit(
            Board board,
            int rearrangements,
            Dictionary<string, int> rearrangementsTo,
            LinkedList<(BoardArrangement, int)> frontier,
            bool first,
            int stateBudget)
        {
            var key = KeyOf(board);
            if (rearrangementsTo.TryGetValue(key, out var known) && known <= rearrangements)
                return true;
            rearrangementsTo[key] = rearrangements;
            if (first)
                frontier.AddFirst((board.CurrentArrangement(), rearrangements));
            else
                frontier.AddLast((board.CurrentArrangement(), rearrangements));
            return rearrangementsTo.Count < stateBudget;
        }

        private static string KeyOf(Board board)
        {
            var key = new StringBuilder();
            foreach (var block in board.Blocks)
                key.Append(block.IsGone ? "gone;" : $"{block.Position.X},{block.Position.Y};");
            return key.ToString();
        }
    }
}
