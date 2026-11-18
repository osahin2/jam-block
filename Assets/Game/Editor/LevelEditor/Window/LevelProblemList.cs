using System;
using Game.Data.Levels;
using Game.Editor.LevelEditor.Editing;
using Game.Logic.Levels;
using UnityEngine.UIElements;

namespace Game.Editor.LevelEditor.Window
{
    // The problem list under the board: a count, then one row per problem saying what it is about and what is wrong.
    // Hovering a row focuses its problem; clicking a row about a block or door picks that block or door.
    public sealed class LevelProblemList : VisualElement
    {
        public const int NoProblem = -1;

        public event Action FocusChanged;
        public event Action<ILevelSelection> Picked;

        private readonly Label _header = new Label();
        private readonly ScrollView _rows = new ScrollView();
        private readonly Func<ILevelSelection, string> _nameOf;

        // The index of the problem under the pointer in the last report shown, or NoProblem.
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

        // The USS modifier for a severity: "error" or "warning".
        public static string SeverityName(Severity severity) => severity.ToString().ToLowerInvariant();

        public void Show(LevelReport report, LevelData level)
        {
            var problems = report.Problems;
            if (Focused >= problems.Count)
                Focused = NoProblem;
            _rows.Clear();
            var errors = 0;
            for (var i = 0; i < problems.Count; i++)
            {
                _rows.Add(Row(i, problems[i], level));
                errors += problems[i].Severity == Severity.Error ? 1 : 0;
            }
            var warnings = problems.Count - errors;
            _header.text = problems.Count == 0 ? "No problems."
                : $"{Plural(errors, "error")}, {Plural(warnings, "warning")}." + (errors > 0 ? " Fix the errors to save." : "");
        }

        private VisualElement Row(int index, LevelProblem problem, LevelData level)
        {
            var subject = LevelProblemGuide.SubjectOf(problem, level);
            var row = new VisualElement();
            row.AddToClassList("problem-row");
            row.AddToClassList($"problem-row--{SeverityName(problem.Severity)}");
            var title = new Label($"{problem.Severity}: {(subject != null ? _nameOf(subject) : "Level")}");
            title.AddToClassList("problem-title");
            var explanation = new Label(LevelProblemGuide.Explain(problem.Kind));
            explanation.AddToClassList("problem-text");
            row.Add(title);
            row.Add(explanation);
            row.RegisterCallback<PointerEnterEvent>(_ => Focus(index));
            row.RegisterCallback<PointerLeaveEvent>(_ => Focus(NoProblem));
            if (subject != null)
            {
                row.AddToClassList("problem-row--selectable");
                row.tooltip = "Click to select it on the board.";
                row.RegisterCallback<ClickEvent>(_ => Picked?.Invoke(subject));
            }
            return row;
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
