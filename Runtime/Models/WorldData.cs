using System;
using System.Collections.Generic;
using UnityEngine;

namespace Models {

    [Serializable]
    public class WorldData {
        [SerializeField] public string id; // TODO: check if this breaks when publishing world
        [SerializeField] public string worldName = "NewWorld";
        [SerializeField] public List<StructureData> objectPlacementData;
        [SerializeField] public List<EnemySpawnAreaData> enemySpawnAreas;
        [SerializeField] public List<PlayerSpawnAreaData> playerSpawnAreas;
        public List<ConsumableItem> consumableItems = new List<ConsumableItem>();
    }
}