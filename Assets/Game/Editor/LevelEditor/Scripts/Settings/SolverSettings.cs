using UnityEngine;

namespace Game.Editor.LevelEditor
{
    [CreateAssetMenu(menuName = "Game/Level Editor/Solver Settings")]
    public sealed class SolverSettings : ScriptableObject
    {
        [Tooltip("Milliseconds the solver may search after every change before it reports the level as not decided. Editing waits this long at most.")]
        [SerializeField, Min(0)] private int _quickCheckMilliseconds = 250;

        [Tooltip("Arrangements of the blocks Check fully may search before it gives up and reports the level as not decided.")]
        [SerializeField, Min(1)] private int _fullCheckBudget = 100000;

        public int QuickCheckMilliseconds => _quickCheckMilliseconds;
        public int FullCheckBudget => _fullCheckBudget;
    }
}
