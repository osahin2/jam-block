using System.Collections.Generic;
using Game.Data.Levels;

namespace Game.Logic.Play
{
    public sealed class LevelMechanics
    {
        public IceMechanic Ice { get; }
        public ArrowMechanic Arrow { get; }
        internal IReadOnlyList<IMoveRule> Rules { get; }
        internal IReadOnlyList<IBlockGuard> Guards { get; }

        private LevelMechanics(IceMechanic ice, ArrowMechanic arrow, IReadOnlyList<IMoveRule> rules,
            IReadOnlyList<IBlockGuard> guards)
        {
            Ice = ice;
            Arrow = arrow;
            Rules = rules;
            Guards = guards;
        }

        public static LevelMechanics For(LevelData level)
        {
            var ice = new IceMechanic(level);
            var arrow = new ArrowMechanic(level);
            return new LevelMechanics(ice, arrow, new IMoveRule[] { ice, arrow }, new IBlockGuard[] { ice });
        }
    }
}
