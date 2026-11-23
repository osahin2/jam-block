using Game.Data.Levels;
using UnityEngine;

namespace Game.Play
{
    public sealed class BoardProjection
    {
        private readonly TouchPlane _plane;
        private BoardLayout _layout;

        public BoardProjection(TouchPlane plane)
        {
            _plane = plane;
        }

        public void Use(BoardLayout layout) => _layout = layout;

        public bool TryFindOnBoard(Vector2 screen, out Vector2 onBoard)
        {
            if (_plane.TryProject(screen, out var local))
            {
                onBoard = _layout.LocalToBoard(local);
                return true;
            }
            onBoard = default;
            return false;
        }

        public Vector2 ScreenPointOf(Cell cell, float lift) => _plane.ToScreen(_layout.AboveCell(cell, lift));

        public Rect ScreenRect()
        {
            var min = new Vector2(float.MaxValue, float.MaxValue);
            var max = new Vector2(float.MinValue, float.MinValue);
            for (var corner = 0; corner < BoardLayout.BoxCorners; corner++)
            {
                var screen = _plane.ToScreen(_layout.BoxCorner(corner));
                min = Vector2.Min(min, screen);
                max = Vector2.Max(max, screen);
            }
            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }
    }
}
