using System.Collections.Generic;
using Game.Data.Levels;

namespace Game.Logic.Play
{
    public sealed class VacuumEffect : IBoardEffect
    {
        private readonly List<int> _targets = new List<int>();

        public bool CanTarget(IReadOnlyBoard board, Cell cell)
        {
            var block = board.BlockAt(cell);
            return block != Board.NoBlock && !board.IsGuarded(block);
        }

        public void Apply(IBoard board, Cell cell)
        {
            var color = board.Blocks[board.BlockAt(cell)].ColorId;
            _targets.Clear();
            for (var i = 0; i < board.Blocks.Count; i++)
            {
                var block = board.Blocks[i];
                if (!block.IsGone && block.ColorId == color && !board.IsGuarded(i))
                    _targets.Add(i);
            }
            for (var i = 0; i < _targets.Count; i++)
                board.Hit(_targets[i]);
        }
    }
}
