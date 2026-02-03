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
        public List<EnemySpawnerData> enemySpawnAreas = new();

        [SerializeField]
        public List<NPCSpawnerData> npcSpawnAreas = new();

        [SerializeField]
        public List<PlayerSpawnerData> playerSpawnAreas = new();
        public List<ConsumableItemData> consumableItems = new();
        public List<WeaponItemData> weaponItems = new();
        public List<EnemyData> enemies = new();
        public List<LootTableData> lootTables = new();
        public List<DialogData> dialogs = new();

        public ShopData shopData = new();

        public string worldName
        {
            get => _worldName;
            set => _worldName = value?.Replace(" ", "_") ?? "New_FTR_World";
        }

        public override string ToString()
        {
            return $"World Name: {worldName}\n"
                + $"Structures: {objectPlacementData.Count}\n"
                + $"Enemy Spawners: {enemies.Count}\n"
                + $"NPC Spawners: {npcSpawnAreas.Count}\n"
                + $"Player Spawners: {playerSpawnAreas.Count}";
        }
    }
}
