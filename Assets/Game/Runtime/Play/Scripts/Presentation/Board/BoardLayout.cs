using Game.Data.Levels;
using Game.Logic.Play;
using UnityEngine;

namespace Game.Play
{
    public sealed class BoardLayout
    {
        public float CellSize { get; }
        public float Left { get; }
        public float Right => -Left;
        public float Top { get; }
        public float Bottom => -Top;
        public float FrameWidth { get; }
        public Bounds Box { get; }

        public BoardLayout(int width, int height, BoardArtSettings art)
        {
            CellSize = art.CellSize;
            Left = -width * CellSize * 0.5f;
            Top = height * CellSize * 0.5f;
            FrameWidth = art.FrameWidth;
            var highest = Mathf.Max(art.TouchHeight, art.ArrowTop);
            var size = new Vector3((Right + FrameWidth) * 2f, highest, (Top + FrameWidth) * 2f);
            Box = new Bounds(new Vector3(0f, highest * 0.5f, 0f), size);
        }

        // Row 0 is the far edge (+Z), so rows run toward the camera.
        public Vector3 CellCenter(Cell cell) =>
            new Vector3(Left + (cell.X + 0.5f) * CellSize, 0f, Top - (cell.Y + 0.5f) * CellSize);

        public Vector3 PoseToLocal(DragPose pose) =>
            CellCenter(pose.Position) + new Vector3(pose.OffsetX * CellSize, 0f, -pose.OffsetY * CellSize);

        public Vector3 ToLocal(Direction direction) => new Vector3(direction.Offset.X, 0f, -direction.Offset.Y);

        // In cells from the board's top-left corner, x to the right and y down, as BlockDrag expects.
        public Vector2 LocalToBoard(Vector3 local) => new Vector2((local.x - Left) / CellSize, (Top - local.z) / CellSize);
    }
}
