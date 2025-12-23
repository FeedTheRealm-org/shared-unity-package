using System;
using System.Collections.Generic;
using UnityEngine;

namespace Models {

    [Serializable]
    public class WorldData {
        [SerializeField]
        public string id;
        [SerializeField]
        public string worldName = "New World";
        [SerializeField]
        public List<PlacedAsset> objectPlacementData;
        [SerializeField]
        public List<EnemySpawnAreaData> enemySpawnAreas;
        public List<ConsumableItem> consumableItems = new List<ConsumableItem>();

        //[SerializeField]
        public List<EnemyData> enemies = new List<EnemyData>();
    }
}