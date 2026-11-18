using System;

namespace Game.Data.Coins
{
    // The saved coin balance.
    [Serializable]
    public sealed class CoinsDocument
    {
        public int Balance;
    }
}
