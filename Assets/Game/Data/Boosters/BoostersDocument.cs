using System;
using System.Collections.Generic;

namespace Game.Data.Boosters
{
    [Serializable]
    public sealed class BoostersDocument
    {
        public List<BoosterStock> Stocks = new List<BoosterStock>();
    }
}
