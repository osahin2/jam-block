using System;
using Game.Data.Levels;
using Game.Logic.Play;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Play
{
    // Builds a level's ground, frame, doors and blocks under one root object. The game builds it at the scene root;
    // the level editor's scene preview builds it under its own object.
    public sealed class BoardBuilder
    {
        private static readonly Side[] Sides = (Side[])Enum.GetValues(typeof(Side));
        // Frame pieces have their visible cap facing -Y.
        private static readonly Quaternion CapUp = Quaternion.Euler(180f, 0f, 0f);
        // The door piece is long along X; on the left and right sides it is turned to run along Z.
        private static readonly Quaternion AlongZ = Quaternion.Euler(0f, 90f, 0f);

        private readonly BoardArtSettings _art;
        private readonly ColorPalette _palette;
        private readonly BlockMotionSettings _motion;
        private readonly BlockPieceBuilder _pieces;
        private GameObject _root;

        public BoardBuilder(BoardArtSettings art, ColorPalette palette, BlockMotionSettings motion, BlockPieceBuilder pieces)
        {
            _art = art;
            _palette = palette;
            _motion = motion;
            _pieces = pieces;
        }

        public BlockView[] Build(LevelData level, BoardLayout layout, Transform parent = null)
        {
            Clear();
            _root = new GameObject("Board");
            _root.transform.SetParent(parent, false);
            BuildGround(level, layout);
            BuildFrame(level, layout);
            return BuildBlocks(level, layout);
        }

        private void Clear()
        {
            if (_root != null)
                Object.Destroy(_root);
        }

        private void BuildGround(LevelData level, BoardLayout layout)
        {
            for (var y = 0; y < level.Height; y++)
            {
                for (var x = 0; x < level.Width; x++)
                    Spawn(_art.Ground, _art.GroundMaterial, layout.CellCenter(new Cell(x, y)), Quaternion.identity);
            }
        }

        private void BuildFrame(LevelData level, BoardLayout layout)
        {
            foreach (var side in Sides)
                BuildSide(level, layout, side);

            var half = _art.Scale * 0.5f;
            Wall(new Vector3(layout.Left - half, 0f, layout.Top + half));
            Wall(new Vector3(layout.Right + half, 0f, layout.Top + half));
            Wall(new Vector3(layout.Left - half, 0f, layout.Bottom - half));
            Wall(new Vector3(layout.Right + half, 0f, layout.Bottom - half));
        }

        // Each cell along a side gets a door piece when a door covers it, else two half-cell wall pieces.
        private void BuildSide(LevelData level, BoardLayout layout, Side side)
        {
            var outward = Direction.OutOf(side).Offset;
            var outwardWorld = layout.ToWorld(Direction.OutOf(side));
            var along = side.IsHorizontal() ? Vector3.right : Vector3.forward;
            var turn = side.IsHorizontal() ? Quaternion.identity : AlongZ;
            var frameOffset = outwardWorld * (layout.CellSize * 0.5f + _art.Scale * 0.5f);
            var wallOffset = along * (_art.Scale * 0.5f);
            for (var i = 0; i < side.Length(level.Width, level.Height); i++)
            {
                var edgeCell = side.IsHorizontal()
                    ? new Cell(i, outward.Y < 0 ? 0 : level.Height - 1)
                    : new Cell(outward.X < 0 ? 0 : level.Width - 1, i);
                var center = layout.CellCenter(edgeCell) + frameOffset;
                var door = DoorAt(level, side, i);
                if (door != null)
                {
                    Spawn(_art.Door, _palette.For(door.ColorId), center + Lift(_art.FrameLift), turn * CapUp);
                    continue;
                }
                Wall(center - wallOffset);
                Wall(center + wallOffset);
            }
        }

        private BlockView[] BuildBlocks(LevelData level, BoardLayout layout)
        {
            var views = new BlockView[level.Blocks.Count];
            for (var i = 0; i < views.Length; i++)
            {
                var block = level.Blocks[i];
                var view = new GameObject($"Block {i}").AddComponent<BlockView>();
                view.Init(_motion);
                view.transform.SetParent(_root.transform, false);
                _pieces.Build(view.transform, block.Shape, _palette.For(block.ColorId));
                view.SetPose(layout.CellCenter(block.Position));
                views[i] = view;
            }
            return views;
        }

        private void Wall(Vector3 position) => Spawn(_art.Wall, _art.WallMaterial, position + Lift(_art.FrameLift), CapUp);

        private void Spawn(Mesh mesh, Material material, Vector3 position, Quaternion rotation)
        {
            var piece = new GameObject(mesh.name, typeof(MeshFilter), typeof(MeshRenderer));
            piece.GetComponent<MeshFilter>().sharedMesh = mesh;
            piece.GetComponent<MeshRenderer>().sharedMaterial = material;
            piece.transform.SetParent(_root.transform, false);
            // Local, like the blocks, so the board follows the parent it is built under.
            piece.transform.localPosition = position;
            piece.transform.localRotation = rotation;
            piece.transform.localScale = Vector3.one * _art.Scale;
        }

        private static DoorData DoorAt(LevelData level, Side side, int index)
        {
            foreach (var door in level.Doors)
            {
                if (door.Side == side && door.Covers(index))
                    return door;
            }
            return null;
        }

        private static Vector3 Lift(float height) => new Vector3(0f, height, 0f);
    }
}
