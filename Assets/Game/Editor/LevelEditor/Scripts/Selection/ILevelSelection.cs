using System.Collections.Generic;
using Game.Data.Levels;

namespace Game.Editor.LevelEditor
{
    public interface ILevelSelection
    {
        bool Exists(LevelData level);
        int ColorId(LevelData level);
        string Describe(LevelData level);
        // Board cells for a block, ring cells for a door.
        IEnumerable<Cell> Cells(LevelData level);
        void BeginMove(LevelData level);
        // Moves by the offset from where it was when the move began; returns whether it moved.
        bool MoveBy(LevelData level, Cell offset);
        void Recolor(LevelData level, int colorId);
        void Delete(LevelData level);
    }
}
