namespace Core.Levels
{
    public readonly struct LevelLoop
    {
        public int LevelCount { get; }

        // Index the content restarts from once every level has been completed once.
        public int LoopStartIndex { get; }

        public LevelLoop(int levelCount, int loopStartIndex)
        {
            LevelCount = levelCount;
            LoopStartIndex = loopStartIndex;
        }
    }
}
