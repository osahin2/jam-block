using System;
using System.Collections.Generic;
using Game.Data.Levels;
using Game.Logic.Levels;
using UnityEngine.UIElements;

namespace Game.Editor.LevelEditor
{
    public sealed class SolverPanel : VisualElement
    {
        private const string UnsolvableClass = "solver--unsolvable";
        private const string UnknownClass = "solver--unknown";

        public event Action PlaybackChanged;
        public event Action FullCheckRequested;

        private readonly Label _summary = new Label();
        private readonly Label _move = new Label();
        private readonly Button _checkFully;
        private readonly Button _show;
        private readonly Button _previous;
        private readonly Button _next;
        private readonly Button _close;
        private readonly Func<int, string> _blockName;
        private LevelSolution _solution;
        private LevelData _level;
        private bool _searchedFully;
        private SolutionPlayback _playback;

        public bool IsPlaying => _playback != null;
        public bool ShowsNextMove => IsPlaying && !_playback.IsAtEnd;
        public LevelData PlayedLevel => _playback.Shown;

        public SolverPanel(Func<int, string> blockName)
        {
            _blockName = blockName;
            AddToClassList("solver");
            _summary.AddToClassList("solver-summary");
            _move.AddToClassList("solver-move");
            var controls = new VisualElement();
            controls.AddToClassList("solver-controls");
            _checkFully = ControlButton(controls, "Check fully",
                "Searches every arrangement up to the full check budget in SolverSettings, with a progress bar you can cancel.",
                () => FullCheckRequested?.Invoke());
            _show = ControlButton(controls, "Show solution",
                "Plays the fewest moves back on the board, one at a time. The board cannot be edited until you close it.", Play);
            _previous = ControlButton(controls, "Back", "Goes back one move.", () => Step(_playback.Previous));
            _next = ControlButton(controls, "Next", "Plays the next move.", () => Step(_playback.Next));
            _close = ControlButton(controls, "Close", "Stops showing the solution and goes back to editing.", Close);
            Add(_summary);
            Add(controls);
            Add(_move);
        }

        public void Show(LevelSolution solution, LevelData level, bool searchedFully)
        {
            if (solution != _solution)
                _playback = null;
            _solution = solution;
            _level = level;
            _searchedFully = searchedFully;
            Draw();
        }

        public List<Cell> NextBlockCells() => _playback.NextBlockCells();

        public List<Cell> NextTargetCells() => _playback.NextTargetCells();

        private void Play()
        {
            _playback = new SolutionPlayback(_level, _solution);
            Draw();
            PlaybackChanged?.Invoke();
        }

        private void Step(Action step)
        {
            step();
            Draw();
            PlaybackChanged?.Invoke();
        }

        private void Close()
        {
            _playback = null;
            Draw();
            PlaybackChanged?.Invoke();
        }

        private void Draw()
        {
            _summary.text = Summary();
            EnableInClassList(UnsolvableClass, Has(Solvability.Unsolvable));
            EnableInClassList(UnknownClass, Has(Solvability.Unknown));
            Display(_checkFully, Has(Solvability.Unknown) && !_searchedFully);
            Display(_show, Has(Solvability.Solvable) && !IsPlaying);
            Display(_previous, IsPlaying);
            Display(_next, IsPlaying);
            Display(_close, IsPlaying);
            Display(_move, IsPlaying);
            if (!IsPlaying)
                return;
            _previous.SetEnabled(!_playback.IsAtStart);
            _next.SetEnabled(!_playback.IsAtEnd);
            _move.text = MoveText();
        }

        private bool Has(Solvability verdict) => _solution != null && _solution.Verdict == verdict;

        private string Summary() =>
            _solution != null ? SolverGuide.Summarize(_solution, _searchedFully) :"Fix the errors to check whether the level can be solved.";

        private string MoveText()
        {
            if (_playback.IsAtEnd)
                return $"All {SolverGuide.Plural(_playback.MoveCount, "move")} played: the board is clear.";
            var move = _playback.NextMove;
            var action = move.Leaves ? "leaves through its door from the marked place" : "moves to the marked place";
            return $"Move {_playback.Step + 1} of {_playback.MoveCount}: the {_blockName(move.Block)} {action}.";
        }

        private static Button ControlButton(VisualElement parent, string text, string tooltip, Action clicked)
        {
            var button = new Button(clicked) { text = text, tooltip = tooltip };
            button.AddToClassList("solver-button");
            parent.Add(button);
            return button;
        }

        private static void Display(VisualElement element, bool shown) =>
            element.style.display = shown ? DisplayStyle.Flex : DisplayStyle.None;
    }
}
