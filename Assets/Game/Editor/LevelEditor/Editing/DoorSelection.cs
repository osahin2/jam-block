using System.Collections.Generic;
using Game.Data.Levels;

namespace Game.Editor.LevelEditor.Editing
{
    // Holds the door itself, not its index, so removing an earlier door never makes it mean another one. A door moves
    // only along its own side.
    public sealed class DoorSelection : ILevelSelection
    {
        private readonly DoorData _door;
        private int _start;

        public DoorSelection(DoorData door)
        {
            _door = door;
        }

        public bool Exists(LevelData level) => level.Doors.Contains(_door);

        public int ColorId(LevelData level) => _door.ColorId;

        public string Describe(LevelData level) =>
            $"door on the {_door.Side.ToString().ToLowerInvariant()} side, {_door.Length} cells wide";

        public IEnumerable<Cell> Cells(LevelData level) => LevelEdits.DoorCells(level, level.Doors.IndexOf(_door));

        public void BeginMove(LevelData level) => _start = _door.Start;

        public bool MoveBy(LevelData level, Cell offset)
        {
            var start = _start + _door.Side.Along(offset);
            return LevelEdits.TrySetDoorSpan(level, level.Doors.IndexOf(_door), start, start + _door.Length);
        }

        public void Recolor(LevelData level, int colorId) => _door.ColorId = colorId;

        public void Delete(LevelData level) => level.Doors.Remove(_door);
    }
}
