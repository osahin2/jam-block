namespace Game.Editor.LevelEditor
{
    public static class EditorText
    {
        public static string Plural(int count, string word) => count == 1 ? $"1 {word}" : $"{count} {word}s";
    }
}
