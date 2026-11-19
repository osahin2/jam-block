using UnityEngine;

namespace Game.Play
{
    [CreateAssetMenu(menuName = "Game/Play/Board Art Settings")]
    public sealed class BoardArtSettings : ScriptableObject
    {
        // The supplied models are made for a cell of 2 model units; block pieces are 1-unit quarters of a cell.
        public const float ModelCellSize = 2f;
        // Height of a block piece from its bottom, where its pivot is, to its top, in model units, measured from
        // BlockParts.fbx.
        private const float BlockHeight = 1.619f;

        [SerializeField] private float _cellSize = 2f;
        [Tooltip("Raises block pieces above the ground, in model units; 0 sets their bottom on the ground, a negative " +
                 "value sinks them into it.")]
        [SerializeField] private float _blockLift;
        [Tooltip("Raises the frame (walls and doors), in model units.")]
        [SerializeField] private float _frameLift;
        [Tooltip("Piece prefabs: a root at the piece's pivot with the model upright under it, a scale of 1; the board " +
                 "builder places, turns, scales and colors them.")]
        [SerializeField] private BoardPiece _blockCenter;
        [SerializeField] private BoardPiece _blockEdge;
        [SerializeField] private BoardPiece _blockOuterCorner;
        [SerializeField] private BoardPiece _blockInnerCorner;
        [SerializeField] private BoardPiece _ground;
        [SerializeField] private BoardPiece _wall;
        [SerializeField] private BoardPiece _door;
        [SerializeField] private BlockView _block;

        public float CellSize => _cellSize;
        public float Scale => _cellSize / ModelCellSize;
        public float BlockLift => _blockLift * Scale;
        public float FrameLift => _frameLift * Scale;
        // Wall and door pieces are one model unit wide.
        public float FrameWidth => Scale;
        // The block tops, the highest part of the board.
        public float TouchHeight => (_blockLift + BlockHeight) * Scale;
        public BoardPiece BlockCenter => _blockCenter;
        public BoardPiece BlockEdge => _blockEdge;
        public BoardPiece BlockOuterCorner => _blockOuterCorner;
        public BoardPiece BlockInnerCorner => _blockInnerCorner;
        public BoardPiece Ground => _ground;
        public BoardPiece Wall => _wall;
        public BoardPiece Door => _door;
        public BlockView Block => _block;
    }
}
