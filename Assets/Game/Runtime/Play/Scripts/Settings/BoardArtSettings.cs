using UnityEngine;

namespace Game.Play
{
    [CreateAssetMenu(menuName = "Game/Play/Board Art Settings")]
    public sealed class BoardArtSettings : ScriptableObject
    {
        // The supplied models are made for a cell of 2 model units; block pieces are 1-unit quarters of a cell.
        public const float ModelCellSize = 2f;
        // Bottom, where the pivot is, to top; measured from BlockPlates.fbx.
        private const float BlockHeight = 0.519f;
        // Measured from WallAndDoor.fbx.
        private const float FrameTopAbovePivot = 0.611f;
        // Measured from WallAndDoor.fbx. Wall and door are straight extrusions along X, so a stretched piece keeps
        // its cross-section.
        private const float WallModelLength = 1f;
        private const float DoorModelLength = 2f;
        // Bottom, where the pivot is, to top; measured from WallAndDoor.fbx.
        private const float ArrowHeight = 0.126f;

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
        [Tooltip("Height of a dropped door's top above the ground while a block passes, in model units: the flat " +
                 "strip of its color left in the frame.")]
        [SerializeField] private float _doorOpenHeight = 0.05f;
        [Tooltip("Piece prefabs: a root at the piece's pivot with the model upright under it, a scale of 1; the board " +
                 "builder places, turns, scales and colors them.")]
        [SerializeField] private BoardPiece _blockCenter;
        [SerializeField] private BoardPiece _blockEdge;
        [SerializeField] private BoardPiece _blockOuterCorner;
        [SerializeField] private BoardPiece _blockInnerCorner;
        [SerializeField] private BoardPiece _ground;
        [SerializeField] private BoardPiece _wall;
        [SerializeField] private DoorView _door;
        [Tooltip("The frame's corner piece: a square of the wall's thickness, its rounded corner toward +X +Z.")]
        [SerializeField] private BoardPiece _frameCorner;
        [Tooltip("The arrow on a door's top: its bottom on its pivot, pointing out along +Z.")]
        [SerializeField] private BoardPiece _doorArrow;
        [SerializeField] private BlockView _block;

        public float CellSize => _cellSize;
        public float Scale => _cellSize / ModelCellSize;
        public float BlockLift => _blockLift * Scale;
        public float FrameLift => _frameLift * Scale;
        public float WallWidth => _wallThickness * Scale;
        public float DoorWidth => _doorThickness * Scale;
        public float WallScalePerCell => _cellSize / WallModelLength;
        public float DoorScalePerCell => _cellSize / DoorModelLength;
        public float FrameWidth => Mathf.Max(WallWidth, DoorWidth);
        public float FrameTop => (_frameLift + FrameTopAbovePivot) * Scale;
        public float ArrowTop => FrameTop + ArrowHeight * Scale;
        public float DoorDrop => FrameTop - _doorOpenHeight * Scale;
        public float TouchHeight => (_blockLift + BlockHeight) * Scale;
        public BoardPiece BlockCenter => _blockCenter;
        public BoardPiece BlockEdge => _blockEdge;
        public BoardPiece BlockOuterCorner => _blockOuterCorner;
        public BoardPiece BlockInnerCorner => _blockInnerCorner;
        public BoardPiece Ground => _ground;
        public BoardPiece Wall => _wall;
        public DoorView Door => _door;
        public BoardPiece FrameCorner => _frameCorner;
        public BoardPiece DoorArrow => _doorArrow;
        public BlockView Block => _block;
    }
}
