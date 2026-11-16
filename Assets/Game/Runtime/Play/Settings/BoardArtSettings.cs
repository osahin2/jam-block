using UnityEngine;

namespace Game.Play.Settings
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
        [SerializeField] private Mesh _blockCenter;
        [SerializeField] private Mesh _blockEdge;
        [SerializeField] private Mesh _blockOuterCorner;
        [SerializeField] private Mesh _blockInnerCorner;
        [SerializeField] private Mesh _ground;
        [SerializeField] private Mesh _wall;
        [SerializeField] private Mesh _door;
        [SerializeField] private Material _groundMaterial;
        [SerializeField] private Material _wallMaterial;

        public float CellSize => _cellSize;
        public float Scale => _cellSize / ModelCellSize;
        public float BlockLift => _blockLift * Scale;
        public float FrameLift => _frameLift * Scale;
        public float TouchHeight => (_blockLift + BlockTopAbovePivot) * Scale;
        public Mesh BlockCenter => _blockCenter;
        public Mesh BlockEdge => _blockEdge;
        public Mesh BlockOuterCorner => _blockOuterCorner;
        public Mesh BlockInnerCorner => _blockInnerCorner;
        public Mesh Ground => _ground;
        public Mesh Wall => _wall;
        public Mesh Door => _door;
        public Material GroundMaterial => _groundMaterial;
        public Material WallMaterial => _wallMaterial;
    }
}
