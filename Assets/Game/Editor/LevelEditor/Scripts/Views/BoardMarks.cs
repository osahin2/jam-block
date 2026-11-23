using System.Collections.Generic;
using Game.Data.Levels;
using Game.Logic.Levels;

namespace Game.Editor.LevelEditor
{
    public sealed class BoardMarks
    {
        private const string SelectedClass = "selected";
        private const string FaultClass = "problem-fault";
        private const string FocusClass = "problem-focus";
        private const string SolutionFromClass = "solution-from";
        private const string SolutionToClass = "solution-to";

        private readonly List<BoardMark> _marks = new List<BoardMark>();

        public IReadOnlyList<BoardMark> All => _marks;

        public void Clear() => _marks.Clear();

        public void AddProblems(LevelReport report, LevelData level)
        {
            foreach (var problem in report.Problems)
                AddProblem(problem, level, $"problem--{LevelProblemList.SeverityName(problem.Severity)}");
        }

        public void AddFocus(LevelProblem problem, LevelData level) => AddProblem(problem, level, FocusClass);

        public void AddSelection(IEnumerable<Cell> cells) => Add(cells, SelectedClass);

        public void AddNextMove(IEnumerable<Cell> from, IEnumerable<Cell> to)
        {
            Add(from, SolutionFromClass);
            Add(to, SolutionToClass);
        }

        private void AddProblem(LevelProblem problem, LevelData level, string className)
        {
            var subject = LevelProblemGuide.SubjectOf(problem, level);
            if (subject != null)
                Add(subject.Cells(level), className);
            if (problem.Cell.HasValue)
                _marks.Add(new BoardMark(problem.Cell.Value, FaultClass));
        }

        private void Add(IEnumerable<Cell> cells, string className)
        {
            foreach (var cell in cells)
                _marks.Add(new BoardMark(cell, className));
        }
    }
}
