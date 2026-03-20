using System;
using System.Collections.Generic;

namespace FTRShared.Runtime.Models
{
    [Serializable]
    public class WorldCreatables
    {
        public List<ConsumableItemData> consumableItems = new();
        public List<WeaponItemData> weaponItems = new();
        public List<EnemyData> enemies = new();
        public List<LootTableData> lootTables = new();
        public List<DialogData> dialogs = new();
        public List<NPCData> npcs = new();
        public List<QuestData> quests = new();
        public WorldShopsData worldShopsData = new();
    }
}
