using Game.Data.Levels;
using Game.Editor.LevelEditor;
using Game.Tests.Levels;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Game.Tests.LevelEditor
{
    public sealed class LevelDraftTests
    {
        private const int Red = 1;
        private const int Blue = 2;
        private const int Yellow = 3;

        private LevelDraft _draft;
        private EraseTool _erase;

        [SetUp]
        public void CreateDraft()
        {
            _draft = LevelDraft.Create();
            _erase = new EraseTool();
            _draft.Load(new LevelBuilder(5, 5)
                .Rect(Red, 0, 0, 2, 1)
                .Rect(Blue, 0, 2, 3, 1)
                .Rect(Yellow, 3, 3, 2, 2)
                .Door(Red, Side.Top, 0, 2)
                .Build());
        }

        [TearDown]
        public void DiscardDraft() => _draft.Discard();

        [Test]
        public void Undo_AfterEraseStrokes_BringsBackEachLevelInTurn()
        {
            var original = Snapshot();
            Stroke(new Cell(0, 0), new Cell(1, 0));
            var afterFirst = Snapshot();
            Stroke(new Cell(3, 3), new Cell(4, 3), new Cell(4, 4));
            var afterSecond = Snapshot();

            Undo.PerformUndo();
            Assert.AreEqual(afterFirst, Snapshot(), "the first undo takes back the second stroke");
            Undo.PerformUndo();
            Assert.AreEqual(original, Snapshot(), "the second undo takes back the first stroke");
            Undo.PerformRedo();
            Undo.PerformRedo();
            Assert.AreEqual(afterSecond, Snapshot());
        }

        [Test]
        public void StrokeThatChangesNothing_AddsNoUndoStep()
        {
            Stroke(new Cell(0, 0));
            var erased = Snapshot();
            Stroke(new Cell(2, 0));

            Undo.PerformUndo();

            Assert.AreNotEqual(erased, Snapshot(), "the undo took back the erase, not an empty stroke");
        }

        [Test]
        public void Load_ForgetsTheUndoHistoryOfTheLevelBefore()
        {
            Stroke(new Cell(0, 0));
            var loaded = new LevelBuilder(3, 3).Rect(Blue, 0, 0, 1, 1).Build();
            _draft.Load(loaded);

            Undo.PerformUndo();

            Assert.IsTrue(_draft.Matches(loaded));
        }

        private void Stroke(Cell press, params Cell[] drags)
        {
            _draft.BeginStroke();
            _erase.Press(_draft.Level, press, Red);
            foreach (var cell in drags)
                _erase.Drag(_draft.Level, cell);
            _erase.Release();
            _draft.EndStroke(_erase.Name);
        }

        private string Snapshot() => JsonUtility.ToJson(_draft.Level);
    }
}
