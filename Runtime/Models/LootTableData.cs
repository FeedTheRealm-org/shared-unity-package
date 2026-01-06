using System;
using System.Collections.Generic;
using UnityEngine;

namespace Models
{
    [Serializable]
    public class LootTableData
    {
        [SerializeField]
        public string id;

        [SerializeField]
        public string name = "Loot Table";

        [SerializeField]
        public int minGoldDropAmount = 0;

        [SerializeField]
        public int maxGoldDropAmount = 0;

        [SerializeField]
        public List<LootEntryData> lootItems = new();

        public LootTableData(
            string id,
            string name,
            int minGoldDropAmount,
            int maxGoldDropAmount,
            List<LootEntryData> lootItems
        )
        {
            this.id = id;
            this.name = name;
            this.minGoldDropAmount = minGoldDropAmount;
            this.maxGoldDropAmount = maxGoldDropAmount;
            this.lootItems = lootItems ?? new List<LootEntryData>();
        }
    }
}
