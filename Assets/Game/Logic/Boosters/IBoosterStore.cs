using System.Collections.Generic;

namespace Game.Logic.Boosters
{
    public interface IBoosterStore
    {
        IReadOnlyDictionary<string, int> Load();

        void Save(IReadOnlyDictionary<string, int> stock);
    }
}
