using Game.Data.Levels;
using Game.Logic.Play;
using UnityEngine;

namespace Game.Play
{
    public sealed class BoardLayout
    {
        public const int BoxCorners = 8;

        public float CellSize { get; }
        public float Left { get; }
        public float Right => -Left;
        public float Top { get; }
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

        public Vector3 CellCenter(Cell cell) =>
            new Vector3(Left + (cell.X + 0.5f) * CellSize, 0f, Top - (cell.Y + 0.5f) * CellSize);

        public Vector3 AboveCell(Cell cell, float lift) => CellCenter(cell) + Vector3.up * (lift * CellSize);

        public Vector3 PoseToLocal(DragPose pose) =>
            CellCenter(pose.Position) + new Vector3(pose.OffsetX * CellSize, 0f, -pose.OffsetY * CellSize);

        public Vector3 ToLocal(Direction direction) => new Vector3(direction.Offset.X, 0f, -direction.Offset.Y);

        public Vector2 LocalToBoard(Vector3 local) => new Vector2((local.x - Left) / CellSize, (Top - local.z) / CellSize);

        public Vector3 BoxCorner(int corner) =>
            new Vector3(
                (corner & 1) == 0 ? Box.min.x : Box.max.x,
                (corner & 2) == 0 ? Box.min.y : Box.max.y,
                (corner & 4) == 0 ? Box.min.z : Box.max.z);
    }
}
