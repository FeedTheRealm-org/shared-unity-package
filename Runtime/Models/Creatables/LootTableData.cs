using System;
using System.Collections.Generic;
using UnityEngine;

namespace FTRShared.Runtime.Models
{
    [Serializable]
    public class LootTableData
    {
        public string id;

        public string name = "Loot Table";

        public int minGoldDropAmount = 0;

        public int maxGoldDropAmount = 0;

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

        /// <summary>
        /// Represents a single loot entry in a loot table.
        /// Stores only the referenced consumable item id and its
        /// individual drop probability (0-100).
        ///
        /// The id field refers to a ConsumableItemData.
        /// </summary>
        [Serializable]
        public class LootEntryData
        {
            /// <summary>
            /// Id of the consumable item this entry refers to.
            /// </summary>
            public string id;

            /// <summary>
            /// Per-item drop probability (0-100).
            /// </summary>
            public int dropProbability = 0;

            public LootEntryData() { }

            public LootEntryData(string id, int dropProbability)
            {
                this.id = id;
                this.dropProbability = dropProbability;
            }
        }
    }
}
