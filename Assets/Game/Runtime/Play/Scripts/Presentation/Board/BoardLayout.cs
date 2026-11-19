using Game.Data.Levels;
using Game.Logic.Play;
using UnityEngine;

namespace Game.Play
{
    // Board cells to positions in the board root's local space, the space the board is built in, and back. The board
    // lies on the XZ plane centered on the origin; row 0 is the far edge (+Z), so rows run toward the camera.
    public sealed class BoardLayout
    {
        public float CellSize { get; }
        public float Left { get; }
        public float Right => -Left;
        public float Top { get; }
        public float Bottom => -Top;
        // The board with its frame, from the ground to the blocks' tops: what the camera frames and the input covers.
        public Bounds Box { get; }

        public BoardLayout(int width, int height, BoardArtSettings art)
        {
            CellSize = art.CellSize;
            Left = -width * CellSize * 0.5f;
            Top = height * CellSize * 0.5f;
            var size = new Vector3((Right + art.FrameWidth) * 2f, art.TouchHeight, (Top + art.FrameWidth) * 2f);
            Box = new Bounds(new Vector3(0f, art.TouchHeight * 0.5f, 0f), size);
        }

        public Vector3 CellCenter(Cell cell) =>
            new Vector3(Left + (cell.X + 0.5f) * CellSize, 0f, Top - (cell.Y + 0.5f) * CellSize);

        public Vector3 PoseToLocal(DragPose pose) =>
            CellCenter(pose.Position) + new Vector3(pose.OffsetX * CellSize, 0f, -pose.OffsetY * CellSize);

        public Vector3 ToLocal(Direction direction) => new Vector3(direction.Offset.X, 0f, -direction.Offset.Y);

        // In cells from the board's top-left corner, x to the right and y down, as BlockDrag expects.
        public Vector2 LocalToBoard(Vector3 local) => new Vector2((local.x - Left) / CellSize, (Top - local.z) / CellSize);
    }
}
