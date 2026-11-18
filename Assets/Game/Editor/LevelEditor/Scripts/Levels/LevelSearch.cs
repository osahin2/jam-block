using System;

namespace Game.Editor.LevelEditor.Levels
{
    // Whether a level matches what the designer typed in the level list: its play number, or a part of its name in any
    // case. An empty query matches every level.
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
