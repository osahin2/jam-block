using System.Collections.Generic;
using Game.Editor.Import;
using Game.Play;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Game.Tests.Import
{
    public sealed class BlockPlatesImportTests
    {
        [Test]
        public void BlockPlates_EveryVertexAtOnePositionHasOneSmoothNormalInUV3()
        {
            var meshes = 0;
            foreach (var asset in AssetDatabase.LoadAllAssetsAtPath(BlockPlatesImport.BlockPlatesPath))
            {
                if (!(asset is Mesh mesh))
                    continue;
                meshes++;
                var smooth = new List<Vector3>();
                mesh.GetUVs(BlockPlatesImport.SmoothNormalsChannel, smooth);
                var vertices = mesh.vertices;
                Assert.AreEqual(vertices.Length, smooth.Count, mesh.name);
                var byPosition = new Dictionary<Vector3, Vector3>();
                for (var i = 0; i < vertices.Length; i++)
                {
                    Assert.AreEqual(1f, smooth[i].magnitude, 1e-3f, mesh.name);
                    if (byPosition.TryGetValue(vertices[i], out var first))
                        Assert.AreEqual(first, smooth[i], $"{mesh.name} vertex {i}");
                    else
                        byPosition.Add(vertices[i], smooth[i]);
                }
            }
            Assert.Greater(meshes, 0);
        }

        [Test]
        public void BlockPiecePrefabs_UseThePlates()
        {
            var guid = AssetDatabase.FindAssets("t:BoardArtSettings", new[] { "Assets/Game" })[0];
            var art = AssetDatabase.LoadAssetAtPath<BoardArtSettings>(AssetDatabase.GUIDToAssetPath(guid));
            foreach (var prefab in new[] { art.BlockCenter, art.BlockEdge, art.BlockOuterCorner, art.BlockInnerCorner })
            {
                var mesh = prefab.GetComponentInChildren<MeshFilter>().sharedMesh;
                Assert.AreEqual(BlockPlatesImport.BlockPlatesPath, AssetDatabase.GetAssetPath(mesh), prefab.name);
            }
        }

        // The fill material is made at run time, so a build that strips vertex channels no asset's shader reads would
        // drop UV3.
        [Test]
        public void Build_KeepsEveryMeshChannel()
        {
            Assert.IsFalse(PlayerSettings.stripUnusedMeshComponents);
        }
    }
}
