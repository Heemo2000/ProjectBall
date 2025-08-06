using System;
using UnityEngine;

namespace Game.Gameplay
{
    [Serializable]
    public class SpawnData
    {
        public AnimationCurve shapeCurve;

        public Mesh[] meshes;
        [Min(5)]
        public int chunkCount = 10;

        [Min(10)]
        public int meshCount = 100;

        [Min(10.0f)]
        public float length = 10.0f;

        public override int GetHashCode()
        {
            return HashCode.Combine(shapeCurve.GetHashCode(), meshes.GetHashCode(), chunkCount, meshCount, length);
        }
    }
}
