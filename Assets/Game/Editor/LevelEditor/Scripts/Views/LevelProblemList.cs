using System;
using Game.Data.Levels;
using Game.Logic.Levels;
using UnityEngine.UIElements;

namespace Game.Editor.LevelEditor
{
    public sealed class LevelProblemList : VisualElement
    {
        public const int NoProblem = -1;

        public event Action FocusChanged;
        public event Action<ILevelSelection> Picked;

        private readonly Label _header = new Label();
        private readonly ScrollView _rows = new ScrollView();
        private readonly Func<ILevelSelection, string> _nameOf;

        public int Focused { get; private set; } = NoProblem;

        public LevelProblemList(Func<ILevelSelection, string> nameOf)
        {
            _nameOf = nameOf;
            AddToClassList("problems");
            _header.AddToClassList("problems-header");
            _rows.AddToClassList("problem-list");
            Add(_header);
            Add(_rows);
        }

        public static string SeverityName(Severity severity) => severity.ToString().ToLowerInvariant();

        public void Show(LevelReport report, LevelData level)
        {
            var problems = report.Problems;
            if (Focused >= problems.Count)
                Focused = NoProblem;
            _rows.Clear();
            for (var i = 0; i < problems.Count; i++)
                _rows.Add(Row(i, problems[i], level));
            _header.text = Summary(problems.Count, ErrorsIn(report));
        }

        private static int ErrorsIn(LevelReport report)
        {
            var errors = 0;
            foreach (var problem in report.Problems)
                errors += problem.Severity == Severity.Error ? 1 : 0;
            return errors;
        }

        private static string Summary(int problems, int errors)
        {
            if (problems == 0)
                return "No problems.";
            var counts = $"{Plural(errors, "error")}, {Plural(problems - errors, "warning")}.";
            return errors > 0 ? counts + " Fix the errors to save." : counts;
        }

        private VisualElement Row(int index, LevelProblem problem, LevelData level)
        {
            var subject = LevelProblemGuide.SubjectOf(problem, level);
            var row = new VisualElement();
            row.AddToClassList("problem-row");
            row.AddToClassList($"problem-row--{SeverityName(problem.Severity)}");
            row.Add(TextLabel($"{problem.Severity}: {(subject != null ? _nameOf(subject) : "Level")}", "problem-title"));
            row.Add(TextLabel(LevelProblemGuide.Explain(problem.Kind), "problem-text"));
            row.RegisterCallback<PointerEnterEvent>(_ => Focus(index));
            row.RegisterCallback<PointerLeaveEvent>(_ => Focus(NoProblem));
            if (subject != null)
                LetPick(row, subject);
            return row;
        }

        private void LetPick(VisualElement row, ILevelSelection subject)
        {
            row.AddToClassList("problem-row--selectable");
            row.tooltip = "Click to select it on the board.";
            row.RegisterCallback<ClickEvent>(_ => Picked?.Invoke(subject));
        }

        private static Label TextLabel(string text, string className)
        {
            var label = new Label(text);
            label.AddToClassList(className);
            return label;
        }

        private void Focus(int index)
        {
            if (index == Focused)
                return;
            Focused = index;
            FocusChanged?.Invoke();
        }

        private static string Plural(int count, string word) => count == 1 ? $"1 {word}" : $"{count} {word}s";
    }
}
