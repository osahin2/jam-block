using UnityEngine;

namespace Game.Play
{
    // A factory for the board's pieces. The builder chooses which prefab it needs; the source decides how an instance
    // is made. It is "taken", not created: a pooled source reuses instances, so the caller that gave the source owns
    // what was taken and releases it its own way (the game gives pooled pieces back, the level editor's preview
    // destroys its plain copies with their parent).
    public interface IBoardPieceSource
    {
        // An instance of the prefab, active under the parent with the prefab's local pose.
        MeshRenderer Take(MeshRenderer prefab, Transform parent);

        // An instance of the block prefab, active under the parent with the prefab's local pose.
        BlockView Take(BlockView prefab, Transform parent);
    }
}
