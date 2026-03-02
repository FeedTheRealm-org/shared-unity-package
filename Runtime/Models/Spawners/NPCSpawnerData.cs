using System;
using UnityEngine;

namespace Models
{
    [Serializable]
    public class NPCSpawnerData : SpawnerData
    {
        public string NpcId = "";

        public NPCSpawnerData(Vector3 position, float radius, string NpcId)
            : base(position, radius)
        {
            this.NpcId = NpcId;
        }
    }
}
