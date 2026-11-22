using System.Diagnostics;
using UnityEditor;

namespace Game.Editor.LevelEditor
{
    internal sealed class FullCheckProgress
    {
        private const long RedrawMilliseconds = 100;

        private readonly int _budget;
        private readonly Stopwatch _watch = Stopwatch.StartNew();
        private long _drawnAt = -RedrawMilliseconds;

        public bool IsCancelled { get; private set; }

        public FullCheckProgress(int budget) => _budget = budget;

        public bool KeepSearching(int searched)
        {
            if (searched >= _budget)
                return false;
            if (_watch.ElapsedMilliseconds - _drawnAt < RedrawMilliseconds)
                return true;
            _drawnAt = _watch.ElapsedMilliseconds;
            IsCancelled = EditorUtility.DisplayCancelableProgressBar(
                "Checking the level fully",
                $"{searched} of {_budget} arrangements searched",
                (float)searched / _budget);
            return !IsCancelled;
        }
    }
}
