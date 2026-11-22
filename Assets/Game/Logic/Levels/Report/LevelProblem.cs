using Game.Data.Levels;

namespace Game.Logic.Levels
{
    public readonly struct LevelProblem
    {
        public const int NoIndex = -1;

        public LevelProblemKind Kind { get; }
        public Severity Severity { get; }
        public LevelPart Part { get; }
        public int Index { get; }
        public Cell? Cell { get; }

        private LevelProblem(LevelProblemKind kind, Severity severity, LevelPart part, int index, Cell? cell)
        {
            Kind = kind;
            Severity = severity;
            Part = part;
            Index = index;
            Cell = cell;
        }

        public static LevelProblem LevelError(LevelProblemKind kind) =>
            new LevelProblem(kind, Severity.Error, LevelPart.Level, NoIndex, null);

        public static LevelProblem BlockError(LevelProblemKind kind, int index, Cell? cell = null) =>
            new LevelProblem(kind, Severity.Error, LevelPart.Block, index, cell);

        public static LevelProblem BlockWarning(LevelProblemKind kind, int index) =>
            new LevelProblem(kind, Severity.Warning, LevelPart.Block, index, null);

        public static LevelProblem DoorError(LevelProblemKind kind, int index) =>
            new LevelProblem(kind, Severity.Error, LevelPart.Door, index, null);

        public override string ToString() => $"{Severity} {Kind} ({Part} {Index}, cell {Cell})";
    }
}
