using System;
using System.Collections.Generic;
using Game.Data.Levels;
using Game.Play;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Editor.LevelEditor
{
    public sealed class LevelBoardView : VisualElement
    {
        private const int LeftButton = 0;
        private const int RightButton = 1;
        private const float OutlineWidth = 3f;
        private const float TileInset = 0.05f;
        private const float IceBadgeSize = 0.45f;
        private const float IceBadgeMinSize = 12f;
        private const float IceBadgeInset = 0.06f;
        private const float IceBadgeTextSize = 0.6f;
        private const float ArrowInset = 0.2f;
        private const float ArrowHead = 0.18f;
        private const float ArrowWidth = 3f;
        private static readonly Color ArrowOutlineColor = new Color(0.08f, 0.12f, 0.2f);
        private static readonly Color MissingColor = Color.magenta;

        public event Action<Cell> Pressed;
        public event Action<Cell> RightClicked;
        public event Action<Cell> Dragged;
        public event Action Released;
        public event Action<Cell> Hovered;
        public event Action HoverEnded;

        private readonly VisualElement _hoverLayer = new VisualElement { pickingMode = PickingMode.Ignore };
        private LevelData _level;
        private ColorPalette _palette;
        private IReadOnlyList<BoardMark> _marks;
        private float _cellSize;
        private Vector2 _origin;
        private Cell _lastCell;
        private Cell _hoverCell;
        private bool _hovering;

        public LevelBoardView()
        {
            AddToClassList("board");
            _hoverLayer.style.position = Position.Absolute;
            _hoverLayer.style.left = 0f;
            _hoverLayer.style.top = 0f;
            _hoverLayer.style.right = 0f;
            _hoverLayer.style.bottom = 0f;
            RegisterCallback<GeometryChangedEvent>(_ => Rebuild());
            RegisterCallback<PointerDownEvent>(OnPointerDown);
            RegisterCallback<PointerMoveEvent>(OnPointerMove);
            RegisterCallback<PointerUpEvent>(OnPointerUp);
            RegisterCallback<PointerLeaveEvent>(_ => EndHover());
        }

        public void Show(LevelData level, ColorPalette palette, IReadOnlyList<BoardMark> marks)
        {
            _level = level;
            _palette = palette;
            _marks = marks;
            Rebuild();
        }

        private void Rebuild()
        {
            Clear();
            if (_level == null || float.IsNaN(contentRect.width))
                return;
            var columns = _level.Width + 2;
            var rows = _level.Height + 2;
            _cellSize = Mathf.Floor(Mathf.Min(contentRect.width / columns, contentRect.height / rows));
            if (_cellSize < 1f)
                return;
            _origin = new Vector2((contentRect.width - _cellSize * columns) / 2f, (contentRect.height - _cellSize * rows) / 2f);
            Add(Box(BoardRect(), "board-ground"));
            for (var y = -1; y <= _level.Height; y++)
            {
                for (var x = -1; x <= _level.Width; x++)
                {
                    var cell = new Cell(x, y);
                    Add(_level.IsOnBoard(cell) ? BoardCell(cell) : FrameCell(cell));
                }
            }
            foreach (var block in _level.Blocks)
                AddMechanicsOf(block);
            if (_marks != null)
            {
                foreach (var mark in _marks)
                    Add(Box(PieceRect(mark.Cell), mark.ClassName));
            }
            Add(_hoverLayer);
            DrawHover();
        }

        private VisualElement BoardCell(Cell cell)
        {
            var block = LevelEdits.BlockAt(_level, cell);
            if (block == LevelEdits.NoBlock)
            {
                var inset = Mathf.Max(1f, _cellSize * TileInset);
                var tile = CellRect(cell);
                return Box(new Rect(tile.x + inset, tile.y + inset, tile.width - 2f * inset, tile.height - 2f * inset), "board-tile");
            }
            var element = Box(CellRect(cell), "board-block");
            element.style.backgroundColor = ColorOf(_level.Blocks[block].ColorId);
            element.style.borderTopWidth = Outline(cell, new Cell(0, -1), block);
            element.style.borderBottomWidth = Outline(cell, new Cell(0, 1), block);
            element.style.borderLeftWidth = Outline(cell, new Cell(-1, 0), block);
            element.style.borderRightWidth = Outline(cell, new Cell(1, 0), block);
            return element;
        }

        private void AddMechanicsOf(BlockData block)
        {
            if (block.Shape.Count == 0)
                return;
            if (block.Ice > 0)
                AddIceTint(block);
            if (block.Arrow.IsSet)
                Add(ArrowOf(block));
            if (block.Ice > 0)
                Add(IceBadgeOf(block));
        }

        private void AddIceTint(BlockData block)
        {
            foreach (var offset in block.Shape)
            {
                var cell = block.Position + offset;
                if (_level.IsOnBoard(cell))
                    Add(Box(CellRect(cell), "board-ice"));
            }
        }

        private VisualElement ArrowOf(BlockData block)
        {
            var arrow = block.Arrow;
            var horizontal = arrow.Axis == ArrowAxis.Horizontal;
            var rect = CellRect(block.Position + arrow.Start);
            if (horizontal)
                rect.width *= arrow.Length;
            else
                rect.height *= arrow.Length;
            var element = Box(rect, "board-arrow");
            element.generateVisualContent += context => DrawArrow(context.painter2D, rect.size, horizontal);
            return element;
        }

        private VisualElement IceBadgeOf(BlockData block)
        {
            var corner = CellRect(block.Position + TopLeftCellOf(block.Shape));
            var size = Mathf.Max(IceBadgeMinSize, _cellSize * IceBadgeSize);
            var inset = _cellSize * IceBadgeInset;
            var badge = Box(new Rect(corner.x + inset, corner.y + inset, size, size), "board-ice-badge");
            badge.style.borderTopLeftRadius = badge.style.borderTopRightRadius =
                badge.style.borderBottomLeftRadius = badge.style.borderBottomRightRadius = size / 2f;
            badge.style.fontSize = size * IceBadgeTextSize;
            badge.Add(new Label(block.Ice.ToString()) { pickingMode = PickingMode.Ignore });
            return badge;
        }

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

        private void DrawArrow(Painter2D painter, Vector2 size, bool horizontal)
        {
            var inset = _cellSize * ArrowInset;
            var head = _cellSize * ArrowHead;
            var from = horizontal ? new Vector2(inset, size.y / 2f) : new Vector2(size.x / 2f, inset);
            var to = horizontal ? new Vector2(size.x - inset, size.y / 2f) : new Vector2(size.x / 2f, size.y - inset);
            var along = (to - from).normalized;
            var across = new Vector2(-along.y, along.x);
            painter.lineCap = LineCap.Round;
            painter.lineJoin = LineJoin.Round;
            StrokeArrow(painter, from, to, along * head, across * head, ArrowOutlineColor, ArrowWidth + 2f);
            StrokeArrow(painter, from, to, along * head, across * head, Color.white, ArrowWidth);
        }

        private static void StrokeArrow(Painter2D painter, Vector2 from, Vector2 to, Vector2 along, Vector2 across, Color color,
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

        private VisualElement FrameCell(Cell cell)
        {
            var element = Box(FrameRect(cell), "frame");
            var door = LevelEdits.DoorAt(_level, cell);
            if (door != LevelEdits.NoDoor)
            {
                element.AddToClassList("frame--door");
                element.style.backgroundColor = ColorOf(_level.Doors[door].ColorId);
            }
            return element;
        }

        private void DrawHover()
        {
            _hoverLayer.Clear();
            if (!_hovering || _level == null || _cellSize < 1f)
                return;
            var block = LevelEdits.BlockAt(_level, _hoverCell);
            var door = LevelEdits.DoorAt(_level, _hoverCell);
            var cells = block != LevelEdits.NoBlock ? LevelEdits.BlockCells(_level, block)
                : door != LevelEdits.NoDoor ? LevelEdits.DoorCells(_level, door)
                : null;
            if (cells == null)
            {
                _hoverLayer.Add(Box(PieceRect(_hoverCell), "hover-cell"));
                return;
            }
            foreach (var cell in cells)
                _hoverLayer.Add(Box(PieceRect(cell), "hover-piece"));
        }

        private void Hover(Cell cell)
        {
            var inRing = cell.X >= -1 && cell.X <= _level.Width && cell.Y >= -1 && cell.Y <= _level.Height;
            if (!inRing)
            {
                EndHover();
                return;
            }
            if (_hovering && cell == _hoverCell)
                return;
            _hovering = true;
            _hoverCell = cell;
            DrawHover();
            Hovered?.Invoke(cell);
        }

        private void EndHover()
        {
            if (!_hovering)
                return;
            _hovering = false;
            DrawHover();
            HoverEnded?.Invoke();
        }

        private Rect PieceRect(Cell cell) => _level.IsOnBoard(cell) ? CellRect(cell) : FrameRect(cell);

        private Rect FrameRect(Cell cell)
        {
            var half = _cellSize / 2f;
            var rect = CellRect(cell);
            if (cell.X < 0)
                rect.x += half;
            if (cell.Y < 0)
                rect.y += half;
            if (cell.X < 0 || cell.X >= _level.Width)
                rect.width = half;
            if (cell.Y < 0 || cell.Y >= _level.Height)
                rect.height = half;
            return rect;
        }

        private Rect CellRect(Cell cell) =>
            new Rect(_origin.x + (cell.X + 1) * _cellSize, _origin.y + (cell.Y + 1) * _cellSize, _cellSize, _cellSize);

        private Rect BoardRect() =>
            new Rect(_origin.x + _cellSize, _origin.y + _cellSize, _level.Width * _cellSize, _level.Height * _cellSize);

        private static VisualElement Box(Rect rect, string className)
        {
            var element = new VisualElement { pickingMode = PickingMode.Ignore };
            element.AddToClassList(className);
            element.style.position = Position.Absolute;
            element.style.left = rect.x;
            element.style.top = rect.y;
            element.style.width = rect.width;
            element.style.height = rect.height;
            return element;
        }

        private float Outline(Cell cell, Cell step, int block) =>
            LevelEdits.BlockAt(_level, cell + step) == block ? 0f : OutlineWidth;

        private Color ColorOf(int colorId)
        {
            if (_palette == null || !_palette.Has(colorId))
                return MissingColor;
            return _palette.ForBlock(colorId).color;
        }

        private Cell CellAt(Vector2 localPosition) => new Cell(
            Mathf.FloorToInt((localPosition.x - _origin.x) / _cellSize) - 1,
            Mathf.FloorToInt((localPosition.y - _origin.y) / _cellSize) - 1);

        private void OnPointerDown(PointerDownEvent evt)
        {
            if (_level == null || _cellSize < 1f)
                return;
            if (evt.button == RightButton)
            {
                RightClicked?.Invoke(CellAt(evt.localPosition));
                evt.StopPropagation();
                return;
            }
            if (evt.button != LeftButton)
                return;
            this.CapturePointer(evt.pointerId);
            _lastCell = CellAt(evt.localPosition);
            Pressed?.Invoke(_lastCell);
            evt.StopPropagation();
        }

        private void OnPointerMove(PointerMoveEvent evt)
        {
            if (_level == null || _cellSize < 1f)
                return;
            var target = CellAt(evt.localPosition);
            Hover(target);
            if (!this.HasPointerCapture(evt.pointerId))
                return;
            while (_lastCell != target)
            {
                var step = _lastCell.X != target.X
                    ? new Cell(Math.Sign(target.X - _lastCell.X), 0)
                    : new Cell(0, Math.Sign(target.Y - _lastCell.Y));
                _lastCell += step;
                Dragged?.Invoke(_lastCell);
            }
        }

        private void OnPointerUp(PointerUpEvent evt)
        {
            if (!this.HasPointerCapture(evt.pointerId))
                return;
            this.ReleasePointer(evt.pointerId);
            Released?.Invoke();
        }
    }
}
