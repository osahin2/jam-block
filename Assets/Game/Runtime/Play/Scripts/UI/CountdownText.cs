using UnityEngine;

namespace Game.Play
{
    public static class CountdownText
    {
        public const int Length = 5;

        // Rounded up, so 00:00 shows only once the time is over.
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
