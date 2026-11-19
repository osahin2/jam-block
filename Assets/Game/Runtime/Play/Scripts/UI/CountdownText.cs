using UnityEngine;

namespace Game.Play
{
    // The level's time as the HUD shows it: whole seconds rounded up, so 00:00 shows only once the time is over,
    // written as mm:ss into a buffer the caller keeps, so a new second allocates nothing.
    public static class CountdownText
    {
        public const int Length = 5;

        public static int WholeSeconds(float remainingSeconds) => Mathf.Max(0, Mathf.CeilToInt(remainingSeconds));

        public static void Write(int wholeSeconds, char[] text)
        {
            var minutes = wholeSeconds / 60 % 100;
            var seconds = wholeSeconds % 60;
            text[0] = (char)('0' + minutes / 10);
            text[1] = (char)('0' + minutes % 10);
            text[2] = ':';
            text[3] = (char)('0' + seconds / 10);
            text[4] = (char)('0' + seconds % 10);
        }
    }
}
