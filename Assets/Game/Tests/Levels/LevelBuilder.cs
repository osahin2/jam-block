using System.Collections.Generic;
using Game.Data.Levels;

namespace Game.Tests.Levels
{
    public sealed class LevelBuilder
    {
        private readonly LevelData _level;

        public LevelBuilder(int width, int height)
        {
            _level = new LevelData { Width = width, Height = height };
        }

        public LevelBuilder Block(int colorId, int x, int y, params Cell[] shape)
        {
            _level.Blocks.Add(new BlockData { ColorId = colorId, Position = new Cell(x, y), Shape = new List<Cell>(shape) });
            return this;
        }

        public LevelBuilder Rect(int colorId, int x, int y, int width, int height)
        {
            var shape = new List<Cell>();
            for (var row = 0; row < height; row++)
            {
                for (var column = 0; column < width; column++)
                    shape.Add(new Cell(column, row));
            }
            return Block(colorId, x, y, shape.ToArray());
        }

        public LevelBuilder Ice(int ice)
        {
            _level.Blocks[_level.Blocks.Count - 1].Ice = ice;
            return this;
        }

        public LevelBuilder Arrow(ArrowAxis axis, int startX = 0, int startY = 0, int length = 1)
        {
            _level.Blocks[_level.Blocks.Count - 1].Arrow =
                new ArrowData { Axis = axis, Start = new Cell(startX, startY), Length = length };
            return this;
        }

        public LevelBuilder Door(int colorId, Side side, int start, int length = 1)
        {
            _level.Doors.Add(new DoorData { ColorId = colorId, Side = side, Start = start, Length = length });
            return this;
        }

        public LevelData Build() => _level;
    }
}
