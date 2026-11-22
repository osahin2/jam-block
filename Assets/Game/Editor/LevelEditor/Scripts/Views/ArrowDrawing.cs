using Game.Data.Levels;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Editor.LevelEditor
{
    public sealed class ArrowDrawing : IBlockMechanicDrawing
    {
        private const float Inset = 0.2f;
        private const float Head = 0.18f;
        private const float Width = 3f;
        private const float OutlineExtraWidth = 2f;
        private static readonly Color OutlineColor = new Color(0.08f, 0.12f, 0.2f);

        public void DrawOnBlock(BlockData block, IBoardCanvas canvas)
        {
            if (!block.Arrow.IsSet)
                return;
            var horizontal = block.Arrow.Axis == ArrowAxis.Horizontal;
            var rect = SpanOf(block, canvas, horizontal);
            var cellSize = canvas.CellSize;
            var element = canvas.AddBox(rect, "board-arrow");
            element.generateVisualContent += context => Draw(context.painter2D, rect.size, horizontal, cellSize);
        }

        public void DrawOnTop(BlockData block, IBoardCanvas canvas)
        {
        }

        private static Rect SpanOf(BlockData block, IBoardCanvas canvas, bool horizontal)
        {
            var rect = canvas.CellRect(block.Position + block.Arrow.Start);
            if (horizontal)
                rect.width *= block.Arrow.Length;
            else
                rect.height *= block.Arrow.Length;
            return rect;
        }

        private static void Draw(Painter2D painter, Vector2 size, bool horizontal, float cellSize)
        {
            var inset = cellSize * Inset;
            var from = horizontal ? new Vector2(inset, size.y / 2f) : new Vector2(size.x / 2f, inset);
            var to = horizontal ? new Vector2(size.x - inset, size.y / 2f) : new Vector2(size.x / 2f, size.y - inset);
            var along = (to - from).normalized * (cellSize * Head);
            var across = new Vector2(-along.y, along.x);
            painter.lineCap = LineCap.Round;
            painter.lineJoin = LineJoin.Round;
            Stroke(painter, from, to, along, across, OutlineColor, Width + OutlineExtraWidth);
            Stroke(painter, from, to, along, across, Color.white, Width);
        }

        private static void Stroke(Painter2D painter, Vector2 from, Vector2 to, Vector2 along, Vector2 across, Color color,
            float width)
        {
            painter.strokeColor = color;
            painter.lineWidth = width;
            painter.BeginPath();
            painter.MoveTo(from);
            painter.LineTo(to);
            painter.MoveTo(from + along + across);
            painter.LineTo(from);
            painter.LineTo(from + along - across);
            painter.MoveTo(to - along + across);
            painter.LineTo(to);
            painter.LineTo(to - along - across);
            painter.Stroke();
        }
    }
}
