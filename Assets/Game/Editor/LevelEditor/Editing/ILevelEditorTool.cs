using Game.Data.Levels;

namespace Game.Editor.LevelEditor.Editing
{
    // One way of editing the board with the pointer. Cells run from -1 to the board's size, so the ring just outside
    // the board is where doors are placed. Press and Drag return whether the level changed; a press that changes only
    // the tool's own state, such as the select tool's selection, returns false. Describe says what a press at the cell
    // would do, for the line under the board.
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
