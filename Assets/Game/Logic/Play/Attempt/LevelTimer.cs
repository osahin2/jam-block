namespace Game.Logic.Play
{
    public sealed class LevelTimer
    {
        private float _frozenSeconds;

        public float RemainingSeconds { get; private set; }
        public bool HasStarted { get; private set; }
        public bool IsPaused { get; private set; }
        public bool HasExpired => RemainingSeconds <= 0f;
        public bool IsRunning => HasStarted && !IsPaused && !HasExpired;
        public bool IsFrozen => _frozenSeconds > 0f;

        public LevelTimer(int limitSeconds)
        {
            RemainingSeconds = limitSeconds;
        }

        public void Start() => HasStarted = true;

        public void Pause() => IsPaused = true;

        public void Resume() => IsPaused = false;

        public void Freeze(float seconds) => _frozenSeconds = seconds;

        public bool Tick(float deltaSeconds)
        {
            if (!IsRunning)
                return false;
            var counted = deltaSeconds - _frozenSeconds;
            _frozenSeconds = counted < 0f ? -counted : 0f;
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
