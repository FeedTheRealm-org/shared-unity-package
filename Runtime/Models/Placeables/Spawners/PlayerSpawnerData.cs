using System;
using UnityEngine;

namespace FTRShared.Runtime.Models
{
    [Serializable]
    public class PlayerSpawnerData : SpawnerData
    {
        public PlayerSpawnerData(Vector3 position, float radius)
            : base(position, radius) { }
    }
}
