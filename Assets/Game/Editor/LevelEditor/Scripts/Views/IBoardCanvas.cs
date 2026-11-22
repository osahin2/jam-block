using Game.Data.Levels;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Editor.LevelEditor
{
    public interface IBoardCanvas
    {
        float CellSize { get; }

        bool IsOnBoard(Cell cell);

        Rect CellRect(Cell cell);

        VisualElement AddBox(Rect rect, string className);
    }
}
