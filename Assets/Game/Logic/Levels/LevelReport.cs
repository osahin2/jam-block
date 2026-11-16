using System.Collections.Generic;

namespace Game.Logic.Levels
{
    public sealed class LevelReport
    {
        private readonly List<LevelProblem> _problems = new List<LevelProblem>();

        public IReadOnlyList<LevelProblem> Problems => _problems;
        public bool HasErrors { get; private set; }

        public override string ToString() => string.Join("; ", _problems);

        internal void Add(LevelProblem problem)
        {
            _problems.Add(problem);
            HasErrors |= problem.Severity == Severity.Error;
        }
    }
}
