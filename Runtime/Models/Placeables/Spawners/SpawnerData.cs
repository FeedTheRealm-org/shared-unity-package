using System;
using UnityEngine;

namespace FTRShared.Runtime.Models
{
    [Serializable]
    public class SpawnerData
    {
        public Vector3 Position;
        public float Radius = 3f;

        public SpawnerData(Vector3 position, float radius)
        {
            Position = position;
            Radius = radius;
        }
    }
}
