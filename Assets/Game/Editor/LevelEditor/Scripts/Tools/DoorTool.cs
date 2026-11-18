using Game.Data.Levels;

namespace Game.Editor.LevelEditor
{
    // A press on the frame places a one-cell door in the chosen color; dragging along the side makes it longer. A
    // drag that starts on a door moves its nearer end, so a door can be made longer or shorter.
    public sealed class DoorTool : ILevelEditorTool
    {
        private int _door = LevelEdits.NoDoor;
        private int _anchor;

        public string Name => "Door";
        public string Tooltip => "Click the frame to place a door, drag along it to make the door longer.";

        public string Describe(LevelData level, Cell cell)
        {
            if (LevelEdits.DoorAt(level, cell) != LevelEdits.NoDoor)
                return "Door: drag one of its ends along the frame to change its length.";
            return LevelEdits.TryGetSide(level, cell, out _)
                ? "Frame: click, or drag along it, to place a door in the selected color."
                : "Doors go on the frame around the board.";
        }

        public bool Press(LevelData level, Cell cell, int colorId)
        {
            _door = LevelEdits.NoDoor;
            if (!LevelEdits.TryGetSide(level, cell, out var side))
                return false;
            var along = side.Along(cell);
            _door = LevelEdits.DoorAt(level, cell);
            if (_door != LevelEdits.NoDoor)
            {
                // The far end stays put, so dragging moves the end nearer to the press.
                var door = level.Doors[_door];
                var lastCell = door.End - 1;
                _anchor = along - door.Start <= lastCell - along ? lastCell : door.Start;
                return false;
            }
            _door = LevelEdits.AddDoor(level, colorId, side, along);
            _anchor = along;
            return _door != LevelEdits.NoDoor;
        }

        public bool Drag(LevelData level, Cell cell)
        {
            if (_door == LevelEdits.NoDoor)
                return false;
            var along = level.Doors[_door].Side.Along(cell);
            var start = along < _anchor ? along : _anchor;
            var end = (along > _anchor ? along : _anchor) + 1;
            return LevelEdits.TrySetDoorSpan(level, _door, start, end);
        }

        public void Release() => _door = LevelEdits.NoDoor;
    }
}
