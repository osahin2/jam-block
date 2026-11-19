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
        // Height of the wall and door pieces' cap above their pivot, in model units, measured from WallAndDoor.fbx.
        private const float FrameTopAbovePivot = 0.611f;

        [SerializeField] private float _cellSize = 2f;
        [Tooltip("Raises block pieces above the ground, in model units; 0 sets their bottom on the ground, a negative " +
                 "value sinks them into it.")]
        [SerializeField] private float _blockLift;
        [Tooltip("Raises the frame (walls and doors), in model units; the pieces are deep, so a raised piece still " +
                 "reaches below the ground.")]
        [SerializeField] private float _frameLift;
        [Tooltip("Thickness of the walls across the wall, in model units; the supplied pieces are 1 thick (half a " +
                 "cell). The inner face stays on the board's edge.")]
        [SerializeField] private float _wallThickness = 1f;
        [Tooltip("Thickness of the doors across the wall, in model units, as for the walls.")]
        [SerializeField] private float _doorThickness = 1f;
        [Tooltip("Piece prefabs: a root at the piece's pivot with the model upright under it, a scale of 1; the board " +
                 "builder places, turns, scales and colors them.")]
        [SerializeField] private BoardPiece _blockCenter;
        [SerializeField] private BoardPiece _blockEdge;
        [SerializeField] private BoardPiece _blockOuterCorner;
        [SerializeField] private BoardPiece _blockInnerCorner;
        [SerializeField] private BoardPiece _ground;
        [SerializeField] private BoardPiece _wall;
        [SerializeField] private BoardPiece _door;
        [Tooltip("The frame's corner piece: a square of the wall's thickness, its rounded corner toward +X +Z.")]
        [SerializeField] private BoardPiece _frameCorner;
        [SerializeField] private BlockView _block;

        public float CellSize => _cellSize;
        public float Scale => _cellSize / ModelCellSize;
        public float BlockLift => _blockLift * Scale;
        public float FrameLift => _frameLift * Scale;
        public float WallWidth => _wallThickness * Scale;
        public float DoorWidth => _doorThickness * Scale;
        // How far the frame reaches out from the board's edge.
        public float FrameWidth => Mathf.Max(WallWidth, DoorWidth);
        // The frame's top above the ground.
        public float FrameTop => (_frameLift + FrameTopAbovePivot) * Scale;
        // The block tops, where a touch meets a block.
        public float TouchHeight => (_blockLift + BlockHeight) * Scale;
        public BoardPiece BlockCenter => _blockCenter;
        public BoardPiece BlockEdge => _blockEdge;
        public BoardPiece BlockOuterCorner => _blockOuterCorner;
        public BoardPiece BlockInnerCorner => _blockInnerCorner;
        public BoardPiece Ground => _ground;
        public BoardPiece Wall => _wall;
        public BoardPiece Door => _door;
        public BoardPiece FrameCorner => _frameCorner;
        public BlockView Block => _block;
    }
}
