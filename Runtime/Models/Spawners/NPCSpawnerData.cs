using System;
using UnityEngine;

namespace FTRShared.Runtime.Models
{
    [Serializable]
    public class NPCSpawnerData : SpawnerData
    {
        public NPCSpawnerData(Vector3 position, float radius)
            : base(position, radius) { }
    }
}
