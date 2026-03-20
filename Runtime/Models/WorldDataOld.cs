using System;
using System.Collections.Generic;
using UnityEngine;

namespace FTRShared.Runtime.Models
{
    [Serializable]
    public class WorldDataOld
    {
        public string id;
        public int zone_id = 0;
        private string _worldName = "New_FTR_World";
        public List<StructureData> objectPlacementData = new();
        public List<EnemySpawnerData> enemySpawnAreas = new();
        public List<NPCSpawnerData> npcSpawnAreas = new();
        public List<PlayerSpawnerData> playerSpawnAreas = new();
        public List<ConsumableItemData> consumableItems = new();
        public List<WeaponItemData> weaponItems = new();
        public List<EnemyData> enemies = new();
        public List<LootTableData> lootTables = new();
        public List<DialogData> dialogs = new();
        public List<NPCData> npcs = new();
        public List<QuestData> quests = new();
        public WorldShopsData worldShopsData = new();

        public string worldName
        {
            get => _worldName;
            set =>
                _worldName = System.Text.RegularExpressions.Regex.Replace(
                    value ?? "New_FTR_World",
                    @"[^a-zA-Z0-9_]",
                    "_"
                );
        }
    }
}
