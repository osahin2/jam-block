using System.Collections.Generic;
using Core.Save;
using Game.Data.Boosters;
using Game.Logic.Boosters;

namespace Game.Play
{
    public sealed class SavedBoosterStore : IBoosterStore
    {
        private const string Key = "boosters";

        private readonly ISaveService _save;

        public SavedBoosterStore(ISaveService save) => _save = save;

        public IReadOnlyDictionary<string, int> Load()
        {
            var stock = new Dictionary<string, int>();
            foreach (var entry in _save.Load<BoostersDocument>(Key).Stocks)
                stock[entry.Id] = entry.Count;
            return stock;
        }

        public void Save(IReadOnlyDictionary<string, int> stock)
        {
            var document = new BoostersDocument();
            foreach (var entry in stock)
                document.Stocks.Add(new BoosterStock { Id = entry.Key, Count = entry.Value });
            _save.Save(Key, document);
        }
    }
}
