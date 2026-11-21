using Game.Data.Levels;

namespace Game.Logic.Levels
{
    public interface ILevelCheck
    {
        void Check(LevelData level, LevelReport report);
    }
}
