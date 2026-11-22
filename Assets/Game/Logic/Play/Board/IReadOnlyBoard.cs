using System.Collections.Generic;
using Game.Data.Levels;

namespace Game.Logic.Play
{
    public interface IReadOnlyBoard
    {
        int GoneCount { get; }
        IReadOnlyList<BoardBlock> Blocks { get; }

        int BlockAt(Cell cell);

        bool IsGuarded(int block);
    }
}
