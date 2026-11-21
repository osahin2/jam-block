using System;
using System.Collections.Generic;

namespace Game.Logic.Boosters
{
    public sealed class BoosterInventory
    {
        private readonly IBoosterStore _store;
        private readonly Dictionary<string, int> _stock = new Dictionary<string, int>();

        public BoosterInventory(IBoosterStore store, IReadOnlyDictionary<string, int> startingStock)
        {
            _store = store;
            foreach (var start in startingStock)
                _stock[start.Key] = start.Value;
            foreach (var saved in store.Load())
            {
                if (saved.Value < 0)
                    throw new ArgumentOutOfRangeException(nameof(store), saved.Value, "A stored booster stock cannot be negative.");
                _stock[saved.Key] = saved.Value;
            }
        }

        public int Count(string id) => _stock.TryGetValue(id, out var count) ? count : 0;

        public bool TryUse(string id)
        {
            var count = Count(id);
            if (count == 0)
                return false;
            _stock[id] = count - 1;
            _store.Save(_stock);
            return true;
        }

        public void Add(string id, int amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount), amount, "An amount to add cannot be negative.");
            _stock[id] = Count(id) + amount;
            _store.Save(_stock);
        }
    }
}
