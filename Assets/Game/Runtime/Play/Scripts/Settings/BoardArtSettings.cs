using UnityEngine;

namespace Game.Play
{
    [CreateAssetMenu(menuName = "Game/Play/Board Art Settings")]
    public sealed class BoardArtSettings : ScriptableObject
    {
        // The supplied models are made for a cell of 2 model units; block pieces are 1-unit quarters of a cell.
        public const float ModelCellSize = 2f;
        // Height of the block pieces' top above their pivot, in model units, measured from BlockParts.fbx.
        private const float BlockTopAbovePivot = 0.963f;

        [SerializeField] private float _cellSize = 2f;
        [Tooltip("Raises block pieces so their bottom sits on the ground, in model units.")]
        [SerializeField] private float _blockLift = 0.656f;
        [Tooltip("Raises the frame (walls and doors), in model units.")]
        [SerializeField] private float _frameLift;
        [Tooltip("Piece prefabs: one mesh each, a scale of 1; the board builder places, scales and colors them.")]
        [SerializeField] private MeshRenderer _blockCenter;
        [SerializeField] private MeshRenderer _blockEdge;
        [SerializeField] private MeshRenderer _blockOuterCorner;
        [SerializeField] private MeshRenderer _blockInnerCorner;
        [SerializeField] private MeshRenderer _ground;
        [SerializeField] private MeshRenderer _wall;
        [SerializeField] private MeshRenderer _door;
        [SerializeField] private BlockView _block;

        public float CellSize => _cellSize;
        public float Scale => _cellSize / ModelCellSize;
        public float BlockLift => _blockLift * Scale;
        public float FrameLift => _frameLift * Scale;
        // Wall and door pieces are one model unit wide.
        public float FrameWidth => Scale;
        // The block tops, the highest part of the board.
        public float TouchHeight => (_blockLift + BlockTopAbovePivot) * Scale;
        public MeshRenderer BlockCenter => _blockCenter;
        public MeshRenderer BlockEdge => _blockEdge;
        public MeshRenderer BlockOuterCorner => _blockOuterCorner;
        public MeshRenderer BlockInnerCorner => _blockInnerCorner;
        public MeshRenderer Ground => _ground;
        public MeshRenderer Wall => _wall;
        public MeshRenderer Door => _door;
        public BlockView Block => _block;
    }
}
