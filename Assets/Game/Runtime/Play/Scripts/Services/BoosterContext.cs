using Game.Logic.Play;

namespace Game.Play
{
    public sealed class BoosterContext
    {
        public LevelAttempt Attempt { get; }
        public BoardInteraction Board { get; }
        public AimingOverlay Overlay { get; }
        public BoosterShots Shots { get; }

        public BoosterContext(LevelAttempt attempt, BoardInteraction board, AimingOverlay overlay, BoosterShots shots)
        {
            Attempt = attempt;
            Board = board;
            Overlay = overlay;
            Shots = shots;
        }
    }
}
