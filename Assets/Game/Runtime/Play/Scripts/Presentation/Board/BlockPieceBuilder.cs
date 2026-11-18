using System.Collections.Generic;
using Game.Data.Levels;
using Game.Play.Settings;
using UnityEngine;

namespace Game.Play.Presentation
{
    // Builds a block's mesh from the supplied quarter pieces. Each cell has four quarters; a quarter's piece follows
    // from its two side neighbours and the diagonal one: none gives an outer corner, one an edge, both with the
    // diagonal the center. Where both side neighbours exist without the diagonal, one inner-corner piece covers the
    // three quarters around that concave corner. Piece math is in model units, where a cell is 2 wide.
    public sealed class BlockPieceBuilder
    {
        private static readonly Vector2Int[] Quarters =
        {
            new Vector2Int(1, 1), new Vector2Int(-1, 1), new Vector2Int(-1, -1), new Vector2Int(1, -1)
        };
        // Pieces lie on their XY plane with the top facing -Z; this turns the top up and their +Y to world +Z.
        private static readonly Quaternion LayFlat = Quaternion.Euler(90f, 0f, 0f);
        private static readonly Vector2 PieceCenter = new Vector2(0.5f, 0.5f);

        private readonly BoardArtSettings _art;
        private readonly HashSet<Cell> _cells = new HashSet<Cell>();
        private readonly HashSet<(Cell, Vector2Int)> _covered = new HashSet<(Cell, Vector2Int)>();

        public BlockPieceBuilder(BoardArtSettings art)
        {
            _art = art;
        }

        public void Build(Transform parent, IReadOnlyList<Cell> shape, Material material)
        {
            _cells.Clear();
            _covered.Clear();
            foreach (var cell in shape)
                _cells.Add(cell);

            foreach (var cell in shape)
            {
                foreach (var quarter in Quarters)
                {
                    if (IsInnerCorner(cell, quarter))
                        PlaceInnerCorner(parent, material, cell, quarter);
                }
            }
            foreach (var cell in shape)
            {
                foreach (var quarter in Quarters)
                {
                    if (!_covered.Contains((cell, quarter)))
                        PlaceQuarter(parent, material, cell, quarter);
                }
            }
        }

        private bool IsInnerCorner(Cell cell, Vector2Int quarter) =>
            HasSide(cell, quarter) && HasAbove(cell, quarter) && !HasDiagonal(cell, quarter);

        private void PlaceInnerCorner(Transform parent, Material material, Cell cell, Vector2Int quarter)
        {
            Place(parent, _art.BlockInnerCorner, material, cell, quarter, CornerAngle(quarter));
            _covered.Add((cell, quarter));
            _covered.Add((new Cell(cell.X + quarter.x, cell.Y), new Vector2Int(-quarter.x, quarter.y)));
            _covered.Add((new Cell(cell.X, cell.Y - quarter.y), new Vector2Int(quarter.x, -quarter.y)));
        }

        private void PlaceQuarter(Transform parent, Material material, Cell cell, Vector2Int quarter)
        {
            var side = HasSide(cell, quarter);
            var above = HasAbove(cell, quarter);
            var quarterCenter = (Vector2)quarter * 0.5f;
            if (!side && !above)
            {
                Place(parent, _art.BlockOuterCorner, material, cell, Vector2.zero, CornerAngle(quarter));
                return;
            }
            if (side && above)
            {
                Place(parent, _art.BlockCenter, material, cell, quarterCenter - PieceCenter, 0f);
                return;
            }
            var rim = side ? new Vector2(0f, quarter.y) : new Vector2(quarter.x, 0f);
            var angle = Mathf.Atan2(rim.y, rim.x) * Mathf.Rad2Deg - 90f;
            Place(parent, _art.BlockEdge, material, cell, quarterCenter - Rotate(PieceCenter, angle), angle);
        }

        private void Place(Transform parent, Mesh mesh, Material material, Cell cell, Vector2 pieceOffset, float angle)
        {
            var scale = _art.Scale;
            var piece = new GameObject(mesh.name, typeof(MeshFilter), typeof(MeshRenderer));
            piece.GetComponent<MeshFilter>().sharedMesh = mesh;
            piece.GetComponent<MeshRenderer>().sharedMaterial = material;
            var transform = piece.transform;
            transform.SetParent(parent, false);
            transform.localPosition = new Vector3(
                cell.X * _art.CellSize + pieceOffset.x * scale,
                _art.BlockLift,
                -cell.Y * _art.CellSize + pieceOffset.y * scale);
            transform.localRotation = Quaternion.Euler(0f, -angle, 0f) * LayFlat;
            transform.localScale = Vector3.one * scale;
        }

        // Quarter and piece axes: x to the right, y up the board (toward row 0).
        private bool HasSide(Cell cell, Vector2Int quarter) => _cells.Contains(new Cell(cell.X + quarter.x, cell.Y));

        private bool HasAbove(Cell cell, Vector2Int quarter) => _cells.Contains(new Cell(cell.X, cell.Y - quarter.y));

        private bool HasDiagonal(Cell cell, Vector2Int quarter) =>
            _cells.Contains(new Cell(cell.X + quarter.x, cell.Y - quarter.y));

        private static float CornerAngle(Vector2Int quarter) => Mathf.Atan2(quarter.y, quarter.x) * Mathf.Rad2Deg - 45f;

        private static Vector2 Rotate(Vector2 vector, float degrees)
        {
            var radians = degrees * Mathf.Deg2Rad;
            var cos = Mathf.Cos(radians);
            var sin = Mathf.Sin(radians);
            return new Vector2(vector.x * cos - vector.y * sin, vector.x * sin + vector.y * cos);
        }
    }
}
