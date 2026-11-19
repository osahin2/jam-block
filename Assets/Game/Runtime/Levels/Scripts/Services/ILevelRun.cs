namespace Game.Levels
{
    // Which level the Game scene plays and what follows a win. Chosen once when the scene starts: the player's own run
    // or a designer's test from the level editor.
    public interface ILevelRun
    {
        LevelAsset Level { get; }

        // The level's number as the player sees it.
        int Number { get; }

        // After a win: moves on to the level that follows.
        void Advance();
    }
}
