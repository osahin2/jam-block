using UnityEngine;

namespace Game.Play
{
    public sealed class BoardRoot : MonoBehaviour
    {
        [SerializeField] private Transform _idlePieces;

        public Transform IdlePieces => _idlePieces;
    }
}
