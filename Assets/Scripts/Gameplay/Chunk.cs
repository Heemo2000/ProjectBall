using Dreamteck.Splines;
using UnityEngine;


namespace Game.Gameplay
{
    public class Chunk : MonoBehaviour
    {
        [SerializeField] private Transform endPoint;
        public Transform EndPoint { get => endPoint; set => endPoint = value; }
    }
}
