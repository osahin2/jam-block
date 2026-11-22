using System.Diagnostics;
using Game.Data.Levels;
using Game.Logic.Levels;
using UnityEditor;
using UnityEngine;

namespace Game.Editor.LevelEditor
{
    public sealed class SolvabilityCheck
    {
        private readonly SolverSettings _settings;
        private string _checkedLevel;
        private LevelSolution _solution;

        public bool SearchedFully { get; private set; }

        public SolvabilityCheck(SolverSettings settings) => _settings = settings;

        public LevelSolution Solve(LevelData level)
        {
            var json = JsonUtility.ToJson(level);
            if (_solution != null && json == _checkedLevel)
                return _solution;
            var watch = Stopwatch.StartNew();
            Remember(json, LevelSolver.Solve(level, _ => watch.ElapsedMilliseconds < _settings.QuickCheckMilliseconds), false);
            return _solution;
        }

        public void SearchFully(LevelData level)
        {
            var progress = new FullCheckProgress(_settings.FullCheckBudget);
            try
            {
                var solution = LevelSolver.Solve(level, progress.KeepSearching);
                Remember(JsonUtility.ToJson(level), solution, !progress.IsCancelled);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private void Remember(string level, LevelSolution solution, bool searchedFully)
        {
            _checkedLevel = level;
            _solution = solution;
            SearchedFully = searchedFully;
        }
    }
}
