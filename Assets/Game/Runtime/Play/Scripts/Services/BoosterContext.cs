using Game.Logic.Play;

namespace Game.Play
{
    public sealed class BoosterContext
    {
        public LevelAttempt Attempt { get; }
        public BoardInput Input { get; }
        public BoardPresenter Board { get; }
        public AimingOverlay Overlay { get; }
        public BoosterShots Shots { get; }

        public BoosterContext(LevelAttempt attempt, BoardInput input, BoardPresenter board, AimingOverlay overlay,
            BoosterShots shots)
        {
            Attempt = attempt;
            Input = input;
            Board = board;
            Overlay = overlay;
            Shots = shots;
        }
    }
}
