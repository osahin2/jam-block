using System;
using Game.Logic.Play;

namespace Game.Play
{
    public interface IBooster
    {
        BoosterDefinition Definition { get; }

        bool IsAvailable(LevelAttempt attempt);

        void Use(BoosterContext context, Action onDone);

        void Cancel();
    }
}
