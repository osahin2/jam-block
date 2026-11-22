using Game.Data.Levels;

namespace Game.Logic.Play
{
    public sealed class IceMechanic : IMoveRule, IBlockGuard
    {
        private readonly int[] _startingIce;
        private readonly int[] _hits;

        public IceMechanic(LevelData level)
        {
            _startingIce = new int[level.Blocks.Count];
            for (var i = 0; i < _startingIce.Length; i++)
                _startingIce[i] = level.Blocks[i].Ice;
            _hits = new int[_startingIce.Length];
        }

        public int IceLeft(IReadOnlyBoard board, int block)
        {
            if (block >= _startingIce.Length)
                return 0;
            return IceAfter(_startingIce[block], board.GoneCount + _hits[block]);
        }

        public static int IceAfter(int ice, int melted) => ice > melted ? ice - melted : 0;

        public bool Allows(IReadOnlyBoard board, int block, Direction direction) => IceLeft(board, block) == 0;

        public bool Guards(IReadOnlyBoard board, int block) => IceLeft(board, block) > 0;

        void IBlockGuard.TakeHit(int block) => _hits[block]++;
    }
}
