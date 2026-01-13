using System;
using UnityEngine;

namespace Models
{
    [Serializable]
    public class NPCSpawnerData : SpawnerData
    {
        [SerializeField]
        public Vector3 Position;
        public float Radius = 3f;

        public NPCSpawnerData(Vector3 position, float radius)
            : base(position, radius) { }
    }
}
