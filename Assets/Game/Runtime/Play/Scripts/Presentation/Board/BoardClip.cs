using UnityEngine;

namespace Game.Play
{
    public static class BoardClip
    {
        private static readonly int WorldToLocalId = Shader.PropertyToID("_BoardWorldToLocal");
        private static readonly int HalfSizeId = Shader.PropertyToID("_BoardHalfSize");

        public static void Set(BoardLayout layout, Transform root)
        {
            var half = layout.Box.extents;
            Shader.SetGlobalMatrix(WorldToLocalId, root.worldToLocalMatrix);
            Shader.SetGlobalVector(HalfSizeId, new Vector4(half.x, half.z, 0f, 0f));
        }
    }
}
