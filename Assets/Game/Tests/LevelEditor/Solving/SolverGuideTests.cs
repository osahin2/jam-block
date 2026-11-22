using System;
using Game.Editor.LevelEditor;
using Game.Logic.Levels;
using NUnit.Framework;

namespace Game.Tests.LevelEditor
{
    public sealed class SolverGuideTests
    {
        [Test]
        public void EveryVerdict_HasASummary()
        {
            foreach (Solvability verdict in Enum.GetValues(typeof(Solvability)))
                Assert.IsTrue(SolverGuide.Explains(verdict), verdict.ToString());
        }
    }
}
