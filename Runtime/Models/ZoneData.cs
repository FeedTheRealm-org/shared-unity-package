using System;
using System.Collections.Generic;
using UnityEngine;

namespace FTRShared.Runtime.Models
{
    /// <summary>
    /// Zones only contain the data relevant placeables, like structures and spawners
    /// </summary>
    [Serializable]
    public class ZoneData
    {
        public ZoneData(string worldId, int zoneId)
        {
            this.worldId = worldId;
            this.zoneId = zoneId;
            last_edited_at = DateTime.Now;
        }

        public string worldId;
        public int zoneId;
        public List<StructureData> objectPlacementData = new();
        public List<EnemySpawnerData> enemySpawnAreas = new();
        public List<NPCSpawnerData> npcSpawnAreas = new();
        public List<PlayerSpawnerData> playerSpawnAreas = new();
        public DateTime last_edited_at;
        public DateTime published_at;
    }
}
