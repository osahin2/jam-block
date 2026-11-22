using System.Collections.Generic;
using Game.Data.Levels;
using Game.Logic.Play;

namespace Game.Logic.Levels
{
    internal sealed class BlockReach
    {
        private readonly Queue<Cell> _open = new Queue<Cell>();
        private readonly HashSet<Cell> _seen = new HashSet<Cell>();
        private readonly List<Cell> _places = new List<Cell>();

        public IReadOnlyList<Cell> Places => _places;
        public bool CanLeave { get; private set; }
        public Cell ExitPlace { get; private set; }

        public void Find(Board board, int block)
        {
            _open.Clear();
            _seen.Clear();
            _places.Clear();
            CanLeave = false;
            var start = board.Blocks[block].Position;
            _open.Enqueue(start);
            _seen.Add(start);
            while (_open.Count > 0 && !CanLeave)
                Visit(board, block, _open.Dequeue(), start);
            board.Place(block, start);
        }

        private void Visit(Board board, int block, Cell place, Cell start)
        {
            board.Place(block, place);
            if (board.CanLeave(block))
            {
                CanLeave = true;
                ExitPlace = place;
                return;
            }
            if (place != start)
                _places.Add(place);
            foreach (var direction in Direction.All)
            {
                var next = place + direction.Offset;
                if (board.CanStep(block, direction) && _seen.Add(next))
                    _open.Enqueue(next);
            }
        }
    }
}
