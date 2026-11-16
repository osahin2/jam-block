namespace Core.Levels
{
    public interface ILevelProgression
    {
        // 1-based number shown to the player; keeps counting after the content loops.
        int CurrentLevelNumber { get; }

        // Index into the game's level list; after the last level it loops as LevelLoop describes.
        int CurrentLevelIndex { get; }

        // Saves immediately.
        void CompleteCurrentLevel();
    }
}
