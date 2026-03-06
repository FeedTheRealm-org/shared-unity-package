using System;
using UnityEngine;

namespace FTRShared.Runtime.Models
{
    [Serializable]
    public class NPCSpawnerData : SpawnerData
    {
        public string npcId = "";

        public NPCSpawnerData(Vector3 position, float radius)
            : base(position, radius) { }

        public NPCSpawnerData(Vector3 position, float radius, string npcId)
            : base(position, radius)
        {
            this.npcId = npcId;
        }
    }
}
