using System.Collections.Generic;
using Game.Logic.Boosters;

namespace Game.Tests.Boosters
{
    public sealed class MemoryBoosterStore : IBoosterStore
    {
        private readonly List<string> _log;

        public Dictionary<string, int> Saved { get; } = new Dictionary<string, int>();
        public int SaveCount { get; private set; }

        public MemoryBoosterStore(List<string> log = null) => _log = log;

        public IReadOnlyDictionary<string, int> Load() => new Dictionary<string, int>(Saved);

        public void Save(IReadOnlyDictionary<string, int> stock)
        {
            Saved.Clear();
            foreach (var entry in stock)
                Saved[entry.Key] = entry.Value;
            SaveCount++;
            _log?.Add("boosters");
        }
    }
}
