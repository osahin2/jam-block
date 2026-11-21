using UnityEngine;

namespace Game.Play
{
    public interface IBoardPieceSource
    {
        T Take<T>(T prefab, Transform parent) where T : Component;
    }
}
