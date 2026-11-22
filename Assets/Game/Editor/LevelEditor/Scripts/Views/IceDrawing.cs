using System.Collections.Generic;
using Game.Data.Levels;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Editor.LevelEditor
{
    public sealed class IceDrawing : IBlockMechanicDrawing
    {
        private const float BadgeSize = 0.45f;
        private const float BadgeMinSize = 12f;
        private const float BadgeInset = 0.06f;
        private const float BadgeTextSize = 0.6f;

        public void DrawOnBlock(BlockData block, IBoardCanvas canvas)
        {
            if (block.Ice == 0)
                return;
            foreach (var offset in block.Shape)
            {
                var cell = block.Position + offset;
                if (canvas.IsOnBoard(cell))
                    canvas.AddBox(canvas.CellRect(cell), "board-ice");
            }
        }

        public void DrawOnTop(BlockData block, IBoardCanvas canvas)
        {
            if (block.Ice == 0)
                return;
            var corner = canvas.CellRect(block.Position + TopLeftCellOf(block.Shape));
            var size = Mathf.Max(BadgeMinSize, canvas.CellSize * BadgeSize);
            var inset = canvas.CellSize * BadgeInset;
            var badge = canvas.AddBox(new Rect(corner.x + inset, corner.y + inset, size, size), "board-ice-badge");
            Round(badge, size / 2f);
            badge.style.fontSize = size * BadgeTextSize;
            badge.Add(new Label(block.Ice.ToString()) { pickingMode = PickingMode.Ignore });
        }

        private static void Round(VisualElement element, float radius) =>
            element.style.borderTopLeftRadius = element.style.borderTopRightRadius =
                element.style.borderBottomLeftRadius = element.style.borderBottomRightRadius = radius;

        private static Cell TopLeftCellOf(List<Cell> shape)
        {
            var topLeft = shape[0];
            foreach (var cell in shape)
            {
                if (cell.Y < topLeft.Y || (cell.Y == topLeft.Y && cell.X < topLeft.X))
                    topLeft = cell;
            }
            return topLeft;
        }
    }
}
