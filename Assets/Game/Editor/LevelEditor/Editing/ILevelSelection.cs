using System.Collections.Generic;
using Game.Data.Levels;

namespace Game.Editor.LevelEditor.Editing
{
    // A selected block or door. Each kind knows how it is drawn, moved, recolored and deleted, so the select tool
    // and the window treat both the same way.
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
