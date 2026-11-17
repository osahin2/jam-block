namespace Game.Logic.Play
{
    // Turns a finger position into whole-cell steps and a sub-cell offset. Positions are in cells from the board's
    // top-left corner, x to the right and y down.
    public sealed class BlockDrag
    {
        private const float HalfCell = 0.5f;

        private readonly Board _board;
        private readonly int _maxStepsPerMove;
        private float _grabX;
        private float _grabY;
        private int _exitDoor;

        public int Block { get; private set; } = Board.NoBlock;
        public bool IsActive => Block != Board.NoBlock;

        public BlockDrag(Board board)
        {
            _board = board;
            _maxStepsPerMove = board.Width + board.Height;
        }

        public void Begin(int block, float x, float y)
        {
            var position = _board.Blocks[block].Position;
            Block = block;
            _grabX = x - position.X;
            _grabY = y - position.Y;
        }

        public DragPose MoveTo(float x, float y)
        {
            var targetX = x - _grabX;
            var targetY = y - _grabY;
            _exitDoor = StepResult.NoDoor;
            for (var i = 0; i < _maxStepsPerMove; i++)
            {
                var position = _board.Blocks[Block].Position;
                var movedX = StepToward(targetX - position.X, Direction.Left, Direction.Right);
                if (_exitDoor != StepResult.NoDoor)
                    break;
                var movedY = StepToward(targetY - position.Y, Direction.Up, Direction.Down);
                if (_exitDoor != StepResult.NoDoor || !(movedX || movedY))
                    break;
            }

            var block = Block;
            var current = _board.Blocks[block].Position;
            if (_exitDoor != StepResult.NoDoor)
            {
                End();
                return new DragPose(block, current, 0f, 0f, _exitDoor);
            }
            var offsetX = Offset(targetX - current.X, Direction.Left, Direction.Right);
            var offsetY = Offset(targetY - current.Y, Direction.Up, Direction.Down);
            return new DragPose(block, current, offsetX, offsetY, StepResult.NoDoor);
        }

        public void End() => Block = Board.NoBlock;

        private bool StepToward(float delta, Direction backward, Direction forward)
        {
            if (delta > -HalfCell && delta < HalfCell)
                return false;
            var result = _board.Step(Block, delta > 0f ? forward : backward);
            _exitDoor = result.ExitDoor;
            return result.Moved;
        }

        private float Offset(float delta, Direction backward, Direction forward)
        {
            var min = _board.CanStep(Block, backward) ? -HalfCell : 0f;
            var max = _board.CanStep(Block, forward) ? HalfCell : 0f;
            return delta < min ? min : delta > max ? max : delta;
        }
    }
}
