using System;
using System.Collections.Generic;
using Game.Logic.Boosters;
using NUnit.Framework;

namespace Game.Tests.Boosters
{
    public sealed class BoosterInventoryTests
    {
        private const string Hammer = "hammer";
        private const string Vacuum = "vacuum";

        [Test]
        public void Count_WithNothingSaved_IsTheStartingStock()
        {
            var inventory = new BoosterInventory(new MemoryBoosterStore(), Starting(2, 1));

            Assert.AreEqual(2, inventory.Count(Hammer));
            Assert.AreEqual(1, inventory.Count(Vacuum));
        }

        [Test]
        public void Count_WithASavedStock_IsTheSavedStock()
        {
            var store = new MemoryBoosterStore();
            store.Save(new Dictionary<string, int> { [Hammer] = 0 });

            var inventory = new BoosterInventory(store, Starting(2, 1));

            Assert.AreEqual(0, inventory.Count(Hammer));
            Assert.AreEqual(1, inventory.Count(Vacuum));
        }

        [Test]
        public void Count_OfAnUnknownBooster_IsZero()
        {
            var inventory = new BoosterInventory(new MemoryBoosterStore(), Starting(2, 1));

            Assert.AreEqual(0, inventory.Count("freeze"));
        }

        [Test]
        public void TryUse_WithStock_LowersItAndSaves()
        {
            var store = new MemoryBoosterStore();
            var inventory = new BoosterInventory(store, Starting(2, 1));

            Assert.IsTrue(inventory.TryUse(Hammer));

            Assert.AreEqual(1, inventory.Count(Hammer));
            Assert.AreEqual(1, store.Saved[Hammer]);
            Assert.AreEqual(1, store.Saved[Vacuum]);
        }

        [Test]
        public void TryUse_AtZero_ChangesAndSavesNothing()
        {
            var store = new MemoryBoosterStore();
            var inventory = new BoosterInventory(store, Starting(0, 1));

            Assert.IsFalse(inventory.TryUse(Hammer));

            Assert.AreEqual(0, inventory.Count(Hammer));
            Assert.AreEqual(0, store.SaveCount);
        }

        [Test]
        public void Add_RaisesTheStockAndSaves()
        {
            var store = new MemoryBoosterStore();
            var inventory = new BoosterInventory(store, Starting(0, 1));

            inventory.Add(Hammer, 3);

            Assert.AreEqual(3, inventory.Count(Hammer));
            Assert.AreEqual(3, store.Saved[Hammer]);
        }

        [Test]
        public void Add_ANegativeAmount_ThrowsAndSavesNothing()
        {
            var store = new MemoryBoosterStore();
            var inventory = new BoosterInventory(store, Starting(2, 1));

            Assert.Throws<ArgumentOutOfRangeException>(() => inventory.Add(Hammer, -1));
            Assert.AreEqual(2, inventory.Count(Hammer));
            Assert.AreEqual(0, store.SaveCount);
        }

        [Test]
        public void New_AfterAUse_StartsWhereTheLastInventoryStopped()
        {
            var store = new MemoryBoosterStore();
            new BoosterInventory(store, Starting(2, 1)).TryUse(Hammer);

            var inventory = new BoosterInventory(store, Starting(2, 1));

            Assert.AreEqual(1, inventory.Count(Hammer));
        }

        [Test]
        public void New_WithANegativeSavedStock_Throws()
        {
            var store = new MemoryBoosterStore();
            store.Save(new Dictionary<string, int> { [Hammer] = -1 });

            Assert.Throws<ArgumentOutOfRangeException>(() => new BoosterInventory(store, Starting(2, 1)));
        }

        private static Dictionary<string, int> Starting(int hammer, int vacuum) =>
            new Dictionary<string, int> { [Hammer] = hammer, [Vacuum] = vacuum };
    }
}
