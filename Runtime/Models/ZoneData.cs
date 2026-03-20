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
        public string id;
        public int zone_id;
        public List<StructureData> objectPlacementData = new();
        public List<EnemySpawnerData> enemySpawnAreas = new();
        public List<NPCSpawnerData> npcSpawnAreas = new();
        public List<PlayerSpawnerData> playerSpawnAreas = new();
    }
}
