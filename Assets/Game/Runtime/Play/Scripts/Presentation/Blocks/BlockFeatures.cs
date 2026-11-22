using System.Collections.Generic;

namespace Game.Play
{
    public static class BlockFeatures
    {
        public static IReadOnlyList<IBlockFeature> Of(IceSettings ice, BoardArtSettings art) =>
            new IBlockFeature[] { new IceFeature(ice, art), new ArrowFeature(art) };
    }
}
