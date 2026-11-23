using Game.Data.Levels;

namespace Game.Editor.LevelEditor
{
    public sealed class DoorTool : ILevelEditorTool
    {
        private int _door = LevelLookup.NoDoor;
        private int _anchor;

        public string Name => "Door";
        public string Tooltip => "Click the frame to place a door, drag along it to make the door longer.";

        public string Describe(LevelData level, Cell cell)
        {
            if (LevelLookup.DoorAt(level, cell) != LevelLookup.NoDoor)
                return "Door: drag one of its ends along the frame to change its length.";
            return LevelLookup.TryGetSide(level, cell, out _)
                ? "Frame: click, or drag along it, to place a door in the selected color."
                : "Doors go on the frame around the board.";
        }

        public bool Press(LevelData level, Cell cell, int colorId)
        {
            _door = LevelLookup.NoDoor;
            if (!LevelLookup.TryGetSide(level, cell, out var side))
                return false;
            var along = side.Along(cell);
            _door = LevelLookup.DoorAt(level, cell);
            if (_door != LevelLookup.NoDoor)
            {
                _anchor = FartherEndOf(level.Doors[_door], along);
                return false;
            }
            _anchor = along;
            return LevelEdits.TryAddDoor(level, colorId, side, along, out _door);
        }

        public bool Drag(LevelData level, Cell cell)
        {
            if (_door == LevelLookup.NoDoor)
                return false;
            var along = level.Doors[_door].Side.Along(cell);
            var start = along < _anchor ? along : _anchor;
            var end = (along > _anchor ? along : _anchor) + 1;
            return LevelEdits.TrySetDoorSpan(level, _door, start, end);
        }

        public void Release() => _door = LevelLookup.NoDoor;

        public bool RightClick(LevelData level, Cell cell) => false;

        private static int FartherEndOf(DoorData door, int along)
        {
            var lastCell = door.End - 1;
            return along - door.Start <= lastCell - along ? lastCell : door.Start;
        }
    }
}
