using System;

namespace Game.Editor.LevelEditor
{
    public static class LevelSearch
    {
        public static bool Matches(int number, string name, string query)
        {
            var wanted = query.Trim();
            if (wanted.Length == 0)
                return true;
            if (int.TryParse(wanted, out var wantedNumber) && wantedNumber == number)
                return true;
            return name.IndexOf(wanted, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
