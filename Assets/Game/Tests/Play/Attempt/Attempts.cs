using Game.Data.Levels;
using Game.Logic.Play;

namespace Game.Tests.Play
{
    public static class Attempts
    {
        public static LevelAttempt Of(LevelData level) => new LevelAttempt(level, LevelMechanics.For(level));
    }
}
