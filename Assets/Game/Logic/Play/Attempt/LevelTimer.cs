namespace Game.Logic.Play
{
    public sealed class LevelTimer
    {
        public float RemainingSeconds { get; private set; }
        public float FrozenSeconds { get; private set; }
        public bool HasStarted { get; private set; }
        public bool IsPaused { get; private set; }
        public bool HasExpired => RemainingSeconds <= 0f;
        public bool IsRunning => HasStarted && !IsPaused && !HasExpired;

        public LevelTimer(int limitSeconds)
        {
            RemainingSeconds = limitSeconds;
        }

        public void Start() => HasStarted = true;

        public void Pause() => IsPaused = true;

        public void Resume() => IsPaused = false;

        public void Freeze(float seconds) => FrozenSeconds = seconds;

        public bool Tick(float deltaSeconds)
        {
            if (!IsRunning)
                return false;
            var counted = deltaSeconds - FrozenSeconds;
            FrozenSeconds = counted < 0f ? -counted : 0f;
            if (counted <= 0f)
                return false;
            RemainingSeconds -= counted;
            if (RemainingSeconds > 0f)
                return false;
            RemainingSeconds = 0f;
            return true;
        }
    }
}
