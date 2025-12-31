using System;
using System.Collections.Generic;
using UnityEngine;

namespace Models
{
    [Serializable]
    public class WorldData
    {
        [SerializeField]
        public string id;

        [SerializeField]
        private string _worldName = "New_FTR_World";

        [SerializeField]
        public List<StructureData> objectPlacementData = new();

        [SerializeField]
        public List<EnemySpawnAreaData> enemySpawnAreas = new();

        [SerializeField]
        public List<PlayerSpawnAreaData> playerSpawnAreas = new();
        public List<ConsumableItem> consumableItems = new();

        public string worldName
        {
            get => _worldName;
            set => _worldName = value?.Replace(" ", "_") ?? "New_FTR_World";
        }
    }
}
