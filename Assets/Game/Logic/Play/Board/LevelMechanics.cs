using System.Collections.Generic;
using Game.Data.Levels;

namespace Game.Logic.Play
{
    public sealed class LevelMechanics
    {
        public IceMechanic Ice { get; }
        public IReadOnlyList<IMoveRule> Rules { get; }
        public IReadOnlyList<IBlockGuard> Guards { get; }

        private LevelMechanics(IceMechanic ice, IReadOnlyList<IMoveRule> rules, IReadOnlyList<IBlockGuard> guards)
        {
            Ice = ice;
            Rules = rules;
            Guards = guards;
        }

        public static LevelMechanics For(LevelData level)
        {
            var ice = new IceMechanic(level);
            return new LevelMechanics(ice, new IMoveRule[] { ice }, new IBlockGuard[] { ice });
        }
    }
}
