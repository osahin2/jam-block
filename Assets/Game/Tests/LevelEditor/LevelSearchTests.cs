using Game.Editor.LevelEditor.Levels;
using NUnit.Framework;

namespace Game.Tests.LevelEditor
{
    public sealed class LevelSearchTests
    {
        [TestCase("")]
        [TestCase("   ")]
        public void EmptyQuery_MatchesEveryLevel(string query) => Assert.IsTrue(LevelSearch.Matches(7, "Level_07", query));

        [Test]
        public void PartOfTheName_MatchesInAnyCase() => Assert.IsTrue(LevelSearch.Matches(1, "Tutorial_Doors", "doors"));

        [Test]
        public void PlayNumber_MatchesALevelWhoseNameDoesNotHaveIt() =>
            Assert.IsTrue(LevelSearch.Matches(537, "Level_12", "537"));

        [Test]
        public void SurroundingSpaces_AreIgnored() => Assert.IsTrue(LevelSearch.Matches(3, "Level_03", " 3 "));

        [Test]
        public void NeitherNumberNorName_DoesNotMatch() => Assert.IsFalse(LevelSearch.Matches(4, "Level_04", "12"));
    }
}
