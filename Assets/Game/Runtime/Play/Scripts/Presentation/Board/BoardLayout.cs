using Game.Data.Levels;
using Game.Logic.Play;
using UnityEngine;

namespace Game.Play
{
    // Board cells to world positions and back. The board lies on the XZ plane centered on the origin; row 0 is the
    // far edge (+Z), so rows run toward the camera.
    public sealed class BoardLayout
    {
        public float CellSize { get; }
        public float Left { get; }
        public float Right => -Left;
        public float Top { get; }
        public float Bottom => -Top;

        public BoardLayout(int width, int height, float cellSize)
        {
            CellSize = cellSize;
            Left = -width * cellSize * 0.5f;
            Top = height * cellSize * 0.5f;
        }

        public Vector3 CellCenter(Cell cell) =>
            new Vector3(Left + (cell.X + 0.5f) * CellSize, 0f, Top - (cell.Y + 0.5f) * CellSize);

        public Vector3 PoseToWorld(DragPose pose) =>
            CellCenter(pose.Position) + new Vector3(pose.OffsetX * CellSize, 0f, -pose.OffsetY * CellSize);

        public Vector3 ToWorld(Direction direction) => new Vector3(direction.Offset.X, 0f, -direction.Offset.Y);

        // In cells from the board's top-left corner, x to the right and y down, as BlockDrag expects.
        public Vector2 WorldToBoard(Vector3 world) => new Vector2((world.x - Left) / CellSize, (Top - world.z) / CellSize);
    }
}
