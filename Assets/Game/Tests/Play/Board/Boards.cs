using Game.Data.Levels;
using Game.Logic.Play;

namespace Game.Tests.Play
{
    public static class Boards
    {
        public static Board Of(LevelData level)
        {
            var mechanics = LevelMechanics.For(level);
            return new Board(level, mechanics.Rules, mechanics.Guards);
        }
    }
}
