using Game.Data.Levels;

namespace Game.Logic.Levels
{
    public sealed class DoorPlacementCheck : ILevelCheck
    {
        public void Check(LevelData level, LevelReport report)
        {
            for (var i = 0; i < level.Doors.Count; i++)
            {
                var door = level.Doors[i];
                if (!door.FitsSide(level.Width, level.Height))
                    report.Add(LevelProblem.DoorError(LevelProblemKind.DoorOutsideSide, i));
                if (OverlapsAnEarlierDoor(level, i))
                    report.Add(LevelProblem.DoorError(LevelProblemKind.DoorsOverlap, i));
            }
        }

        private static bool OverlapsAnEarlierDoor(LevelData level, int index)
        {
            var door = level.Doors[index];
            for (var i = 0; i < index; i++)
            {
                if (level.Doors[i].Overlaps(door.Side, door.Start, door.End))
                    return true;
            }
            return false;
        }
    }
}
