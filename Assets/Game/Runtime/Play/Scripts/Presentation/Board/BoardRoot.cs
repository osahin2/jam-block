using UnityEngine;

namespace Game.Play
{
    // The Game scene's place for the board: a level is built under this object, and the pooled pieces no level uses
    // wait inactive under the idle object, so both live and die with the scene.
    public sealed class BoardRoot : MonoBehaviour
    {
        [SerializeField] private Transform _idlePieces;

        public Transform IdlePieces => _idlePieces;
    }
}
