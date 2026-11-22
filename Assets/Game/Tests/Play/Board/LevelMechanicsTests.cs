using Game.Logic.Play;
using Game.Tests.Levels;
using NUnit.Framework;

namespace Game.Tests.Play
{
    public sealed class LevelMechanicsTests
    {
        [Test]
        public void For_HandsTheIceToTheBoardAsAMoveRuleAndAGuard()
        {
            var mechanics = LevelMechanics.For(new LevelBuilder(3, 3).Rect(1, 0, 0, 1, 1).Build());

            CollectionAssert.Contains(mechanics.Rules, mechanics.Ice);
            CollectionAssert.Contains(mechanics.Guards, mechanics.Ice);
        }
    }
}
