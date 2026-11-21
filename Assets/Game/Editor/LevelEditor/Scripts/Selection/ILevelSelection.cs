using System.Collections.Generic;
using Game.Data.Levels;

namespace Game.Editor.LevelEditor
{
    public interface ILevelSelection
    {
        bool Exists(LevelData level);
        int ColorId(LevelData level);
        string Describe(LevelData level);
        IEnumerable<Cell> Cells(LevelData level);
        void BeginMove(LevelData level);
        bool MoveBy(LevelData level, Cell offset);
        void Recolor(LevelData level, int colorId);
        void Delete(LevelData level);
    }
}
