using System.Collections.Generic;
using Game.Data.Levels;

namespace Game.Logic.Play
{
    internal sealed class CellGroups
    {
        private readonly List<Cell>[] _groups;
        private readonly bool[] _grouped;

        public int Count { get; private set; }

        public IReadOnlyList<Cell> this[int group] => _groups[group];

        public CellGroups(int maxCells)
        {
            _groups = new List<Cell>[maxCells];
            for (var i = 0; i < maxCells; i++)
                _groups[i] = new List<Cell>(maxCells);
            _grouped = new bool[maxCells];
        }

        public void Find(IReadOnlyList<Cell> cells)
        {
            for (var i = 0; i < Count; i++)
                _groups[i].Clear();
            Count = 0;
            for (var i = 0; i < cells.Count; i++)
                _grouped[i] = false;
            for (var i = 0; i < cells.Count; i++)
            {
                if (!_grouped[i])
                    Gather(cells, i, _groups[Count++]);
            }
        }

        private void Gather(IReadOnlyList<Cell> cells, int first, List<Cell> group)
        {
            Take(cells, first, group);
            for (var reached = 0; reached < group.Count; reached++)
            {
                for (var i = 0; i < cells.Count; i++)
                {
                    if (!_grouped[i] && cells[i].Touches(group[reached]))
                        Take(cells, i, group);
                }
            }
        }

        private void Take(IReadOnlyList<Cell> cells, int index, List<Cell> group)
        {
            _grouped[index] = true;
            group.Add(cells[index]);
        }
    }
}
