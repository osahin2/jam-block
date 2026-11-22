using System.Collections.Generic;
using Game.Data.Levels;
using UnityEngine;

namespace Game.Play
{
    public sealed class BlockPieceBuilder
    {
        private static readonly Vector2Int[] Quarters =
        {
            new Vector2Int(1, 1), new Vector2Int(-1, 1), new Vector2Int(-1, -1), new Vector2Int(1, -1)
        };

        private readonly BoardArtSettings _art;
        private readonly HashSet<Cell> _cells = new HashSet<Cell>();
        private readonly HashSet<(Cell, Vector2Int)> _covered = new HashSet<(Cell, Vector2Int)>();

        public BlockPieceBuilder(BoardArtSettings art)
        {
            _art = art;
        }

        public void Build(BlockView block, IReadOnlyList<Cell> shape, Material material, IBoardPieceSource pieces)
        {
            Remember(shape);
            PlaceInnerCorners(block, shape, material, pieces);
            FillTheOtherQuarters(block, shape, material, pieces);
        }

        private void Remember(IReadOnlyList<Cell> shape)
        {
            _cells.Clear();
            _covered.Clear();
            foreach (var cell in shape)
                _cells.Add(cell);
        }

        private void PlaceInnerCorners(BlockView block, IReadOnlyList<Cell> shape, Material material, IBoardPieceSource pieces)
        {
            foreach (var cell in shape)
            {
                foreach (var quarter in Quarters)
                {
                    if (IsInnerCorner(cell, quarter))
                        PlaceInnerCorner(block, material, cell, quarter, pieces);
                }
            }
        }

        private void FillTheOtherQuarters(BlockView block, IReadOnlyList<Cell> shape, Material material,
            IBoardPieceSource pieces)
        {
            foreach (var cell in shape)
            {
                foreach (var quarter in Quarters)
                {
                    if (!_covered.Contains((cell, quarter)))
                        PlaceQuarter(block, material, cell, quarter, pieces);
                }
            }
        }

        private bool IsInnerCorner(Cell cell, Vector2Int quarter) =>
            HasSide(cell, quarter) && HasAbove(cell, quarter) && !HasDiagonal(cell, quarter);

        private void PlaceInnerCorner(BlockView block, Material material, Cell cell, Vector2Int quarter, IBoardPieceSource pieces)
        {
            Place(block, pieces, _art.BlockInnerCorner, material, cell, quarter, CornerAngle(quarter));
            _covered.Add((cell, quarter));
            _covered.Add((new Cell(cell.X + quarter.x, cell.Y), new Vector2Int(-quarter.x, quarter.y)));
            _covered.Add((new Cell(cell.X, cell.Y - quarter.y), new Vector2Int(quarter.x, -quarter.y)));
        }

        private void PlaceQuarter(BlockView block, Material material, Cell cell, Vector2Int quarter, IBoardPieceSource pieces)
        {
            var side = HasSide(cell, quarter);
            var above = HasAbove(cell, quarter);
            var quarterCenter = (Vector2)quarter * 0.5f;
            if (!side && !above)
            {
                Place(block, pieces, _art.BlockOuterCorner, material, cell, quarterCenter, CornerAngle(quarter));
                return;
            }
            if (side && above)
            {
                Place(block, pieces, _art.BlockCenter, material, cell, quarterCenter, 0f);
                return;
            }
            var rim = side ? new Vector2(0f, quarter.y) : new Vector2(quarter.x, 0f);
            var angle = Mathf.Atan2(rim.y, rim.x) * Mathf.Rad2Deg - 90f;
            Place(block, pieces, _art.BlockEdge, material, cell, quarterCenter, angle);
        }

        private void Place(BlockView block, IBoardPieceSource pieces, BoardPiece prefab, Material material, Cell cell,
            Vector2 pivot, float angle)
        {
            var scale = _art.Scale;
            var piece = pieces.Take(prefab, block.transform);
            block.AddPiece(piece);
            piece.SetMaterial(material);
            var transform = piece.transform;
            transform.localPosition = new Vector3(
                cell.X * _art.CellSize + pivot.x * scale,
                _art.BlockLift,
                -cell.Y * _art.CellSize + pivot.y * scale);
            transform.localRotation = Quaternion.Euler(0f, -angle, 0f);
            transform.localScale = Vector3.one * scale;
        }

        private bool HasSide(Cell cell, Vector2Int quarter) => _cells.Contains(new Cell(cell.X + quarter.x, cell.Y));

        private bool HasAbove(Cell cell, Vector2Int quarter) => _cells.Contains(new Cell(cell.X, cell.Y - quarter.y));

        private bool HasDiagonal(Cell cell, Vector2Int quarter) =>
            _cells.Contains(new Cell(cell.X + quarter.x, cell.Y - quarter.y));

        private static float CornerAngle(Vector2Int quarter) => Mathf.Atan2(quarter.y, quarter.x) * Mathf.Rad2Deg - 45f;
    }
}
