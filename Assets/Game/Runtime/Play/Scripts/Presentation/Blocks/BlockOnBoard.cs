using Game.Logic.Play;

namespace Game.Play
{
    public readonly struct BlockOnBoard
    {
        public IReadOnlyBoard Board { get; }
        public LevelMechanics Mechanics { get; }
        public int Index { get; }
        public BoardBlock Block => Board.Blocks[Index];

        public BlockOnBoard(IReadOnlyBoard board, LevelMechanics mechanics, int index)
        {
            Board = board;
            Mechanics = mechanics;
            Index = index;
        }

        public static BlockOnBoard Of(LevelAttempt attempt, int index) =>
            new BlockOnBoard(attempt.Board, attempt.Mechanics, index);
    }
}
