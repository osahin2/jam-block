using Game.Data.Levels;

namespace Game.Logic.Levels
{
    public readonly struct LevelProblem
    {
        public const int NoIndex = -1;

        public LevelProblemKind Kind { get; }
        public Severity Severity { get; }
        // The block or door the problem is about, by its index in the level.
        public int Index { get; }
        public Cell Cell { get; }

        private LevelProblem(LevelProblemKind kind, Severity severity, int index, Cell cell)
        {
            Kind = kind;
            Severity = severity;
            Index = index;
            Cell = cell;
        }

        public static LevelProblem Error(LevelProblemKind kind, int index = NoIndex, Cell cell = default) =>
            new LevelProblem(kind, Severity.Error, index, cell);

        public static LevelProblem Warning(LevelProblemKind kind, int index) =>
            new LevelProblem(kind, Severity.Warning, index, default);

        public override string ToString() => $"{Severity} {Kind} (index {Index}, cell {Cell})";
    }
}
