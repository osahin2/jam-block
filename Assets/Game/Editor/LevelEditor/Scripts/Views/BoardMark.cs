using Game.Data.Levels;

namespace Game.Editor.LevelEditor
{
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
