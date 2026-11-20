using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Game.Editor.Import
{
    // The held block's outline (Quick Outline's fill) pushes the pieces out along the normals in UV3. Hard edges split a
    // corner's vertices with different normals, so the pushed shell would crack there; each vertex gets the average
    // normal of all vertices at its position instead, written once at import. FBX carries only two-component UVs, so
    // this cannot be done in the model file.
    public sealed class BlockPlatesImport : AssetPostprocessor
    {
        public const string BlockPlatesPath = "Assets/Art/Models/BlockPlates.fbx";
        public const int SmoothNormalsChannel = 3;

        public override uint GetVersion() => 1;

        private void OnPostprocessModel(GameObject root)
        {
            if (assetPath != BlockPlatesPath)
                return;
            foreach (var filter in root.GetComponentsInChildren<MeshFilter>(true))
                WriteSmoothNormals(filter.sharedMesh);
        }

        private static void WriteSmoothNormals(Mesh mesh)
        {
            var vertices = mesh.vertices;
            var normals = mesh.normals;
            var sums = new Dictionary<Vector3, Vector3>();
            for (var i = 0; i < vertices.Length; i++)
            {
                sums.TryGetValue(vertices[i], out var sum);
                sums[vertices[i]] = sum + normals[i];
            }
            var smooth = new List<Vector3>(vertices.Length);
            foreach (var vertex in vertices)
                smooth.Add(sums[vertex].normalized);
            mesh.SetUVs(SmoothNormalsChannel, smooth);
        }
    }
}
