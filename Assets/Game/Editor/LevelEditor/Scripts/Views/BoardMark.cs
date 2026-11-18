using Game.Data.Levels;

namespace Game.Editor.LevelEditor
{
    // A cell the board draws a style over: a selection, a problem, the problem under the pointer.
    public readonly struct BoardMark
    {
        public Cell Cell { get; }
        public string ClassName { get; }

        public BoardMark(Cell cell, string className)
        {
            Cell = cell;
            ClassName = className;
        }
    }
}
