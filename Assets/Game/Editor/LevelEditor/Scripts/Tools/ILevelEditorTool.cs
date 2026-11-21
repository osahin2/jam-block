using Game.Data.Levels;

namespace Game.Editor.LevelEditor
{
    // Cells run from -1 to the board's size: the ring outside the board is where doors go. Press and Drag return
    // whether the level changed, false when only the tool's own state, such as a selection, did.
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
