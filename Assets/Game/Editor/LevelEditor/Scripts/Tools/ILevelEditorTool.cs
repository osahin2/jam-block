using Game.Data.Levels;

namespace Game.Editor.LevelEditor
{
    public interface ILevelEditorTool
    {
        string Name { get; }
        string Tooltip { get; }

        string Describe(LevelData level, Cell cell);
        bool Press(LevelData level, Cell cell, int colorId);
        bool Drag(LevelData level, Cell cell);
        void Release();
    }
}
