using System;
using UnityEngine;

namespace Models
{
    [Serializable]
    public class PlayerSpawnAreaData
    {
        [SerializeField]
        public Vector3 Position;
        public int Size = 1;

        public PlayerSpawnAreaData(Vector3 position, int size)
        {
            Position = position;
            Size = size;
        }
    }
}
