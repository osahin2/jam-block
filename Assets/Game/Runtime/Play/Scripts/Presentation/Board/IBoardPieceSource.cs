using UnityEngine;

namespace Game.Play
{
    // A pooled source reuses instances, so whoever gave the source releases what was taken, its own way.
    public interface IBoardPieceSource
    {
        // Active under the parent with the prefab's local pose.
        T Take<T>(T prefab, Transform parent) where T : Component;
    }
}
