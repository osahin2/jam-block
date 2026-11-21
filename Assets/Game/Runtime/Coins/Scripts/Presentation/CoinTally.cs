namespace Game.Coins
{
    public static class CoinTally
    {
        public static int Share(int landing, int coins, int amount) => Upto(landing, coins, amount) - Upto(landing - 1, coins, amount);

        private static int Upto(int landed, int coins, int amount) => (int)((long)amount * landed / coins);
    }
}
