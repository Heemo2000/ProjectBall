using Dreamteck.Splines;
using UnityEngine;

namespace Game.Gameplay
{
    [System.Serializable]
    public class ChunksData
    {
        public ChunkShape shape = ChunkShape.Straight;
        public Chunk[] chunks;
        [Min(5)]
        public int maxCount = 20;
    }
}
