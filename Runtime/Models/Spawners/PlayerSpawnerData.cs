using System;
using UnityEngine;

namespace Models
{
    [Serializable]
    public class PlayerSpawnerData : SpawnerData
    {
        [SerializeField]
        public Vector3 Position;
        public float Radius = 3f;

        public PlayerSpawnerData(Vector3 position, float radius)
            : base(position, radius) { }
    }
}
